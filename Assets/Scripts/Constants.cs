using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static int selectedLevel = 0;
    public static bool isFromMainMenu = false;
    public static int modeId = 0;
    public static bool openEnvironmentSelection = false;
    public static string BaseUrl = "http://45.79.126.10:8082";
    static bool isWinEditor = false;
    public static bool IsWindowEditor()
    {
        return Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer;
    }
}
