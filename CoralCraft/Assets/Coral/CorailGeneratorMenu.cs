using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class CorailGeneratorMenu : EditorWindow
{

    public int numCoralType = 0;

    [MenuItem("Coral/CoralGenerator")]
    public static void showWindow() {
        EditorWindow window = GetWindow(typeof(CorailGeneratorMenu));
        window.Show();
    }

    private void OnGUI() {
        string[] coralType = {"None", "Stony coral", "Soft coral"};
        numCoralType = EditorGUILayout.Popup("Pre-Config", numCoralType, coralType);
        switch(numCoralType) {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
        }
    }
}
