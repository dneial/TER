using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;


public class SpaceColonizationMenu : EditorWindow
{

    [MenuItem("GameObject/Space Colonization")]
    public static void showWindow() {
        EditorWindow window = GetWindow(typeof(SpaceColonizationMenu));
        window.Show();
    }



    static float leaf_influence_radius = 1f;
    static float leaf_kill_distance = 1f;
    static int influence_points = 100;
    static AnimationCurve thickness = AnimationCurve.EaseInOut(0,0.35f,1,1);

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

        thickness.AddKey(0.4f, 0.9f);
        thickness.AddKey(0.3f, 0.8f);
        thickness.AddKey(0.95f, 1);
        thickness.SmoothTangents(3, -1);
        thickness = EditorGUILayout.CurveField("Thickness", thickness);

      
        if(GUILayout.Button("Generate"))
        {
            generator = new SpaceColonization(leaf_kill_distance, leaf_influence_radius, influence_points);
            view = new SpaceColonizationView();

            generator.Generate();
            
            this.view.update(this.generator.GetNodes(), thickness);
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
