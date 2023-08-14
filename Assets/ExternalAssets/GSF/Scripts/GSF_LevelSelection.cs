using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GSF_LevelSelection : MonoBehaviour {

	[Header ("Scene Selection")]
	public Scenes PreviousScene;
	public Scenes NextScene;

	[Header ("Settings")]
	public bool Locked;
	public int PlayableLevels = 6;

	[Header ("UI Panels")]
	public GameObject UnlockAllLevelsBtn;
	public GameObject LoadingScreen;
	public GameObject LevelsPanel;
	public Image FillBar;
	public Text availableCoins;

	[Header ("Audio Settings")]
	public AudioSource ButtonClick;

	[Header ("Ad Sequence ID")]
	public int SequenceID;
	public bool LoadingSequence;
	public int LoadingSequenceID;

	private List<Button> LevelButtons = new List<Button> ();
	AsyncOperation async = null;

	void Start () {
		FillBar.fillAmount = 0;
		Time.timeScale = 1;
		AudioListener.pause = false;
		if (PlayerPrefs.HasKey ("MASTER_VOLUME")) {
			AudioListener.volume = PlayerPrefs.GetFloat ("MASTER_VOLUME");
		}
		LoadingScreen.SetActive (false);
		if (!GameManager.Instance.Initialized) {
			InitializeGame ();
		}
		availableCoins.text = SaveData.Instance.Coins.ToString ();
		GameManager.totalLevelsCount = PlayableLevels;
		CacheButtons ();
		LevelsInit ();

		if (PlayerPrefs.GetInt (Helper.UnlockAllLevels_str) == 1 || PlayerPrefs.GetInt (Helper.UnlockAllEverything_str) == 1) {
			UnlockAllLevelsBtn.SetActive (false);
		}
		if (ScreenFader.Instance)
			ScreenFader.Instance.FadeOut (1f);
	}

	void InitializeGame () {
		SaveData.Instance = new SaveData ();
		GSF_SaveLoad.LoadProgress ();
		GameManager.Instance.Initialized = true;
	}


	void Update () {
		if (Application.platform == RuntimePlatform.Android) {
			if (Input.GetKey (KeyCode.Escape)) {
				SceneManager.LoadScene (PreviousScene.ToString ());
			}
		}

		if (async != null) {
			FillBar.fillAmount = async.progress;
			if (async.progress >= 0.9f) {
				FillBar.fillAmount = 1.0f;
			}
		}

	}

	void CacheButtons () {
		Button[] levelButtons = LevelsPanel.transform.GetComponentsInChildren<Button> ();
		for (int i = 0; i < levelButtons.Length; i++) {
			LevelButtons.Add (levelButtons[i]);
		}
		LevelButtons = LevelButtons.OrderBy (x => Int32.Parse (x.gameObject.name)).ToList ();
		for (int i = 0; i < LevelButtons.Count; i++) {
			int LevelIndex = i + 1;

			LevelButtons[i].onClick.AddListener (() => PlayLevel (LevelIndex));
			LevelButtons[i].onClick.AddListener (() => ButtonClick.Play ());
		}
	}

	void LevelsInit () {
		if (!Locked) {
			for (int i = 0; i < LevelButtons.Count; i++) {
				if (i < PlayableLevels) {
					LevelButtons[i].interactable = true;
					LevelButtons[i].GetComponentInChildren<Text> ().text = (i + 1).ToString ();
				} else {
					LevelButtons[i].interactable = false;
					LevelButtons[i].GetComponentInChildren<Text> ().text = "";
				}
			}
		} else {
			for (int i = 0; i < LevelButtons.Count; i++) {
				LevelButtons[i].interactable = false;
				LevelButtons[i].GetComponentInChildren<Text> ().text = "";
			}

			for (int i = 0; i < LevelButtons.Count; i++) {
				if (i < SaveData.Instance.Level && i < PlayableLevels) {
					LevelButtons[i].interactable = true;
					LevelButtons[i].GetComponentInChildren<Text> ().text = (i + 1).ToString ();
				}
			}
		}
	}

	public void PlayLevel (int level) {
#if UNITY_EDITOR
		GameManager.Instance.EditorSession = false;
#endif
		GameManager.Instance.CurrentLevel = level;
		LoadingScreen.SetActive (true);
		StartCoroutine (LevelStart ());
	}

	IEnumerator LevelStart () {
		if (LoadingSequence) {
			yield return new WaitForSeconds (2);
		}
		async = SceneManager.LoadSceneAsync (NextScene.ToString ());
		yield return async;
	}

	public void BackBtn () {
		PlayerPrefs.SetInt ("SHOW_GARAGE", 1);
		PlayerPrefs.Save ();
		LoadingScreen.SetActive (true);
		StartCoroutine (GoBack ());
		// SceneManager.LoadScene (PreviousScene.ToString ());
	}

	IEnumerator GoBack () {
		async = SceneManager.LoadSceneAsync (PreviousScene.ToString ());
		yield return async;
	}

	public void UnlockAllLevels () {
		Helper.UnlockAllLevels ();
		LevelsInit ();
		// GSF_InAppController.Instance.onPurchaseComplete = () => {
		// 	// SaveData.Instance.Level = GameManager.totalLevelsCount;
		// 	// GSF_SaveLoad.SaveProgress ();

		// };

		// GSF_InAppController.Instance.BuyInAppProduct (inAppProductIdIndex);
	}
}