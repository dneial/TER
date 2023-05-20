using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class LSystemGen
{
    public Lsystem lsystem;
    public GameObject parent;
    public int nbBranches = 0;
    public List<INode> branches;
    public Stack<Branche> stackBranches;
    public Stack<Branche> stackNodes;
    public Stack<BranchState> stateStack;

    public LSystemGen(Lsystem lsystem, GameObject parent)
    {
        this.lsystem = lsystem;
        this.parent = parent;

        this.branches = new List<INode>();
        this.stackBranches = new Stack<Branche>();
        this.stackNodes = new Stack<Branche>();
        this.stateStack = new Stack<BranchState>();
    }


    //regles d'interprétation 3D :

    // F : Se déplacer d’un pas unitaire
    // R : Radius (épaisseur de la branche)
    // h (heading)  : rotation horizontale
    // p (pitch)    : rotation verticale
    // r (roll)     : rotation sur soi même

    // Exemple de grammaire :

    // A = r(180,180) F0.5 H
    // B = F(5,1.0) R4.5
    // C = h-40 F(12, 2.0) R5 [ D ] B
    // D = r90 h40 F(12,2.0) R5 [ G ] [ E ] B
    // E = h20 r180 p(20,-2) F(12,2.0) R5 [ C ] B
    // G = h-20 p(20,2) F(12,2.0) R5 [ C ] B
    // H = h20 F(12,2.0) R5 [ C ] B

    // example de chaine de génération :

    // 0 : A
    // 1 : r(180,180) F0.5 H
    // 2 : r(180,180) F0.5 h20 F(12,2.0) R5 [ C ] B
    // 3 : r(180,180) F0.5 h20 F(12,2.0) R5 [ h-40 F(12,2.0) R5 [ D ] B ] F(5,1.0) R4.5

    public List<INode> ParseAndPlace(string current, bool displayOn)
    {
        //lit la chaine de caractère et construit les branches en fonction des règles de production

        //default values
        BranchState currentState = new BranchState( 0f, 0f, 0f, 0.1f, 1f, 0f);
        stateStack.Push(new BranchState(currentState));
        
        //split the string by spaces
        string[] words = current.Split(' ');

        //for each word do the appropriate action
        
        //if the first character is h, p, r or R update the heading, pitch, roll or Radius
        //if the first character is F, move forward
        //if the first character is [, push the current state
        //if the first character is ], pop the current state

        foreach (string word in words)
        {
            if(word == "")
            {
                //Debug.Log("empty word");
            }
            else if (word[0] == 'h')
            {
                //update heading
                currentState.heading += valueParse(word.Substring(1));
            }
            else if (word[0] == 'p')
            {
                //update pitch
                currentState.pitch += valueParse(word.Substring(1));
            }
            else if (word[0] == 'r')
            {
                //update roll
                currentState.roll += valueParse(word.Substring(1));
            }
            else if (word[0] == 'R')
            {
                //change radius
                currentState.radius = valueParse(word.Substring(1));
            }
            else if (word[0] == 'F')
            {
                //move forward
                //if there are parameters after the F, use them
                if (word.Length != 1)
                {
                    currentState.step = valueParse(word.Substring(1));                
                }
                
                //create a new instance of Branche EACH TIME
                Branche b;
                
                if (stackBranches.Count == 0) {
                    //create first branch from gameobject position
                    b = new Branche(++nbBranches, parent.transform.position);
                }
                else {
                    //create branch from last branch
                    b = new Branche(++nbBranches, stackBranches.Peek());
                }

                b.setState(new BranchState(currentState));
                calculatePosition(b);

                if(displayOn)
                {
                    displayBranch(b, parent);
                }

                //add the branch to the list
                branches.Add(b);

                //add the branch to the stack
                //Debug.Log("pushing new branch : " + b.id);
                stackBranches.Push(b);

            }
            else if (word[0] == '[')
            {
                //Debug.Log("pushing state : " + currentState + "");
                //push the current state
                stateStack.Push(new BranchState(currentState));

                //on va voir si ça casse tout : 
                //on push la dernière branche créée avant le branchement
                stackNodes.Push(stackBranches.Peek());
            }
            else if (word[0] == ']')
            {
                //pop the current state
                currentState.update(stateStack.Pop());

                //après ']', le prochain F doit avoir pour origine la dernière branche créée avant le branchement
                //c'est à dire celle qu'on a push dans stackNodes
                //on pop toutes les branches jusqu'à ce qu'on tombe sur la dernière branche créée avant le branchement
                //PUIS on pop le noeud, au pire on le remettra dans stockNodes si on retombe sur '['

                while (stackBranches.Peek().id > stackNodes.Peek().id)
                {
                    stackBranches.Pop();
                }
                //on devrait pouvoir retomber sur nos pattes avec ça inchallah hein

                //on pop le noeud du coup
                stackNodes.Pop();
                                
            }
            else if(lsystem.variables.Contains(word[0].ToString()))
            {
                //si c'est une variable qui n'a pas été interprétée on fait rien ?
                //Debug.Log("variable non interprétée dans cette itération : " + word[0]);
            }
            else
            {
                Debug.Log("ERREUR ? : " + word[0]);
            }

            //si le mot est le dernier de la chaine ou ']', on arrondit le bout de la branche
            if (displayOn && word != "" && (word == words[words.Length - 1] || word == "]" || word == "] "))
            {
                Debug.Log("dernier mot de la chaine : " + word);
                //dernière branche créée :
                Branche tmp = stackBranches.Peek();
                Vector3 pos = tmp.getGPosition();
                Vector3 rota = tmp.getGRotation();

                //son extremité
                Vector3 branchEnd = pos + Quaternion.Euler(rota.x, rota.y, rota.z) *
                                        Vector3.up *
                                        (tmp.branchState.step/2);
                
                //on place la sphère
                //placeSphere(tmp, branchEnd, rota);
            }
            
        }

        //clear all stacks
        stackBranches.Clear();
        stackNodes.Clear();
        stateStack.Clear();

        //return the list of branches
        return branches;
    }

    //fonction qui calcule la position et place la branche par rapport à son parent
    public void calculatePosition(Branche b)
    {
        //get the parent of b
        Branche parent = (Branche) b.parent;

        //bunch of variables
        Vector3 parentPos, parentRot, parentExtremity, newPos;
        parentPos = parentRot = parentExtremity = newPos = Vector3.zero;


        if (parent != null)
        {
            //get the parent position
            parentPos = parent.position;

            //get the parent rotation
            parentRot = parent.rotation;

            //get the parent extremity
            parentExtremity = parentPos + Quaternion.Euler(parentRot.x, parentRot.y, parentRot.z) *
                                        Vector3.up *
                                        (parent.branchState.step/2);
            
            //little debugging spheres to see the parent's extremity
            // placeSphere(parent, parentExtremity, parentRot);
        } 
    
        //calculate the new position of the child branch
        newPos = parentExtremity + Quaternion.Euler(b.branchState.pitch, b.branchState.roll, b.branchState.heading) * Vector3.up * (b.branchState.step/2);
        
        //place the child branch at the new position
        b.setPosition(newPos);
        
        //set its rotation 
        b.setRotation(new Vector3(b.branchState.pitch, b.branchState.roll, b.branchState.heading));
        
        
    }

    public void displayBranch(Branche b, GameObject parent)
    {
        b.addGameObject(parent);
        b.setGPosition(b.position);
        b.setGRotation(b.rotation);
  
        //cubes and cylinders don't have the same scale in unity so :
        b.gameobject.transform.localScale = new Vector3(b.branchState.radius, b.branchState.step/2, b.branchState.radius); //cylinder
        //b.gameobject.transform.localScale = new Vector3(0.1f, b.branchState.step, 0.1f); //cube

        //extremité 
        Vector3 branchEnd = b.position + Quaternion.Euler(b.rotation.x, b.rotation.y, b.rotation.z) *
                                        Vector3.up *
                                        (b.branchState.step/2);
        placeSphere(b, branchEnd, b.rotation);
    }

    //fonction qui parse la chaine passée en paramètre 
    //et retourne la valeur (float) correspondante
    private float valueParse(string s){
        
        //deux types de valeurs possibles :
        // (5,1.0) -> 5 + random(-1.0, 1.0)
        // -40     -> -40

        if(s[0] == '('){
            string[] values = s.Substring(1, s.Length - 2).Split(',');  // {"5", "1.0"}
            float value = float.Parse(values[0].Replace('.', ','));
            float randomizer = float.Parse(values[1].Replace('.', ','));

            value += Random.Range(-randomizer, randomizer);
            return value;
        } else {
            return float.Parse(s.Replace('.', ','));
        }
    }

    public void placeSphere(Branche b, Vector3 pos, Vector3 rot)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = pos;
        sphere.transform.rotation = Quaternion.Euler(rot);
        sphere.transform.localScale = new Vector3(b.gameobject.transform.localScale.x, b.gameobject.transform.localScale.x, b.gameobject.transform.localScale.x);
        sphere.transform.parent = b.gameobject.transform;
        //random color
        // sphere.GetComponent<Renderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        //red
        // sphere.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
    }

    public void Printbranches()
    {
        for (int i = 0; i < branches.Count; i += 1)
        {
            Debug.Log(branches[i]);
        }
    } 

}
