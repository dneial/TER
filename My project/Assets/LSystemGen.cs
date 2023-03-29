using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class Config {
    public string name;
    public float thickness;
    public float length;
    public int nbIteration;
    public float angle;
    public float noise;
    public float branch_chance;
    public string grammar;

    public Config(float thickness, float length, string name, int nbIteration, float angle, float noise, float branch_chance, string grammar){
        this.name = name;   
        this.thickness = thickness;
        this.length = length;
        this.nbIteration = nbIteration;
        this.angle = angle;
        this.noise = noise;
        this.branch_chance = branch_chance;
        this.grammar = grammar;
    }
}

public class LSystemGen : EditorWindow
{

    [MenuItem("GameObject/L-System")]
    public static void showWindow() {
        EditorWindow window = GetWindow(typeof(LSystemGen));
        window.Show();
    }

    Lsystem lsystem;
    List<Branche> points = new List<Branche>();
    GameObject parent;

    static float thickness = 1;
    static float length = 0.75f;
    static int nbIteration = 4;
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

        string[] files = Directory.GetFiles(Application.dataPath + "/Grammar/", "*.lsys");

        int cursor = 0;
        foreach (string path in files){
            files[cursor] = Path.GetFileName(path);
            cursor++;
        }

        grammar = EditorGUILayout.Popup("Grammar", grammar, files); 

        if(GUILayout.Button("Generate")){
            //saveConfig(new Config(thickness, length, "test", nbIteration, angle, noise, branch_chance, Application.dataPath + "/Grammar/" + files[grammar]));

            lsystem = LsystemInterpretor.ParseFile(Application.dataPath + "/Grammar/" + files[grammar]);

            lsystem.Generate(nbIteration);

            parent = new GameObject();

            GetPoints(lsystem.current);
            PlaceBranches(points[0]);
        
            Debug.Log(lsystem.current);
        }

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
                if (UnityEngine.Random.value >= branch_chance)
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

    //fonction récursive pour placer les branches
    private void PlaceBranches(Branche b)
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

            PlaceBranches(fils);
        }
    }



    //add noise to the angle
    private float addNoise(float angle, float noiseAmmount)
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
    
}