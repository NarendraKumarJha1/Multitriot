using UnityEngine;
using UnityEngine.UI;

public static class ButtonClickExt
{
    public static void OnClick(this Button button, System.Action action)
    {
        if (button == null)
        {
            Debug.LogError("button reference missing");
            return;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(delegate { action?.Invoke(); });
    }
}