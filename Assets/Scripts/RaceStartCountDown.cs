using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceStartCountDown : MonoBehaviour {
    public Renderer _renderer;
    public Light redLight;
    public int redLightMaterialIndex;
    public Light yellowLight;
    public int yellowLightMaterialIndex;
    public Light greenLight;
    public int greenLightMaterialIndex;

    public Text countDownText;
    public AudioClip threeSfx, twoSfx, oneSfx, goSfx;
    AudioSource thisAudioSource;

    void Start () {
        if (!GetComponent<AudioSource> ()) {
            this.gameObject.AddComponent<AudioSource> ();
            thisAudioSource = GetComponent<AudioSource> ();
            thisAudioSource.loop = false;
            thisAudioSource.playOnAwake = false;
        } else {
            thisAudioSource = GetComponent<AudioSource> ();
        }

        countDownText.text = "";

        StartCoroutine (Start_Coroutine ());
    }

    public IEnumerator Start_Coroutine () {
        yield return new WaitForSeconds (1);
        redLight.gameObject.SetActive (true);
        _renderer.materials[redLightMaterialIndex].SetColor ("_EmissionColor", Color.red);
        countDownText.text = "3";
        if (thisAudioSource) thisAudioSource.PlayOneShot (threeSfx);

        yield return new WaitForSeconds (1);
        yellowLight.gameObject.SetActive (true);
        _renderer.materials[yellowLightMaterialIndex].SetColor ("_EmissionColor", Color.yellow);
        countDownText.text = "2";
        if (thisAudioSource) thisAudioSource.PlayOneShot (twoSfx);

        yield return new WaitForSeconds (1);
        greenLight.gameObject.SetActive (true);
        _renderer.materials[greenLightMaterialIndex].SetColor ("_EmissionColor", Color.green);
        countDownText.text = "1";
        if (thisAudioSource) thisAudioSource.PlayOneShot (oneSfx);

        yield return new WaitForSeconds (1);

        countDownText.text = "GO!";
        if (thisAudioSource) thisAudioSource.PlayOneShot (oneSfx);

        Invoke ("DisableLights", 5f);

        RCC_SceneManager.Instance.activePlayerVehicle.canControl = true;

        yield return new WaitForSeconds (1);
        countDownText.gameObject.SetActive (false);
    }

    void DisableLights () {
        redLight.gameObject.SetActive (false);
        yellowLight.gameObject.SetActive (false);
        greenLight.gameObject.SetActive (false);
    }
}