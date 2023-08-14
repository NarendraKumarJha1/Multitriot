using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class UiController : MonoBehaviourPunCallbacks
{

    public GameObject missileLauncher = null;

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        PhotonManager.HostQuit = true;
        GoToMainMenu();
        base.OnMasterClientSwitched(newMasterClient);
    }

    public void SetMissileUi(int missileCount, bool isCarState)
    {
        //Debug.Log("missile count is : " + missileCount);
        missileLauncher.SetActive(missileCount > 0 && isCarState);
    }

    public Image conversionImage = null;
    public Sprite carSprite = null, robotSprite = null;
    public void SetConversionSprite(bool isCar, int amount)
    {
        conversionImage.sprite = isCar ? robotSprite : carSprite;
        conversionImage.gameObject.SetActive(amount > 0);
    }
    public Image carFillerImage = null;
    public void BackToCarFiller(float currentTime, float totalTime)
    {
        if (conversionImage.GetComponent<Button>())
            conversionImage.GetComponent<Button>().interactable = (totalTime - currentTime) / totalTime <= 0;
        carFillerImage.gameObject.SetActive(true);//((totalTime / currentTime) < 1);
        carFillerImage.fillAmount = (totalTime - currentTime)/totalTime;
    }

    public GameObject pausePanel = null;
    public void PasuePress()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0;
    }
    public void ResumePress()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
    }
    public string mainMenuSceneName = "MainMenu";
    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        //Photon.Pun.PhotonNetwork.Disconnect();
        Photon.Pun.PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(mainMenuSceneName);
        Resources.UnloadUnusedAssets();
        //Photon.Pun.PhotonNetwork.Disconnect();
    }
    public void Replay()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("GamePlay");
        Resources.UnloadUnusedAssets();
    }
    public void GoToPlayerSelection()
    {
        Constants.openEnvironmentSelection = true;
        SceneManager.LoadScene("PlayerSelection");
        Resources.UnloadUnusedAssets();
    }

    public Text winningCoinsText = null;
    public void ShowWinningCoins(int coins)
    {
        winningCoinsText.text = "" + coins;
    }
}
