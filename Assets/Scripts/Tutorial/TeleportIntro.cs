using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportIntro : MonoBehaviour {
    public GameObject teleportIntroUi;

    void Start () {
        DisablePlayerSound (true);
        Time.timeScale = 0;
        teleportIntroUi.SetActive (true);
    }

    public void DisableTeleportIntro () {
        DisablePlayerSound (false);
        Time.timeScale = 1;
        teleportIntroUi.SetActive (false);
    }

    public void DisablePlayerSound (bool disable = true) {
        foreach (AudioSource audio in RCC_SceneManager.Instance.activePlayerVehicle.GetComponentsInChildren<AudioSource> ()) {
            audio.enabled = !disable;
        }
    }

}