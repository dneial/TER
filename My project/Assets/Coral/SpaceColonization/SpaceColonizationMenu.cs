using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;


public class SpaceColonizationMenu : EditorWindow
{

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

    private SpaceColonization generator;
    private SpaceColonizationView view;


    void OnGUI()
    {
        GUILayout.Label("Config Name");
        name = EditorGUILayout.TextField("");

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

        GUIContent content = new GUIContent("New Leaves", "Number of new leaves to add each iteration");
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
                prefab = AssetDatabase.LoadAssetAtPath("Assets/Blender_msh/AsimBox.fbx", typeof(GameObject)) as GameObject;
            }

            GameObject go = Instantiate(prefab, new Vector3(0, height, 0), Quaternion.identity);
            Bounds bounds = go.GetComponent<MeshCollider>().bounds;
            
            generator = new SpaceColonization(bounds, leaf_kill_distance, leaf_influence_radius, influence_points);
            view = new SpaceColonizationView();

            DestroyImmediate(go);

            generator.Generate(max_iterations);
            this.view.update(this.generator.GetNodes(), thickness);
            
            Debug.Log("Generated after " + generator.steps + " steps");
            
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
    
}
