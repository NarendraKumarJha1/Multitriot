using UnityEditor;

[CustomEditor(typeof(GSF_SplashScreen))]
public class GSF_SplashScreenEditor : Editor {

	string module = "Splash Screen";

	void Awake() {
		GSF_Editor.GetLogo();
	}

	public override void OnInspectorGUI() {

		GSF_Editor.DefineGUIStyle(module);

		EditorGUILayout.BeginVertical("box");
		DrawDefaultInspector();
		EditorGUILayout.EndHorizontal();
	}
}
