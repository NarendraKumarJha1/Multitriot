using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

#if (UNITY_EDITOR)
public class SceneChanger : Editor
{
    [MenuItem("Abhiwan/Open Scenes/Splash Screen")]
    public static void SplashScreen()
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/Scenes/Splash Scene/Splash Screen.unity");
    }
    [MenuItem("Abhiwan/Open Scenes/Main Menu")]
    public static void MainMenu()
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/Scenes/ShoefyScenes/MainMenu.unity");
    }
    [MenuItem("Abhiwan/Open Scenes/Player Selection")]
    public static void PlayerSelection()
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/Scenes/ShoefyScenes/PlayerSelection.unity");
    }
    [MenuItem("Abhiwan/Open Scenes/Game Play")]
    public static void GamePlay()
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/Scenes/ShoefyScenes/GamePlay.unity");
    }
    [MenuItem("Abhiwan/Open Scenes/Maps/Futuristic")]
    public static void Futuristic()
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/Scenes/ShoefyScenes/Futuristic.unity");
    }
    [MenuItem("Abhiwan/Open Scenes/Maps/Shoefy")]
    public static void Shoefy()
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/Scenes/ShoefyScenes/Shoefy.unity");
    }
    [MenuItem("Abhiwan/Open Scenes/Maps/Lava")]
    public static void Lava()
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/Scenes/ShoefyScenes/Lava.unity");
    }
}
#endif
