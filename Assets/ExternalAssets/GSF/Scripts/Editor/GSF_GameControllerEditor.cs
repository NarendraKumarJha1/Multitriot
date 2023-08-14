using UnityEditor;

[CustomEditor(typeof(GSF_GameController))]
public class GSF_GameControllerEditor : Editor {

    string module = "Game Controller";    void Awake() {        GSF_Editor.GetLogo();    }    public override void OnInspectorGUI() {        GSF_Editor.DefineGUIStyle(module);        EditorGUILayout.BeginVertical("box");        DrawDefaultInspector();        EditorGUILayout.EndHorizontal();
    }
}
