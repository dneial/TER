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
                           int influence_points, AnimationCurve thickness, string prefab, 
                           float height, int max_iterations, int new_leaves)
    {
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
    static GameObject prefab = null;
    static float height = 10f;
    static float scale = 1f;
    static int max_iterations = 1000;
    static int new_leaves = 5;
    static int numConfig;
    static string configName = "";


    private SpaceColonization generator;
    private SpaceColonizationView view;
    private GameObject root = null;
    private GameObject vol = null;


    void OnGUI()
    {
        if(root == null){
            root = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            root.name = "Space Colonization Root";
        }


        //récupération des config existantes
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

        //DEBUT GUI

        EditorGUILayout.BeginVertical();
        scrollPos =  EditorGUILayout.BeginScrollView(scrollPos);

        GUILayout.Space(5);

        //charger config
        numConfig = EditorGUILayout.Popup("Preset", numConfig, nameLConfig);

        if(GUILayout.Button("Charger le preset")){
            if(numConfig > 0){
                chargeConfig(lConfig.myConfigs[numConfig-1]);
            }
        }
        
        GUILayout.Space(5);

        //Créer une nouvelle configuration
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Nouveau preset");
        configName = EditorGUILayout.TextField(configName);
        EditorGUILayout.EndHorizontal();

        //sauvegarder config
        if(GUILayout.Button("Sauvegarder le preset")){
            SavePopup popup = ScriptableObject.CreateInstance<SavePopup>();
            if(prefab == null){
                prefab = AssetDatabase.LoadAssetAtPath("Assets/Coral/SpaceColonisation/Blender_msh/AsimBox.fbx", typeof(GameObject)) as GameObject;
            }
            if(configName == ""){
                popup.setMsg("Nom de preset vide !\nEchec de l'enregistrement");
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

        LSystemMenu.DrawUILine(Color.black);
        
        GUILayout.Label("Paramètres du nuage de points", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUI.indentLevel++;

        GameObject setPrefab = EditorGUILayout.ObjectField("Forme du volume", prefab, typeof(GameObject), true) as GameObject;
        if(setPrefab != prefab || this.vol == null){
            prefab = setPrefab;
            if(prefab != null && this.vol == null) {
                prefab = AssetDatabase.LoadAssetAtPath("Assets/Coral/SpaceColonisation/Blender_msh/AsimBox.fbx", typeof(GameObject)) as GameObject;
                this.vol = Instantiate(prefab, new Vector3(0, height, 0), Quaternion.identity);
                if(this.vol.GetComponent<MeshCollider>() == null)
                {
                    // Debug.Log("No mesh collider found, adding one");
                    this.vol.AddComponent<MeshCollider>();
                }
                Debug.Log(this.vol);
            }
        }
        

        float newHeight = EditorGUILayout.Slider("Hauteur du volume", height, 0.1f, 20f);

        if(this.vol != null && newHeight != height){
            height = newHeight;
            this.vol.transform.position = new Vector3(0, height, 0);
        }

        float newScale = EditorGUILayout.Slider("Echelle du volume", scale, 0.5f, 1.5f);
        if(scale != newScale && this.vol != null){
            Debug.Log("scale: " + this.vol.transform.localScale + ' ' + newScale + ' ' + this.vol.transform.localScale * newScale);
            scale = newScale;
            this.vol.transform.localScale = Vector3.one * scale;
        }

        EditorGUI.indentLevel--;

        LSystemMenu.DrawUILine(Color.black);

        GUILayout.Label("Paramètres de Space Colonization", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUI.indentLevel++;

        GUIContent contentInf = new GUIContent("Points d'influence", "Nombre de points d'influence de départ");
        GUILayout.Label(contentInf);
        influence_points = EditorGUILayout.IntSlider(influence_points, 50, 1000);

        GUIContent contentRay = new GUIContent("Rayon d'influence", "Rayon d'action des points d'influence");
        GUILayout.Label(contentRay);
        leaf_influence_radius = EditorGUILayout.Slider(leaf_influence_radius, 10, 100);

        GUIContent contentLim = new GUIContent("Distance limite", "Distance à partir de laquelle les points d'influence sont détruits");
        GUILayout.Label(contentLim);
        leaf_kill_distance = EditorGUILayout.Slider(leaf_kill_distance, 1, 10);

        GUIContent contentMax = new GUIContent("Nombre d'itérations maximales", "limite du nombre d'itérations de l'algorithme");
        GUILayout.Label(contentMax);
        max_iterations = EditorGUILayout.IntSlider(max_iterations, 1, 1000);

        GUIContent contentNew = new GUIContent("Nouveaux points", "Nombre de points d'influence à rajouter à chaque iteration");
        GUILayout.Label(contentNew);
        new_leaves = EditorGUILayout.IntSlider(new_leaves, 0, 100);

        EditorGUILayout.Space();

        thickness.AddKey(0.4f, 0.9f);
        thickness.AddKey(0.3f, 0.8f);
        thickness.AddKey(0.95f, 1);
        thickness.SmoothTangents(3, -1);
        GUIContent contentThic = new GUIContent("Épaisseur", "Courbe d'évolution de l'épaisseur des segments");
        thickness = EditorGUILayout.CurveField(contentThic, thickness);
        
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();
      
        if(GUILayout.Button("Générer"))
        {
            Bounds bounds = this.vol.GetComponent<MeshCollider>().bounds;
            Debug.Log("Bounds : " + bounds.ToString());
            
            generator = new SpaceColonization(bounds, leaf_kill_distance, leaf_influence_radius, influence_points);
            view = new SpaceColonizationView();


            generator.Generate(max_iterations, new_leaves);

            DestroyImmediate(this.vol);
            scale = 1f;

            this.view.update(this.generator.GetNodes(), thickness);
            
            Debug.Log("Génération après " + generator.steps + " étapes");

            if (this.view.GetRoot() == null)
            {
                Debug.Log("Aucun objet à fusionner");
            }
            else
            {
                MeshCombiner combiner = new MeshCombiner(this.view.GetRoot());
                combiner.combineMeshes();
            }
            
        }

        // if(GUILayout.Button("Save configuration")){
        //     SavePopup popup = ScriptableObject.CreateInstance<SavePopup>();
        //     if(prefab == null){
        //         prefab = AssetDatabase.LoadAssetAtPath("Assets/Coral/SpaceColonisation/Blender_msh/AsimBox.fbx", typeof(GameObject)) as GameObject;
        //     }
        //     if(configName == ""){
        //         popup.setMsg("Nom de configuration vide !\nEchec de l'enregistrement");
        //     } else if (saveConfig(new ConfigSpaceColo(configName, leaf_influence_radius, leaf_kill_distance,
        //     influence_points, thickness, prefab.name, height, max_iterations, new_leaves))){
        //         popup.setMsg("Enregistrement réussi");
        //     } else {
        //         popup.setMsg("Ce nom est déja pris !\nEchec de l'enregistrement");
        //     }
        //     popup.minSize = new Vector2(200, 90);
        //     popup.maxSize = new Vector2(200, 90);
        //     popup.Show();
        // }

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
