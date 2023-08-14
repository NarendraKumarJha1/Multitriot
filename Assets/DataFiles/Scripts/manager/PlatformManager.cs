using UnityEngine;
#if UNITY_EDITOR
/*using UnityEditor;
using UnityEditor.Build.Reporting;*/
#endif

#if UNITY_EDITOR
public class PlatformManager :MonoBehaviour
{
   /* private BuildTarget _targetPlatform = BuildTarget.NoTarget;

    [MenuItem("Tools/Switch Platform")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        PlatformManager window = (PlatformManager)EditorWindow.GetWindow(typeof(PlatformManager));
        window.Show();
    }

    private void OnGUI()
    {
        // Display a dropdown list of available build targets
        _targetPlatform = (BuildTarget)EditorGUILayout.EnumPopup("Select Platform", _targetPlatform);

        if (GUILayout.Button("Switch Platform"))
        {
            // Switch the platform and re-import the assets
            if (_targetPlatform == BuildTarget.iOS || _targetPlatform == BuildTarget.Android || _targetPlatform == BuildTarget.StandaloneWindows64)
            {
                SwitchToTargetPlatform(_targetPlatform);
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError("Invalid platform selected. Please select iOS, Android, or StandaloneWindows64.");
            }
        }
    }
#if UNITY_EDITOR
    private static void SwitchToTargetPlatform(BuildTarget targetPlatform)
    {
        if (EditorUserBuildSettings.activeBuildTarget != targetPlatform)
        {
            // Switch the current platform to the selected one
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildPipeline.GetBuildTargetGroup(targetPlatform), targetPlatform);

            // Build a new report and log the results
            BuildReport report = BuildPipeline.BuildPlayer(GetEnabledScenes(), "Temp/Build", targetPlatform, BuildOptions.None);
            BuildSummary summary = report.summary;
            Debug.Log(string.Format("Build succeeded: {0} errors, {1} warnings", summary.totalErrors, summary.totalWarnings));
        }
        else
        {
            Debug.Log("Platform already set to: " + targetPlatform);
        }
    }
#endif
    private static string[] GetEnabledScenes()
    {
        // Get the list of enabled scenes
        string[] scenes = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }
        return scenes;
    }*/
}
#endif
