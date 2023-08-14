using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : UIManagerParent
{
    [Header("Buttons")]
    [SerializeField] private Button _lavaMode;
    [SerializeField] private Button _cityRace;
    [SerializeField] private Button _beachMode;
    [SerializeField] private Button _garage;
    [SerializeField] private Button _homeO;
    [SerializeField] private Button _homeT;
    [SerializeField] private Button _leaderBoard;
    [SerializeField] private Button _close;

    [Header("Panels")]
    [SerializeField] private GameObject _mainMenupanel;
    [SerializeField] private GameObject _garagePanel;
    [SerializeField] public GameObject _leaderBoardPanel;
    [SerializeField] public GameObject _message;

    public GameObject _loadingScreen;
    public Image _loadingProgressBar;
    List<AsyncOperation> _scenesToLoad = new List<AsyncOperation>();

    public bool _enableBeachMap = false;
    private void Start()
    {
        SetLisForSceneLoad(_lavaMode, _mainMenupanel, _loadingScreen, _loadingProgressBar, _scenesToLoad, true, 2);
        if (_enableBeachMap)
        {
            SetLisForSceneLoad(_beachMode, _mainMenupanel, _loadingScreen, _loadingProgressBar, _scenesToLoad, true, 3);
        }
        SetLisForSceneLoad(_cityRace, _mainMenupanel, _loadingScreen, _loadingProgressBar, _scenesToLoad, true, 4);

        SetLoadingRef(_loadingScreen);
        SetMessageRef(_message);
        SetListener();
    }


    #region

  

    #endregion




    private void SetListener()
    {
        _garage.onClick.AddListener(LoadGarage);
        _close.onClick.AddListener(() => Close());
        _homeT.onClick.AddListener(LoadMainMenuFromLeaderBoard);
        _leaderBoard.onClick.AddListener(LoadLeaderBoard);
    }

    private void Close()
    {
        Application.Quit();
    }

    private void LoadLeaderBoard()
    {
        InitiateLoad(1f);
        Toggle(_mainMenupanel, false, 1.1f);
        Toggle(_leaderBoardPanel, true, 0.9f);
    }

    
    private void LoadMainMenuFromLeaderBoard()
    {
        InitiateLoad(1f);
        Toggle(_mainMenupanel, true, 0.9f);
        Toggle(_leaderBoardPanel, false, 1f);
    }

    private void LoadGarage()
    {
        InitiateLoad(1f);
        Toggle(_mainMenupanel,false,1.1f);
        Toggle(_garagePanel,true,1f);
    }
}
