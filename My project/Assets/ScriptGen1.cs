using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptGen1 : MonoBehaviour
{
    Lsystem lsystem;

    List<Branche> points = new List<Branche>();

    LsystemInterpretor lSysInterp;

    GameObject parent;
    public int nbIteration = 4;
    [Range(0, 180)]
    public float angle;
    [Range(0, 100)]
    public float noise = 5f;
    public float length = 0.75f;
    [Range(0, 1)]
    public float branch_chance = 0.8f;
    

    public UnityEngine.Object grammarFile;


    //Awake is called before Start
    void Awake()
    {
        string grammar = Application.dataPath + "/Grammar/" + grammarFile.name+".lsys";

        lsystem = new Lsystem(new List<string>(), new List<string>(), "", new Dictionary<char, List<Rule>>());
        LsystemInterpretor lSysInterp = new LsystemInterpretor(grammar, lsystem, nbIteration);
        
        lSysInterp.interpret();
        angle = lsystem.angle;
        
        lsystem.Generate(nbIteration);

        Debug.Log(lsystem.ToString());
    }

    // Start is called before the first frame update
    void Start()
    {
        parent = gameObject;

        GetPoints(lsystem.current);
        PlaceBranchesv2(points[0]);
        
        Debug.Log(lsystem.current);
    }

    //reset is called when the user hits the reset button in the inspector's context menu or when adding the component the first time
    void Reset()
    {
        //reset the public variables to their original values
        nbIteration = 4;
        angle = 25f;
        noise = 5f;
        length = 0.75f;
        branch_chance = 1f;
    }

    //regles d'interprétation 3D :

    // F : Se déplacer d’un pas unitaire (∈ V)
    // + : Tourner à gauche d’angle α (∈ S)
    // - : Tourner à droite d’un angle α (∈ S)
    // & : Pivoter vers le bas d’un angle α (∈ S)
    // ^ : Pivoter vers le haut d’un angle α (∈ S)
    // < : Roulez vers la gauche d’un angle α (∈ S)
    // > : Roulez vers la droite d’un angle α (∈ S)
    // | : Tourner sur soi-même de 180° (∈ S)
    // [ : Sauvegarder la position courante (∈ S)
    // ] : Restaurer la dernière position sauvée (∈ S)

    public void GetPoints(string current)
    {
        Vector3 pos = parent.transform.position;
        Vector3 dir = Vector3.up;

        Stack<Vector3> posStack = new Stack<Vector3>();
        Stack<Vector3> dirStack = new Stack<Vector3>();
        Stack<Branche> racineStack = new Stack<Branche>();

        Branche racine = new Branche(0, pos);
        points.Add(racine);
        racineStack.Push(racine);

        for (int i = 0, cpt = 0; i < current.Length; i++)
        {
            char c = current[i];
            if (lsystem.variables.Contains(c.ToString()))
            {
                Vector3 nextpos = pos + dir * length;
                Branche branche = new Branche(++cpt, nextpos);
                racine.addChild(branche);
                points.Add(branche);
                racine = branche;
                pos = nextpos;
            }
            else if (c == '+')
            {
                dir = Quaternion.AngleAxis(addNoise(angle, noise), Vector3.forward) * dir;
            }
            else if (c == '-')
            {
                dir = Quaternion.AngleAxis(-addNoise(angle, noise), Vector3.forward) * dir;
            }
            else if (c == '&')
            {
                dir = Quaternion.AngleAxis(addNoise(angle, noise), Vector3.right) * dir;
            }
            else if (c == '^')
            {
                dir = Quaternion.AngleAxis(-addNoise(angle, noise), Vector3.right) * dir;
            }
            else if (c == '<')
            {
                dir = Quaternion.AngleAxis(addNoise(angle, noise), Vector3.up) * dir;
            }
            else if (c == '>')
            {
                dir = Quaternion.AngleAxis(-addNoise(angle, noise), Vector3.up) * dir;
            }
            else if (c == '|')
            {
                dir = Quaternion.AngleAxis(180, Vector3.up) * dir;
            }
            else if (c == '[')
            {
                // chance of skipping a branch
                if (UnityEngine.Random.value >= this.branch_chance)
                {
                    int j = findClosingBracket(current, i);
                    if (j == -1)
                    {
                        break;
                    }
                    i = j;
                    continue;
                }
                posStack.Push(pos);
                dirStack.Push(dir);
                racineStack.Push(racine);
            }
            else if (c == ']')
            {
                dir = dirStack.Pop();
                pos = posStack.Pop();
                racine = racineStack.Pop();
            }
        }
    }

    public void PrintPoints()
    {
        for (int i = 0; i < points.Count; i += 1)
        {
            Debug.Log(points[i]);
        }
    }

    public void PlaceBranches()
    {
        for (int i = 0; i < points.Count; i += 1)
        {
            Branche branche = points[i];
            Vector3 start = branche.getPosition();
            foreach(var fils in branche.getChildren())
            {
                Vector3 end = fils.getPosition();
                GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);

                line.name = "edge (" + branche.getId() + " " + fils.getId() + ")";
                line.transform.SetParent(parent.transform);

                line.transform.position = (start + end) / 2;
                line.transform.localScale = new Vector3(0.1f, 0.1f, Vector3.Distance(start, end));
                line.transform.LookAt(end);

            }
        }
    }

    //fonction récursive pour placer les branches
    public void PlaceBranchesv2(Branche b)
    {
        Vector3 start = b.getPosition();
        foreach (var fils in b.getChildren())
        {
            Vector3 end = fils.getPosition();
            fils.setGameObject(GameObject.CreatePrimitive(PrimitiveType.Cube));

            fils.getGameObject().name = "edge (" + b.getId() + " " + fils.getId() + ")";
            fils.getGameObject().transform.SetParent(parent.transform);

            fils.getGameObject().transform.position = (start + end) / 2;
            fils.getGameObject().transform.localScale = new Vector3(0.1f, 0.1f, Vector3.Distance(start, end));
            fils.getGameObject().transform.LookAt(end);

            PlaceBranchesv2(fils);
        }
    }
   



    //add noise to the angle
    public float addNoise(float angle, float noiseAmmount)
    {
        float noise = UnityEngine.Random.Range(-noiseAmmount, noiseAmmount);
        return angle + noise;
    }

    private int findClosingBracket(string s, int i)
    {
        int cpt = 0;
        for (int j = i; j < s.Length; j++)
        {
            if (s[j] == '[')
            {
                cpt++;
            }
            else if (s[j] == ']')
            {
                cpt--;
            }
            if (cpt == 0)
            {
                return j;
            }
        }
        return -1;
    }
    
}
