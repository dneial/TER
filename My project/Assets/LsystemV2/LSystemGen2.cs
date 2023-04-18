using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class LSystemGen2 : EditorWindow
{

    [MenuItem("GameObject/L-System2")]
    public static void showWindow() {
        EditorWindow window = GetWindow(typeof(LSystemGen2));
        window.Show();
    }

    LsystemV2 lsystem;
    GameObject parent;

    static float thickness = 1;
    static float length = 0.75f;
    static int nbIteration = 3;
    static float angle = 25;
    static float noise = 5f;
    static float branch_chance = 0.8f;
    static int grammar = 0;



    void OnGUI()
    {
        GUILayout.Label("Config Name");
        name = EditorGUILayout.TextField("");

        GUILayout.Label("Thickness");
        thickness = EditorGUILayout.Slider(thickness, 0, 1);

        GUILayout.Label("Length");
        length = EditorGUILayout.Slider(length, 0, 1);

        GUILayout.Label("nbIteration");
        nbIteration = EditorGUILayout.IntSlider(nbIteration, 1, 10);

        GUILayout.Label("angle");
        angle = EditorGUILayout.Slider(angle, 0, 360);
        
        GUILayout.Label("noise");
        noise = EditorGUILayout.Slider(noise, 0, 360);

        GUILayout.Label("branch_chance");
        branch_chance = EditorGUILayout.Slider(branch_chance, 0, 1);

        string[] files = Directory.GetFiles(Application.dataPath + "/GrammarV2/", "*.lsys");

        int cursor = 0;
        foreach (string path in files){
             files[cursor] = Path.GetFileName(path);
            cursor++;
        }

        grammar = EditorGUILayout.Popup("Grammar", grammar, files); 

        if(GUILayout.Button("Generate")){
            //saveConfig(new Config(thickness, length, "test", nbIteration, angle, noise, branch_chance, Application.dataPath + "/Grammar/" + files[grammar]));

            lsystem = LsystemInterpretorV2.ParseFile(Application.dataPath + "/GrammarV2/" + files[grammar]);

            lsystem.Generate(nbIteration);

            parent = new GameObject();

            ParseAndPlace(lsystem.current);
        
            Debug.Log("\nFIN : "+lsystem.current+"\n-----------\n");
        }
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
  
    
    //compteur du nombre de branches
    int nbBranches = 0;

    //liste des branches
    List<BrancheV2> branches = new List<BrancheV2>();

    //2 piles : branche courante et embranchement 
    Stack<BrancheV2> stackBranches = new Stack<BrancheV2>();
    Stack<BrancheV2> stackNodes = new Stack<BrancheV2>();

    //pile des états des branches
    Stack<BranchState> stateStack = new Stack<BranchState>();

    public void ParseAndPlace(string current)
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
            if (word[0] == 'h')
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
                
                //create a new instance of BrancheV2 EACH TIME
                BrancheV2 b;
                
                if (stackBranches.Count == 0) {
                    //create first branch from gameobject position
                    b = new BrancheV2(++nbBranches, parent.transform.position);
                }
                else {
                    //create branch from last branch
                    b = new BrancheV2(++nbBranches, stackBranches.Peek());
                }

                b.addGameObject(parent);
                b.setState(new BranchState(currentState));
                calculatePosition(b);

                //add the branch to the list
                branches.Add(b);

                //add the branch to the stack
                Debug.Log("pushing new branch : " + b.id);
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
            if (word == words[words.Length - 1] || word[0] == ']')
            {
                //dernière branche créée :
                BrancheV2 tmp = stackBranches.Peek();
                Vector3 pos = tmp.gameobject.transform.position;
                Vector3 rota = tmp.gameobject.transform.rotation.eulerAngles;

                //son extremité
                Vector3 branchEnd = pos + Quaternion.Euler(rota.x, rota.y, rota.z) *
                                        Vector3.up *
                                        (tmp.branchState.step/2);
                
                //on place la sphère
                placeSphere(tmp, branchEnd, rota);
            }
            
        }

        //clear all stacks
        stackBranches.Clear();
        stackNodes.Clear();
        stateStack.Clear();
    }

    //fonction qui calcule la position et place la branche par rapport à son parent
    private void calculatePosition(BrancheV2 b)
    {
        //get the parent of b
        BrancheV2 parent = b.parent;

        //bunch of variables
        Vector3 parentPos, parentRot, parentExtremity, newPos;
        parentPos = parentRot = parentExtremity = newPos = Vector3.zero;


        if (parent != null)
        {
            //get the parent position
            parentPos = parent.gameobject.transform.position;

            //get the parent rotation
            parentRot = parent.gameobject.transform.rotation.eulerAngles;

            //get the parent extremity
            parentExtremity = parentPos + Quaternion.Euler(parentRot.x, parentRot.y, parentRot.z) *
                                        Vector3.up *
                                        (parent.branchState.step/2);
            
            //little debugging spheres to see the parent's extremity
            placeSphere(parent, parentExtremity, parentRot);
        } 
    
        //calculate the new position of the child branch
        newPos = parentExtremity + Quaternion.Euler(b.branchState.pitch, b.branchState.roll, b.branchState.heading) * Vector3.up * (b.branchState.step/2);
        
        //place the child branch at the new position
        b.setPosition(newPos);
        
        //set its rotation 
        b.gameobject.transform.rotation = Quaternion.Euler(b.branchState.pitch, b.branchState.roll, b.branchState.heading);
        
        //adjust its scale and thickness
        //b.gameobject.transform.localScale = new Vector3(b.branchState.radius, b.branchState.step, b.branchState.radius);
       
        //cubes and cylinders don't have the same scale in unity so :
        b.gameobject.transform.localScale = new Vector3(b.branchState.radius, b.branchState.step/2, b.branchState.radius); //cylinder

        // b.gameobject.transform.localScale = new Vector3(0.1f, b.branchState.step, 0.1f); //cube
        
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

    public void placeSphere(BrancheV2 b, Vector3 pos, Vector3 rot)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = pos;
        sphere.transform.rotation = Quaternion.Euler(rot);
        sphere.transform.localScale = new Vector3(b.gameobject.transform.localScale.x, b.gameobject.transform.localScale.x, b.gameobject.transform.localScale.x);
        sphere.transform.parent = b.gameobject.transform;
        sphere.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
    }

    public void Printbranches()
    {
        for (int i = 0; i < branches.Count; i += 1)
        {
            Debug.Log(branches[i]);
        }
    } 

    private void saveConfig(Config config){
        var path = Application.dataPath + "/Config.json";
        List<Config> lConfig = new List<Config>();
        if(File.Exists(path)){
            //lConfig = JsonUtility.FromJson<List<Config>>(File.ReadAllText(path));
        } else {
            File.Create(path);
        }
        lConfig.Add(config);
        File.WriteAllText(path, JsonUtility.ToJson(lConfig));
    }

public class PointInPolyhedronTest : MonoBehaviour
{
    // The eight vertices of the polyhedron
    public Vector3 a, b, c, d, e, f, g, h;

    // The point to test
    public Vector3 x;

    void Start()
    {
        // Define the six faces of the polyhedron
        Vector3[] faceABCD = { a, b, c, d };
        Vector3[] faceADHE = { a, d, h, e };
        Vector3[] faceEFGH = { e, f, g, h };
        Vector3[] faceBFHG = { b, f, h, g };
        Vector3[] faceACGF = { a, c, g, f };
        Vector3[] faceBDEC = { b, d, e, c };

        // Test if the point is inside any of the six faces
        bool isInside = PointInPolyhedron(x, faceABCD) ||
                        PointInPolyhedron(x, faceADHE) ||
                        PointInPolyhedron(x, faceEFGH) ||
                        PointInPolyhedron(x, faceBFHG) ||
                        PointInPolyhedron(x, faceACGF) ||
                        PointInPolyhedron(x, faceBDEC);

        Debug.Log(isInside);
    }

    // Check if a point is inside a convex polyhedron defined by its vertices
    bool PointInPolyhedron(Vector3 point, Vector3[] vertices)
    {
        // Compute the normal of the plane defined by the first three vertices
        Vector3 normal = Vector3.Cross(vertices[1] - vertices[0], vertices[2] - vertices[0]);

        // Compute the signed distance between the point and the plane
        float signedDistance = Vector3.Dot(point - vertices[0], normal);

        // If the point is on the "outside" side of the plane, it is not inside the polyhedron
        if (signedDistance < 0)
            return false;

        // Check if the point is inside each triangle formed by the remaining vertices
        for (int i = 0; i < vertices.Length - 2; i++)
        {
            Vector3 v1 = vertices[i + 1] - vertices[0];
            Vector3 v2 = vertices[i + 2] - vertices[0];
            Vector3 v3 = point - vertices[0];

            // Compute the barycentric coordinates of the point with respect to the triangle
            float dot11 = Vector3.Dot(v1, v1);
            float dot12 = Vector3.Dot(v1, v2);
            float dot13 = Vector3.Dot(v1, v3);
            float dot22 = Vector3.Dot(v2, v2);
            float dot23 = Vector3.Dot(v2, v3);
            float invDenom = 1 / (dot11 * dot22 - dot12 * dot12);
            float u = (dot22 * dot13 - dot12 * dot23) * invDenom;
            float v = (dot11 * dot23 - dot12 * dot13) * invDenom;

            // If the barycentric coordinates are both non-negative and their sum is less than 1, 
            // the point is inside the triangle and thus inside the polyhedron
            if (u >= 0 && v >= 0 && u + v < 1)
            return true;
    }

    // If the point is not inside any of the triangles, it is not inside the polyhedron
    return false;
    }
}

}
