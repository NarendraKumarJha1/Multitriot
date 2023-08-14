using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using RGSK;

[CustomEditor(typeof(RaceUI))]
public class RaceUI_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        Texture logo = (Texture)Resources.Load("EditorUI/RGSKLogo");
        GUILayout.Label(logo, GUILayout.Height(50));

        DrawDefaultInspector();
    }
}
