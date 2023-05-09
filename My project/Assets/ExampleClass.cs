// EditorScript that quickly searches for a help page
// about the selected Object.
//
// If no such page is found in the Manual it opens the Unity forum.

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;

public class ExampleClass : EditorWindow
{
    public Object source;

    [MenuItem("Example/ObjectField Example _h")]
    static void Init()
    {
        var window = GetWindowWithRect<ExampleClass>(new Rect(0, 0, 165, 100));
        window.Show();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        source = EditorGUILayout.ObjectField(source, typeof(Object), true);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Search!"))
        {
            if (source == null)
                ShowNotification(new GUIContent("No object selected for searching"));
            else if (Help.HasHelpForObject(source))
                Help.ShowHelpForObject(source);
            else
                Help.BrowseURL("https://forum.unity3d.com/search.php");
        }
    }
}