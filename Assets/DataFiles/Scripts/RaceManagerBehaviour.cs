using RGSK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RaceManagerBehaviour : MonoBehaviour
{
    public void SetPointer(GameObject _opponentPointer, GameObject _playerPointer, List<MMLapStats> _list)
    {
        foreach (var item in _list)
        {
            if (item.gameObject.CompareTag("Player"))
            {
                Debug.Log("Player Found " + item);
                GameObject _g = Instantiate(_playerPointer);
                _g.GetComponent<RacerPointer>().target = item.gameObject.transform;
            }
            else
            {
                Debug.Log("Enemy Found " + item);
                GameObject _g = Instantiate(_opponentPointer);
                _g.GetComponent<RacerPointer>().target = item.gameObject.transform;
            }

        }
    }
    public void SetListeners(Button _pauseButton, Button _playButton, Button _menu, Button _leaderboardMenu, Button _close, GameObject _panel, GameObject _loadingScreen, GameObject _leaderboard)
    {
        _pauseButton.onClick.AddListener(() => Pause(_panel));
        _playButton.onClick.AddListener(() => Play(_panel));
        _menu.onClick.AddListener(() => Menu(_panel,_loadingScreen));
        _close.onClick.AddListener(() => Close());
        _leaderboardMenu.onClick.AddListener(() => Menu(_panel, _loadingScreen,_leaderboard));
    }

    private void Close()
    {
        Application.Quit();
    }

    public void Play(GameObject _pausePanel)
    {
        Debug.Log("Play Pressed");
        Time.timeScale = 1; // Pause game
        _pausePanel.SetActive(false);
    }    
    public void Menu(GameObject _pausePanel, GameObject _loadingScreen, GameObject _leaderboard = null)
    {
        Debug.Log("Play Pressed");
        if (_leaderboard != null)
        {
            _leaderboard.SetActive(false);
        }
        Time.timeScale = 1; // Pause game
        StartCoroutine(LoadMenu(_loadingScreen));
        _pausePanel.SetActive(false);
    }

    IEnumerator LoadMenu(GameObject _loadingScreen)
    {
        _loadingScreen.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(1);
    }

    public void Pause(GameObject _pausePanel)
    {
        Debug.Log("Pause Pressed");
        Time.timeScale = 0; // Pause game
        _pausePanel.SetActive(true);
    }
}
