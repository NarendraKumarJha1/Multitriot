using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class starFxController : MonoBehaviour {

	public GameObject[] starFX;
	public AudioClip[] starSfxs;
	private AudioSource thisAudioSource;
	public int ea;
	public int currentEa;
	public float delay;
	public float currentDelay;
	public bool isEnd;
	public int idStar;
	public static starFxController myStarFxController;

	void Awake () {
		myStarFxController = this;
		thisAudioSource = GetComponent<AudioSource> ();
	}

	void Start () {
		//  Reset ();

	}

	private bool startPlaying = false;
	public void Play (int starsCount = 3) {
		Reset ();
		ea = starsCount;
		startPlaying = true;
		Debug.Log ("Playing Stars " + ea);
	}

	void Update () {
		if (!isEnd && startPlaying) {
			currentDelay -= Time.unscaledDeltaTime;
			if (currentDelay <= 0) {
				if (currentEa != ea) {
					currentDelay = delay;
					starFX[currentEa].SetActive (true);
					thisAudioSource.PlayOneShot (starSfxs[currentEa]);
					currentEa++;
				} else {
					isEnd = true;
					currentDelay = delay;
					currentEa = 0;
				}
			}
		}
		// if (Input.GetKeyDown (KeyCode.DownArrow)) {
		// 	Reset ();
		// }
	}

	public void Reset () {
		for (int i = 0; i < 3; i++) {
			starFX[i].SetActive (false);
		}
		currentDelay = delay;
		currentEa = 0;
		isEnd = false;
		for (int i = 0; i < 3; i++) {
			starFX[i].SetActive (false);
		}
	}
}