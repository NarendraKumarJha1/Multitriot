using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundsContinuer : MonoBehaviour {

	public Scenes sceneInWhichObjIsToBeDestroyed;
	static SoundsContinuer Instance;

	AudioSource thisAudioSource;

	private void Awake () {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad (this.gameObject);
		} else {
			Destroy (this.gameObject);
		}

		thisAudioSource = GetComponent<AudioSource> ();
	}

	private void Update () {
		if (SceneManager.GetActiveScene ().name.Equals ("PlayerSelection")) {
			thisAudioSource.volume = 0.2f;
		} else {
			thisAudioSource.volume = 0.5f;
		}
		//Debug.Log (SceneManager.GetActiveScene ().name);
		if (SceneManager.GetActiveScene ().name.Equals (sceneInWhichObjIsToBeDestroyed.ToString ()))
			Destroy (this.gameObject);

	}
}