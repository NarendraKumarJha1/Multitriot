using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using System.Globalization;

public class GSF_SplashScreen : MonoBehaviour
{

    [Header("Scene Selection")]
    public Scenes NextScene;

    [Header("Scene Settings")]
    public float WaitTime;

    void Start()
    {
        //DateTime dt = DateTime.Now;

        //dt = dt.AddMinutes(2);
        //string dd = dt.ToString("MM/dd/yyyy h:mm:ss tt");



        //Debug.LogError(dd.ToString());
        //DateTime timme =  DateTime.ParseExact(dd.ToString(), "MM/dd/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

        //const string dateFormat = "ddd MMM dd HH\':\'mm\':\'ss \'GMT\'K yyyy";

        //string nowString = DateTime.Now.ToString(dateFormat, CultureInfo.InvariantCulture);
        //DateTime dateTime = DateTime.ParseExact(nowString, dateFormat, CultureInfo.InvariantCulture);

        //Debug.LogError(dateTime.ToString());

        Time.timeScale = 1;
        AudioListener.pause = false;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        Application.targetFrameRate = 120;
        if (!GameManager.Instance.Initialized)
        {
            InitializeGame();
        }
        StartCoroutine(StartGame());
        UnityEngine.Analytics.AnalyticsEvent.FirstInteraction();
    }

    void InitializeGame()
    {
        SaveData.Instance = new SaveData();
        GSF_SaveLoad.LoadProgress();
        GameManager.Instance.Initialized = true;
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(WaitTime);

        SceneManager.LoadSceneAsync(NextScene.ToString());
    }
}