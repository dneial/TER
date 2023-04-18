using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;


public class LSystemMenu : EditorWindow
{
    [MenuItem("GameObject/L-System")]
    public static void showWindow() {
        EditorWindow window = GetWindow(typeof(LSystemMenu));
        window.Show();
    }

    Lsystem lsystem;
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
        string[] files = Directory.GetFiles(Application.dataPath + "/Grammar/", "*.lsys");

        int cursor = 0;
        foreach (string path in files){
            files[cursor] = Path.GetFileName(path);
            cursor++;
        }

        LConfig lConfig = new LConfig();
        if(File.Exists(Application.dataPath + "/Config.json")){
            lConfig = JsonUtility.FromJson<LConfig>(File.ReadAllText(Application.dataPath + "/Config.json"));
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

        numGrammar = EditorGUILayout.Popup("Grammar", numGrammar, files);

        if(GUILayout.Button("Generate")){
            saveConfig(new Config(thickness, length, configName, nbIteration, angle, noise, branch_chance, files[numGrammar]));

            lsystem = LsystemInterpretor.ParseFile(Application.dataPath + "/Grammar/" + files[numGrammar]);

            lsystem.Generate(nbIteration);

            parent = new GameObject();



            GetPoints(lsystem.current);
            PlaceBranches(points[0]);
        
            Debug.Log(lsystem.current);
        }
    }


    private void saveConfig(Config config){
        var path = Application.dataPath + "/Config.json";
        LConfig lConfig = new LConfig();
        if(File.Exists(path)){
            lConfig = JsonUtility.FromJson<LConfig>(File.ReadAllText(path));
        }
        lConfig.myConfigs.Add(config);
        File.WriteAllText(path, JsonUtility.ToJson(lConfig));
    }

    private void chargeConfig(Config config, string[] files){
        configName = config.name;
        this.lsystem.thickness = config.thickness;
        this.lsystem.length = config.length;
        this.lsystem.nbIteration = config.nbIteration;
        this.lsystem.angle = config.angle;
        this.lsystem.noise = config.noise;
        this.lsystem.branch_chance = config.branch_chance;
        for (int i = 0; i < files.Length; i++){
            if(files[i].Equals(config.grammar)){
                numGrammar = i;
                return;
            }
        }
    }

}