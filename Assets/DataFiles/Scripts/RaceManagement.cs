using RGSK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class RaceManagement : RaceManagerBehaviour
{
    [SerializeField] private GameObject _opponentPointer;
    [SerializeField] private GameObject _playerPointer;
    [SerializeField] private GameObject _leaderboard;
    List<MMLapStats> list = new List<MMLapStats>();

    [Header("Pause")]
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _menu;
    [SerializeField] private Button _leaderBoardMenu;
    [SerializeField] private Button _close;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _loadingScreen;

    private void Start()
    {
        Debug.Log("Race management Initiated");
        StartCoroutine(SetPointerDelay());
        SetListeners(_pauseButton, _playButton, _menu, _leaderBoardMenu, _close, _pausePanel,_loadingScreen, _leaderboard);
    }

    IEnumerator SetPointerDelay()
    {
        yield return new WaitForSeconds(3f);
        list = FindObjectsOfType<MMLapStats>().ToList();
        SetPointer(_opponentPointer, _playerPointer, list);
    }

    private void Update()
    {
        PlayPause();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause(_pausePanel);
        }
    }

    IEnumerator LoadHomeScreen()
    {

        yield return new WaitForSeconds(1);
    }

    private void PlayPause()
    {
        if(Input.GetKeyDown(KeyCode.P)) {
           
        }
        else if(Input.GetKeyUp(KeyCode.P))
        {
            Play(_pausePanel);
        }
    }
}
