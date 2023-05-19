using UnityEngine;
using UnityEditor;

public class SavePopup : EditorWindow
{
    private string msg;

    static void Init()
    {
        SavePopup window = ScriptableObject.CreateInstance<SavePopup>();
        window.ShowPopup();
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField(msg, EditorStyles.wordWrappedLabel);
        GUILayout.Space(10);
        if (GUILayout.Button("Ok")) this.Close();
    }

    public void setMsg(string msg){
        this.msg = msg;
    }
}