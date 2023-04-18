// using System.IO;
// using UnityEditor;
// using UnityEngine;

// public class Config {
//     public string name;
//     public float thickness;
//     public int heigh;

//     public Config(float thickness, int heigh, string name){
//         this.name = name;
//         this.thickness = thickness;
//         this.heigh = heigh;
//     }
// }


// public class LetterGeneratorWindow : EditorWindow {

//     static float thickness = 1.0f;
//     static int heigh = 1;

//     [MenuItem("GameObject/training/LetterGenerator")]
//     public static void showWindow() {
//         EditorWindow window = GetWindow(typeof(LetterGeneratorWindow));
//         window.Show();
//     }

//     void OnGUI()
//     {
//         GUILayout.Label("Thickness");
//         thickness = EditorGUILayout.Slider(thickness, 0, 1);

//         GUILayout.Label("Height");
//         heigh = EditorGUILayout.IntSlider(heigh, 1, 10);



//         if(GUILayout.Button("Generate")){
//             saveConfig(new Config(thickness, heigh, "Yo"));

//             GameObject YL1 = GameObject.CreatePrimitive(PrimitiveType.Capsule);
//             YL1.name = "YL1";
//             GameObject YL2 = GameObject.CreatePrimitive(PrimitiveType.Capsule);
//             YL2.name = "YL2";
//             GameObject YL3 = GameObject.CreatePrimitive(PrimitiveType.Capsule);
//             YL3.name = "YL3";

//             Vector3 vYL1 = YL1.transform.localPosition;

//             YL1.transform.localScale = new Vector3(thickness*heigh, heigh, thickness*heigh);

//             YL2.transform.localScale = new Vector3(thickness*heigh*0.8f, heigh*0.8f, thickness*heigh*0.8f);
//             YL2.transform.localPosition = new Vector3(vYL1.x, vYL1.y+((float)heigh + YL2.transform.localScale.y)*(1+(1-thickness))/2, vYL1.z);
//             YL2.transform.RotateAround(YL2.transform.localPosition - new Vector3(0, YL2.transform.localScale.y*(1+(1-thickness))/2, 0), new Vector3(1,0,0), 40);
            
//             YL3.transform.localScale = new Vector3(thickness*heigh*0.8f, heigh*0.8f, thickness*heigh*0.8f);
//             YL3.transform.localPosition = new Vector3(vYL1.x, vYL1.y+((float)heigh + YL3.transform.localScale.y)*(1+(1-thickness))/2, vYL1.z);
//             YL3.transform.RotateAround(YL3.transform.localPosition - new Vector3(0, YL3.transform.localScale.y*(1+(1-thickness))/2, 0), new Vector3(1,0,0), -40);

//             Undo.RegisterCreatedObjectUndo(YL1, "Create " + YL1.name);
//             Undo.RegisterCreatedObjectUndo(YL2, "Create " + YL2.name);
//             Undo.RegisterCreatedObjectUndo(YL3, "Create " + YL3.name);
//         }

//     }

    
//     private void saveConfig(Config config){
//         string fileName = Application.dataPath + "/ConfigLetter.json";
//         if(File.Exists(fileName)){
//             string jsonString = ",\n" + JsonUtility.ToJson(config);
//             File.AppendAllText(fileName, jsonString);
//         } else {
//             string jsonString = "[" + JsonUtility.ToJson(config);
//             File.AppendAllText(fileName, jsonString);
//         }
    
//     }
// }
