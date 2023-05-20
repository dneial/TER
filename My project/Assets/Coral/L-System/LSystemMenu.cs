using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public struct Config {
    public string name;
    public float length;
    public int nbIteration;
    public float angle;
    public string grammar;

    public Config(float length, string name, int nbIteration, float angle, string grammar){
        this.name = name;   
        this.length = length;
        this.nbIteration = nbIteration;
        this.angle = angle;
        this.grammar = grammar;
    }
}

[System.Serializable]
public class LConfig {
    public List<Config> myConfigs = new List<Config>();
}

public class LSystemMenu : EditorWindow
{
    [MenuItem("Coral/L-System")]
    public static void showWindow() {
        EditorWindow window = GetWindow(typeof(LSystemMenu));
        window.Show();
    }

    public Lsystem lsystem;
    public GameObject parent;
    static float length = 0.75f;
    static int nbIteration = 3;
    static float angle = 25;

    static bool display = true;
    List<INode> points;

    //static int grammar = 0;


    int numConfig;
    string configName = "";
    int numGrammar;
    void OnGUI()
    {
        string[] tmp = Directory.GetFiles(Application.dataPath + "/Coral/L-System/Grammar/", "*.lsys?");

        //trier les fichiers par ordre de leur extension
        string[] files = sortbyExt(tmp);

        int cursor = 0;
        foreach (string path in files){
            files[cursor] = Path.GetFileName(path);
            cursor++;
        }

        LConfig lConfig = new LConfig();
        if(File.Exists(Application.dataPath + "/Coral/L-System/Config.json")){
            lConfig = JsonUtility.FromJson<LConfig>(File.ReadAllText(Application.dataPath + "/Coral/L-System/Config.json"));
        }
        string[] nameLConfig = new string[lConfig.myConfigs.Count+1];

        nameLConfig[0] = "None";
        cursor = 1;
        foreach (var jConfig in lConfig.myConfigs) {
            nameLConfig[cursor] = jConfig.name;
            cursor++;
        }

        numConfig = EditorGUILayout.Popup("Pre-Config", numConfig, nameLConfig);

        if(GUILayout.Button("Charger la config")){
            if(numConfig > 0){
                chargeConfig(lConfig.myConfigs[numConfig-1], files);
            }
        }

        GUILayout.Label("Config Name");
        configName = EditorGUILayout.TextField("", configName);

        GUILayout.Label("Length");
        length = EditorGUILayout.Slider(length, 0, 1);

        GUILayout.Label("angle");
        angle = EditorGUILayout.Slider(angle, 0, 360);

        GUILayout.Label("nbIteration");
        nbIteration = EditorGUILayout.IntSlider(nbIteration, 1, 10);

        GUILayout.Label("Display");
        display = EditorGUILayout.Toggle(display);

        numGrammar = EditorGUILayout.Popup("Grammar", numGrammar, files);

       

        if(GUILayout.Button("Generate")){
            parent = new GameObject();
            points = new List<INode>();


            lsystem = LsystemInterpretor.ParseFile(Application.dataPath + "/Coral/L-System/Grammar/" + files[numGrammar]);
            lsystem.Generate(nbIteration);
            //Debug.Log(lsystem.current);

            //if extension is .lsys
            if (files[numGrammar].EndsWith(".lsys"))
            {
                //traduction de la gammaire lsystemV1 en lsystemV2
                lsystem.trad(length, angle);
            }
            LSystemGen generator = new LSystemGen(lsystem, parent);
            
            points = generator.ParseAndPlace(lsystem.current, display);
   
            
        }

        if(GUILayout.Button("Save configuration")){
            SavePopup popup = ScriptableObject.CreateInstance<SavePopup>();
            if(configName == ""){
                popup.setMsg("Nom de configuration vide !\nEchec de l'enregistrement");
            } else if (saveConfig(new Config(length, configName, nbIteration, angle, files[numGrammar]))){
                popup.setMsg("Enregistrement réussi");
            } else {
                popup.setMsg("Ce nom est déja pris !\nEchec de l'enregistrement");
            }
            popup.minSize = new Vector2(200, 90);
            popup.maxSize = new Vector2(200, 90);
            popup.Show();
        }

        if(GUILayout.Button("Afficher Branches")){    

            if (parent == null || points == null || points.Count == 0 || lsystem == null)
            {
                Debug.Log("No branches to display");
            }
            else
            {
                LSystemGen generator = new LSystemGen(lsystem, parent);
                foreach (INode point in points){
                    generator.displayBranch((Branche) point, parent);
                }
                //vider la liste des points pour ne pas afficher les branches plusieurs fois
                points.Clear();
                

            }  
        }

        //combiner les meshes
        if(GUILayout.Button("Combine Meshes")){
            if (parent == null)
            {
                Debug.Log("No branches to combine");
            }
            else
            {
                MeshCombiner combiner = new MeshCombiner(parent);
                combiner.combineMeshes();
            }
        }   
    }


    private bool saveConfig(Config config){
        var path = Application.dataPath + "/Coral/L-System/Config.json";
        LConfig lConfig = new LConfig();
        if(File.Exists(path)){
            lConfig = JsonUtility.FromJson<LConfig>(File.ReadAllText(path));
            foreach (var jConfig in lConfig.myConfigs) {
                if(jConfig.name == config.name){
                    return false;
                }
            }
        }
        lConfig.myConfigs.Add(config);
        File.WriteAllText(path, JsonUtility.ToJson(lConfig));
        return true;
    }

    private void chargeConfig(Config config, string[] files){
        configName = config.name;
        length = config.length;
        nbIteration = config.nbIteration;
        angle = config.angle;
        for (int i = 0; i < files.Length; i++){
            if(files[i].Equals(config.grammar)){
                numGrammar = i;
                return;
            }
        }
    }

    public string[] sortbyExt(string[] files){
        string[] res = new string[files.Length];
        int cursor = 0;
        foreach (string path in files){
            if (path.EndsWith(".lsys")){
                res[cursor] = path;
                cursor++;
            }
        }
        foreach (string path in files){
            if (path.EndsWith(".lsys2")){
                res[cursor] = path;
                cursor++;
            }
        }
        return res;
    }

}