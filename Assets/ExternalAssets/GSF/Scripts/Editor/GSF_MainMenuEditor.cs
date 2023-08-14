using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GSF_MainMenu))]
public class GSF_MainMenuEditor : Editor {

    string module = "Main Menu";

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

public class ResetSaveData{
	[MenuItem("Window/GSF - Gamerz Studio Framework/Reset Save Data %#r")]
	private static void ResetSave (){				
		Reset ();
	}

	[MenuItem("Window/GSF - Gamerz Studio Framework/Open Save File %#o")]
	private static void OpenSave (){
		Application.OpenURL (Application.persistentDataPath);
	}

	public static void Reset(){
		GSF_SaveLoad.DeleteProgress();
		EditorUtility.DisplayDialog("GSF  - Gamerz Studio Framework",
			"Save data reset successfull !", 
			"Ok");
	}
}
