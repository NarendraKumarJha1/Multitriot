using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Helper
{
    public static string UnlockAllLevels_str = "UnlockAllLevels";
    public static string UnlockAllPlayer_str = "UnlockAllPlayers";
    public static string UnlockAllEverything_str = "UnlockEverything";

    public static float FindAngleInDgrees(Vector3 secondVector, Vector3 firstVector)
    {
        Vector2 dir = secondVector - firstVector;
        float Angle = Mathf.Atan2(dir.y, dir.x);
        float AngleInDegrees = Angle * Mathf.Rad2Deg;

        if (AngleInDegrees < 0)
        {
            AngleInDegrees = AngleInDegrees + 360f;
        }

        return AngleInDegrees;
    }

    public static GameObject FindChildByName(GameObject parent, string name)
    {
        Transform[] trs = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trs)
        {
            if (t.name == name)
            {
                return t.gameObject;
            }
        }
        return null;
    }

    public static GameObject FindChildByTag(GameObject parent, string tag)
    {
        Transform[] trs = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trs)
        {
            if (t.CompareTag(tag))
            {
                return t.gameObject;
            }
        }
        return null;
    }

    public static void ShowMessage(UnityEngine.UI.Text messageUi, string msg, Color textClr, float stayDuration = 1f)
    {
        messageUi.text = msg;
        messageUi.gameObject.SetActive(true);
        messageUi.color = new Color(textClr.r, textClr.g, textClr.b, 0f);
        messageUi.DOFade(1f, .3f).OnComplete(() =>
        {
            messageUi.DOFade(0f, .3f).SetDelay(stayDuration).OnComplete(() =>
            {
                messageUi.gameObject.SetActive(false);
            });
        });
    }

    public static void StartRealtimeCountdown(MonoBehaviour instance, DateTime countdownStartTime, float totalCountdownDurationInMinutes, UnityEngine.UI.Text countdownUi, System.Action onCountdownEnd = null)
    {
        instance.StartCoroutine(RealtimeCountdown(countdownStartTime, totalCountdownDurationInMinutes, countdownUi, onCountdownEnd));
    }

    private static IEnumerator RealtimeCountdown(DateTime countdownStartTime, float totalCountdownDurationInMinutes, UnityEngine.UI.Text countdownUi, System.Action onCountdownEnd = null)
    {
        //DateTime inventoryAddTime = DateTime.Parse (invSlot.weaponAddDateTime);
        TimeSpan elapsedTime = DateTime.Now.Subtract(countdownStartTime);

        TimeSpan oneSec = new TimeSpan(TimeSpan.TicksPerSecond);
        TimeSpan remainingTime = new TimeSpan((long)totalCountdownDurationInMinutes * 60 * TimeSpan.TicksPerSecond) - elapsedTime;
        //DateTime oneSec = inventoryAddTime;

        Debug.Log((int)remainingTime.TotalSeconds + " total seconds...");

        while ((int)remainingTime.TotalSeconds > 0)
        {
            remainingTime = remainingTime - oneSec;
            //Debug.Log (oneSec + " Decrementing second..." + remainingTime);
            countdownUi.text = remainingTime.Hours + ":" + remainingTime.Minutes + ":" + remainingTime.Seconds;

            yield return new WaitForSecondsRealtime(1);
        }

        if (onCountdownEnd != null)
            onCountdownEnd.Invoke();
    }

    public static void UnlockEverything()
    {
        //unlock all levels
        SaveData.Instance.Level = GameManager.totalLevelsCount;
        // unlock all players
        for (int i = 0; i < SaveData.Instance.players.Length; i++)
        {
            SaveData.Instance.players[i].Locked = false;
        }

        //remove ads
        SaveData.Instance.RemoveAds = true;

        GSF_SaveLoad.SaveProgress();

        PlayerPrefs.SetInt(UnlockAllEverything_str, 1);
        PlayerPrefs.Save();
    }

    public static void UnlockAllLevels()
    {
        SaveData.Instance.Level = GameManager.totalLevelsCount;
        GSF_SaveLoad.SaveProgress();
        PlayerPrefs.SetInt(UnlockAllLevels_str, 1);
        PlayerPrefs.Save();
    }

    public static void UnlockAllPlayers()
    {
        for (int i = 0; i < SaveData.Instance.players.Length; i++)
        {
            SaveData.Instance.players[i].Locked = false;
        }
        GSF_SaveLoad.SaveProgress();
        PlayerPrefs.SetInt(UnlockAllPlayer_str, 1);
        PlayerPrefs.Save();
    }

    public static void RemoveAds()
    {
        SaveData.Instance.RemoveAds = true;
        GSF_SaveLoad.SaveProgress();

    }

}