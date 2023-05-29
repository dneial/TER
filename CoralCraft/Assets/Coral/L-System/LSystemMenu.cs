using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;


[System.Serializable]
public struct ConfigLSys {
    public string name;
    public float thickness;
    public float length;
    public int nbIteration;
    public float angle;
    public string grammar;

    public ConfigLSys(float thickness, float length, string name, int nbIteration, float angle, string grammar){
        this.name = name;  
        this.thickness = thickness; 
        this.length = length;
        this.nbIteration = nbIteration;
        this.angle = angle;
        this.grammar = grammar;
    }
}

[System.Serializable]
public class LConfigLSys {
    public List<ConfigLSys> myConfigs = new List<ConfigLSys>();
}

public class LSystemMenu : EditorWindow
{
    Vector2 scrollPos;

    [MenuItem("Coral/L-System")]
    public static void showWindow() {
        EditorWindow window = GetWindow(typeof(LSystemMenu));
        window.Show();
    }

    public Lsystem lsystem;
    public GameObject parent;
    static float thickness = 0.25f;
    static float length = 0.75f;
    static int nbIteration = 3;
    static float angle = 25;

    List<INode> points;


    static int numConfig;
    static string configName = "";
    int numGrammar;

    AnimBool display = new AnimBool(true);
    AnimBool autoFuse = new AnimBool(true);
    AnimBool grammarBool = new AnimBool(true);

    // pour cacher/afficher des actions dans le menu
    private void OnEnable() {
        // display = new AnimBool(true);
        display.valueChanged.AddListener(Repaint);

        // autoFuse = new AnimBool(true);
        autoFuse.valueChanged.AddListener(Repaint);

        // grammarBool = new AnimBool(true);
        grammarBool.valueChanged.AddListener(Repaint);
    }


    void OnGUI()
    {
        //récupérer les fichiers de grammaire
        string[] tmp = Directory.GetFiles(Application.dataPath + "/Coral/L-System/Grammars/", "*.lsys?");

        //trier les fichiers par ordre de leur extension
        string[] files = sortbyExt(tmp);


        int cursor = 0;
        foreach (string path in files){
            files[cursor] = Path.GetFileName(path);
            cursor++;
        }

        //récupérer les Config existantes
        LConfigLSys lConfig = new LConfigLSys();

        if(File.Exists(Application.dataPath + "/Coral/L-System/Config.json")){
            lConfig = JsonUtility.FromJson<LConfigLSys>(File.ReadAllText(Application.dataPath + "/Coral/L-System/Config.json"));
        }

        string[] nameLConfig = new string[lConfig.myConfigs.Count+1];

        nameLConfig[0] = "None";
        cursor = 1;
        foreach (var jConfig in lConfig.myConfigs) {
            nameLConfig[cursor] = jConfig.name;
            cursor++;
        }


        //DEBUT GUI

        EditorGUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        GUILayout.Space(5);

        //Charger Configurations
        numConfig = EditorGUILayout.Popup("Preset", numConfig, nameLConfig);

        if(GUILayout.Button("Charger le preset")){
            if(numConfig > 0){
                chargeConfig(lConfig.myConfigs[numConfig-1], files);
            }
        }

        GUILayout.Space(5);

        //Créer une nouvelle configuration
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Nouveau preset : ");
        configName = EditorGUILayout.TextField(configName);
        EditorGUILayout.EndHorizontal();
        
        //Sauvegarder la configuration
        if(GUILayout.Button("Sauvegarder le preset")){
            SavePopup popup = ScriptableObject.CreateInstance<SavePopup>();
            if(configName == ""){
                popup.setMsg("Nom de preset vide !\nEchec de l'enregistrement");
            } else if (saveConfig(new ConfigLSys(thickness, length, configName, nbIteration, angle, files[numGrammar]))){
                popup.setMsg("Enregistrement réussi");
            } else {
                popup.setMsg("Ce nom est déja pris !\nEchec de l'enregistrement");
            }
            popup.minSize = new Vector2(200, 90);
            popup.maxSize = new Vector2(200, 90);
            popup.Show();
        }

        DrawUILine(Color.black);

        //Choix de la grammaire
        numGrammar = EditorGUILayout.Popup("Grammaire", numGrammar, files);

        if (files[numGrammar].EndsWith(".lsys"))
        {
            grammarBool.target = true;
        }
        else grammarBool.target = false;

        //Paramètres grammaire
        if(EditorGUILayout.BeginFadeGroup(grammarBool.faded)){
            
            EditorGUI.indentLevel++;
            
            //GUILayout.Label("Thickness");
            thickness = EditorGUILayout.Slider("Épaisseur", thickness, 0, 1);

            //GUILayout.Label("Length");
            length = EditorGUILayout.Slider("Longueur", length, 0, 1);

            //GUILayout.Label("angle");
            angle = EditorGUILayout.Slider("Angle", angle, 0, 360);

            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();

        //Paramètres Nombre d'itérations
        GUILayout.Label("nbIteration");
        nbIteration = EditorGUILayout.IntSlider(nbIteration, 1, 10);

        // side to side toggle buttons
        GUILayout.BeginHorizontal();
        
        GUILayout.Label("Affichage Auto");
        display.target = EditorGUILayout.Toggle(display.target);

        GUILayout.Label("Fusion Auto");
        autoFuse.target = EditorGUILayout.Toggle(autoFuse.target);
    
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();       

        if(GUILayout.Button("Générer")){
            parent = new GameObject();
            points = new List<INode>();

            lsystem = LsystemInterpretor.ParseFile(Application.dataPath + "/Coral/L-System/Grammars/" + files[numGrammar]);
            lsystem.Generate(nbIteration);

            //if extension is .lsys
            if (files[numGrammar].EndsWith(".lsys"))
            {
                grammarBool.target = false;
                //traduction de la gammaire lsystemV1 en lsystemV2
                lsystem.trad(thickness, length, angle);
            }
            else grammarBool.target = true;
        
            LSystemGen generator = new LSystemGen(lsystem, parent);
            
            points = generator.ParseAndPlace(lsystem.current, display.target);
   
            if (display.target && autoFuse.target && parent != null)
            {
                MeshCombiner combiner = new MeshCombiner(parent, files[numGrammar]);
                combiner.combineMeshes();
                parent = combiner.getCombinedMesh().gameObject;
            } 
        }

        //DrawUILine(Color.grey);
       
        //Affichage de a structure
        if(EditorGUILayout.BeginFadeGroup(1 - display.faded)){
            if(GUILayout.Button("Afficher Corail")){    
                if (parent == null || points == null || points.Count == 0 || lsystem == null)
                {
                    Debug.Log("Aucun corail à afficher");
                }
                else
                {
                    LSystemGen generator = new LSystemGen(lsystem, parent);
                    foreach (INode point in points){
                        generator.displayBranch((Branche) point, parent);
                    }
                    //vider la liste des points pour ne pas afficher les branches plusieurs fois
                    points.Clear();

                    if (autoFuse.target)
                    {
                        MeshCombiner combiner = new MeshCombiner(parent, files[numGrammar]);
                        combiner.combineMeshes();
                        parent = combiner.getCombinedMesh().gameObject;
                    }
                
                }  
            }
        }
        EditorGUILayout.EndFadeGroup();

        //combiner les meshes
        if(EditorGUILayout.BeginFadeGroup(1 - autoFuse.faded))
        {
            if(GUILayout.Button("Fusionner Objets")){
                if (parent == null)
                {
                    Debug.Log("Aucun objet à fusionner");
                }
                else if (parent.transform.childCount == 0)
                {
                    Debug.Log("veuillez d'abord afficher le corail généré");
                }
                else
                {
                    MeshCombiner combiner = new MeshCombiner(parent, files[numGrammar]);
                    combiner.combineMeshes();
                }
            }
        }
        EditorGUILayout.EndFadeGroup();


        DrawUILine(Color.black);

        if(GUILayout.Button("Étape par étape"))
        {
            points = new List<INode>();

            lsystem = LsystemInterpretor.ParseFile(Application.dataPath + "/Coral/L-System/Grammars/" + files[numGrammar]);
            lsystem.Generate(nbIteration);

            //if extension is .lsys
            if (files[numGrammar].EndsWith(".lsys"))
            {
                grammarBool.target = false;
                //traduction de la gammaire lsystemV1 en lsystemV2
                lsystem.trad(thickness, length, angle);
            }
            else grammarBool.target = true;

            //on a la chaine de caractères
            
            Demo = new LSystemDemo(lsystem);

            Demo.initDemo();
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();   
    }


    private bool saveConfig(ConfigLSys config){
        var path = Application.dataPath + "/Coral/L-System/Config.json";
        LConfigLSys lConfig = new LConfigLSys();
        if(File.Exists(path)){
            lConfig = JsonUtility.FromJson<LConfigLSys>(File.ReadAllText(path));
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

    private void chargeConfig(ConfigLSys config, string[] files){
        configName = config.name;
        // thickness = config.thickness;
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

    public static void DrawUILine(Color color, int thickness = 1, int padding = 15)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
        r.height = thickness;
        r.y+=padding/2;
        r.x-=2;
        r.width +=6;
        EditorGUI.DrawRect(r, color);
    }

}