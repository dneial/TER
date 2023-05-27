using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public struct ConfigSpaceColo {
    public string name;
    public float leaf_influence_radius;
    public float leaf_kill_distance;
    public int influence_points;
    public AnimationCurve thickness;
    public string prefab;
    public float height;
    public int max_iterations;
    public int new_leaves;

    public ConfigSpaceColo(string name, float leaf_influence_radius, float leaf_kill_distance,
    int influence_points, AnimationCurve thickness, string prefab, float height, int max_iterations, int new_leaves){
        this.name = name;
        this.leaf_influence_radius = leaf_influence_radius;
        this.leaf_kill_distance = leaf_kill_distance;
        this.influence_points = influence_points;
        this.thickness = thickness;
        this.prefab = prefab;
        this.height = height;
        this.max_iterations = max_iterations;
        this.new_leaves = new_leaves;
    }
}

[System.Serializable]
public class LConfigSpaceColo {
    public List<ConfigSpaceColo> myConfigs = new List<ConfigSpaceColo>();
}

public class SpaceColonizationMenu : EditorWindow
{
    Vector2 scrollPos;

    [MenuItem("Coral/Space Colonization")]
    public static void showWindow() {
        EditorWindow window = GetWindow(typeof(SpaceColonizationMenu));
        window.Show();
    }



    static float leaf_influence_radius = 1f;
    static float leaf_kill_distance = 1f;
    static int influence_points = 100;
    static AnimationCurve thickness = AnimationCurve.EaseInOut(0,0.35f,1,1);
    static GameObject prefab;
    static float height = 10f;
    static int max_iterations = 1000;
    static int new_leaves = 5;
    static int numConfig;
    static string configName = "";

    private SpaceColonization generator;
    private SpaceColonizationView view;


    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        scrollPos =
            EditorGUILayout.BeginScrollView(scrollPos);


        LConfigSpaceColo lConfig = new LConfigSpaceColo();
        if(File.Exists(Application.dataPath + "/Coral/SpaceColonisation/Config.json")){
            lConfig = JsonUtility.FromJson<LConfigSpaceColo>(File.ReadAllText(Application.dataPath + "/Coral/SpaceColonisation/Config.json"));
        }
        string[] nameLConfig = new string[lConfig.myConfigs.Count+1];

        nameLConfig[0] = "None";
        int cursor = 1;
        foreach (var jConfig in lConfig.myConfigs) {
            nameLConfig[cursor] = jConfig.name;
            cursor++;
        }

        numConfig = EditorGUILayout.Popup("Pre-Config", numConfig, nameLConfig);

        if(GUILayout.Button("Charger la config")){
            if(numConfig > 0){
                chargeConfig(lConfig.myConfigs[numConfig-1]);
            }
        }

        GUILayout.Label("Config Name");
        configName = EditorGUILayout.TextField("", configName);

        GUILayout.Label("Leaf Influence Radius");
        leaf_influence_radius = EditorGUILayout.Slider(leaf_influence_radius, 10, 100);

        GUILayout.Label("Leaf Kill Distance");
        leaf_kill_distance = EditorGUILayout.Slider(leaf_kill_distance, 1, 10);

        GUILayout.Label("Influence Points");
        influence_points = EditorGUILayout.IntSlider(influence_points, 50, 1000);

        GUILayout.Label("Volume Prefab");
        prefab = EditorGUILayout.ObjectField(prefab, typeof(GameObject), true) as GameObject;

        GUILayout.Label("Volume Height");
        height = EditorGUILayout.Slider(height, 0.1f, 10);

        GUILayout.Label("Max Iterations");
        max_iterations = EditorGUILayout.IntSlider(max_iterations, 1, 1000);

        GUIContent content = new GUIContent("New Leaves", "Number of new leaves to add on each iteration");
        new_leaves = EditorGUILayout.IntSlider(content, new_leaves, 0, 100);

        thickness.AddKey(0.4f, 0.9f);
        thickness.AddKey(0.3f, 0.8f);
        thickness.AddKey(0.95f, 1);
        thickness.SmoothTangents(3, -1);
        thickness = EditorGUILayout.CurveField("Thickness", thickness);

      
        if(GUILayout.Button("Generate"))
        {
            if(prefab == null)
            {
                prefab = AssetDatabase.LoadAssetAtPath("Assets/Coral/SpaceColonisation/Blender_msh/AsimBox.fbx", typeof(GameObject)) as GameObject;
            }

            GameObject go = Instantiate(prefab, new Vector3(0, height, 0), Quaternion.identity);
            
            if(go.GetComponent<MeshCollider>() == null)
            {
                // Debug.Log("No mesh collider found, adding one");
                go.AddComponent<MeshCollider>();
            }
            
            Bounds bounds = go.GetComponent<MeshCollider>().bounds;
            
            generator = new SpaceColonization(bounds, leaf_kill_distance, leaf_influence_radius, influence_points);
            view = new SpaceColonizationView();

            DestroyImmediate(go);

            generator.Generate(max_iterations, new_leaves);
            this.view.update(this.generator.GetNodes(), thickness);
            
            Debug.Log("Generated after " + generator.steps + " steps");

            if (this.view.GetRoot() == null)
            {
                Debug.Log("No branches to combine");
            }
            else
            {
                MeshCombiner combiner = new MeshCombiner(this.view.GetRoot());
                combiner.combineMeshes();
            }
            
        }

        if(GUILayout.Button("Save configuration")){
            SavePopup popup = ScriptableObject.CreateInstance<SavePopup>();
            if(prefab == null){
                prefab = AssetDatabase.LoadAssetAtPath("Assets/Coral/SpaceColonisation/Blender_msh/AsimBox.fbx", typeof(GameObject)) as GameObject;
            }
            if(configName == ""){
                popup.setMsg("Nom de configuration vide !\nEchec de l'enregistrement");
            } else if (saveConfig(new ConfigSpaceColo(configName, leaf_influence_radius, leaf_kill_distance,
            influence_points, thickness, prefab.name, height, max_iterations, new_leaves))){
                popup.setMsg("Enregistrement réussi");
            } else {
                popup.setMsg("Ce nom est déja pris !\nEchec de l'enregistrement");
            }
            popup.minSize = new Vector2(200, 90);
            popup.maxSize = new Vector2(200, 90);
            popup.Show();
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

    }

    private bool saveConfig(ConfigSpaceColo config){
        var path = Application.dataPath + "/Coral/SpaceColonisation/Config.json";
        LConfigSpaceColo lConfig = new LConfigSpaceColo();
        if(File.Exists(path)){
            lConfig = JsonUtility.FromJson<LConfigSpaceColo>(File.ReadAllText(path));
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

    private void chargeConfig(ConfigSpaceColo config){
        name = config.name;
        leaf_influence_radius = config.leaf_influence_radius;
        leaf_kill_distance = config.leaf_kill_distance;
        influence_points = config.influence_points;
        thickness = config.thickness;
        height = config.height;
        max_iterations = config.max_iterations;
        new_leaves = config.new_leaves;
        prefab = AssetDatabase.LoadAssetAtPath("Assets/Coral/SpaceColonisation/Blender_msh/" + config.prefab + ".fbx", typeof(GameObject)) as GameObject;
    }
    
}
