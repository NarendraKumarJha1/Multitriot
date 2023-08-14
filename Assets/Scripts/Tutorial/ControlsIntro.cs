using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsIntro : MonoBehaviour {
    public GameObject controlsIntroUi;
    IEnumerator Start () {
        yield return new WaitForSecondsRealtime (0.5f);
        // DisablePlayerSound (true);
        Time.timeScale = 0;
        controlsIntroUi.SetActive (true);
    }
    public void DisableControlsIntro () {
        // DisablePlayerSound (false);
        Time.timeScale = 1;
        controlsIntroUi.SetActive (false);
    }

    public void DisablePlayerSound (bool disable = true) {
        foreach (AudioSource audio in RCC_SceneManager.Instance.activePlayerVehicle.GetComponentsInChildren<AudioSource> ()) {
            audio.enabled = !disable;
        }
    }
}