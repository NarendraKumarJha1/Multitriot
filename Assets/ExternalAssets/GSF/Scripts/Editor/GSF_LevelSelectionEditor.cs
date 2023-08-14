using UnityEditor;

[CustomEditor(typeof(GSF_LevelSelection))]
public class GSF_LevelSelectionEditor : Editor {

    string module = "Level Selection";    void Awake() {        GSF_Editor.GetLogo();    }    public override void OnInspectorGUI() {        GSF_Editor.DefineGUIStyle(module);        EditorGUILayout.BeginVertical("box");        DrawDefaultInspector();        EditorGUILayout.EndHorizontal();
    }
}
