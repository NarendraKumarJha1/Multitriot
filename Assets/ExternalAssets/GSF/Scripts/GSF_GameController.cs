using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GSF_GameController : MonoBehaviour {

	[Header ("Scene Selection")]
	public Scenes PreviousScene;
	public Scenes NextScene;

	[Header ("Game Dialogues")]
	public Game_Dialogues Game_Elements;

	[Header ("SFX Objects")]
	public SFX_Objects SFX_Elements;
	[Header ("Players Prefabs")]
	public GameObject[] allPlayers;

	[Header ("Level Information")]
	public int PlayableLevels = 6;
	public Level_Data[] Levels;
	[Header ("Gameover States")]
	public bool ReasonBased;
	[Tooltip ("Gameover information is optional. This will not appear if un-checked.")]
	public GameOver[] States;

	[Header ("Level End Delay")]
	public float GameWinDelay;
	public float GameLooseDelay;

	[Header ("Ad Sequence ID")]
	public int SequenceID;

	//Local Variables

	GameObject AudioSource_Parent;
	GameObject FX_AudioSource;
	//Timer
	int minutes;
	int seconds;
	string time;
	int reward;
	float TimePerct;
	private int currentLevel;
	private int currentPlayer;
	private int FinishCount = 0;
	private bool isTimerEnabled;
	private int Rewardamount = 0;
	[HideInInspector]
	public bool TimerPaused = false;

	#region debug

	[Header ("Current Level")]
	[Range (1, 25)]
	public int StartLevel = 1;
	[Header ("Debug Values")]
	public int ObjectivesLeft = 0;
	public float LevelTime = 0.0f;

	#endregion

	private int starsCatched = 3;
	public int StarsCatched { get { return starsCatched; } }

	public GameObject player;
	private UnityStandardAssets.Utility.WaypointProgressTracker playerProgressTracker;
	public static Transform lastSpawnPoint = null;
	private Transform targetPoint = null;

	void Start () {

		if (ScreenFader.Instance)
			ScreenFader.Instance.FadeOut (1f);

		//GameManager Variables Reset
		GameManager.Instance.GameStatus = null;
		GameManager.Instance.totalFlips = 0;

		Time.timeScale = 1;
		AudioListener.pause = false;
		if (PlayerPrefs.HasKey ("MASTER_VOLUME")) {
			//AudioListener.volume = PlayerPrefs.GetFloat ("MASTER_VOLUME"); //$ree
		}

		//Initialize Framework
		Init ();

		InvokeRepeating ("CheckDirection", 5, 1);
	}

	#region Initialization

	void Init () {
		if (!GameManager.Instance.Initialized) {
			InitializeGame ();
		}
		//Initialize Audio Sources
		InitializeAudio (FX_AudioSource, "FX_AudioSource");
		FX_AudioSource = GameObject.Find ("FX_AudioSource");

		SpecifyCurrentLevel ();
		InitializeLevel ();
		ActivateLevel ();

		if (currentLevel != 1)
			StartCoroutine (SpawnPlayer ());

		LevelSpecificDecisions ();
	}

	void ActivateFreeMode () {
		Game_Elements.Timer.SetActive (false);
		foreach (Level_Data level in Levels) {
			Destroy (level.LevelObject);
		}

		// GA_TestLogEvents.Instance.GameAnalyticeEventStart ("Free Mode Gameplay");
	}

	void InitializeAudio (GameObject obj, string name) {
		AudioSource_Parent = GameObject.Find ("SFXController");
		obj = new GameObject (name);
		obj.transform.position = AudioSource_Parent.transform.position;
		obj.transform.rotation = AudioSource_Parent.transform.rotation;
		obj.transform.parent = AudioSource_Parent.transform;
		obj.AddComponent<AudioSource> ();
		obj.GetComponent<AudioSource> ().priority = 128;
	}

	void InitializeGame () {
		SaveData.Instance = new SaveData ();
		GSF_SaveLoad.LoadProgress ();
		GameManager.Instance.Initialized = true;
	}

	void InitializeLevel () {

		Time.timeScale = 1;

		Game_Elements.LevelComplete.SetActive (false);
		Game_Elements.FinalComplete.SetActive (false);
		Game_Elements.LevelFailed.SetActive (false);
		Game_Elements.GameExit.SetActive (false);
		Game_Elements.LoadingScreen.SetActive (false);
		Game_Elements.PauseMenu.SetActive (false);
		Game_Elements.HelpScreen.SetActive (false);

		//Reset Finish Points
		if (Levels[currentLevel - 1].Objectives.Length > 0) {
			for (int i = 0; i < Levels[currentLevel - 1].Objectives.Length; i++) {
				if (Levels[currentLevel - 1].Objectives[i].FinishPoint != null)
					Levels[currentLevel - 1].Objectives[i].FinishPoint.SetActive (false);
				if (Levels[currentLevel - 1].Objectives[i].Instruction == "")
					Debug.LogWarning ("Please write insctruction for Level->" + GameManager.Instance.CurrentLevel + " Objective->" + (i + 1) + " in the inspector !");
			}
		} else if (Levels[currentLevel - 1].Objectives.Length == 0) {
			Debug.LogError ("No Objectives have been defined in the inspector !");
		}

		//SpawnItems
		if (Levels[currentLevel - 1].Items.Length > 0) {
			for (int i = 0; i < Levels[currentLevel - 1].Items.Length; i++) {
				SetItemPosition (Levels[currentLevel - 1].Items[i].Item, Levels[currentLevel - 1].Items[i].SpawnPoint);
			}
		}

		if (Levels[currentLevel - 1].GiveReward) {
			if (Levels[currentLevel - 1].RewardLevels.Length == 0)
				Debug.LogWarning ("No Rewards have been defined in the inspector !");
		}

	}

	void LevelSpecificDecisions () {
		if (Levels[currentLevel - 1].Objectives.Length > 0) {
			ActivateFinishPoint ();
		}

		if (Levels[currentLevel - 1].isTimeBased) {
			isTimerEnabled = true;
			Game_Elements.Timer.SetActive (true);
		} else {
			isTimerEnabled = false;
			Game_Elements.Timer.SetActive (false);
		}

		//In-Game Timer
		if (isTimerEnabled)
			InvokeRepeating ("GameTimer", 0, 1);
	}

	public IEnumerator SpawnPlayer () {
		Debug.Log ("Spawning Player");
		Vector3 spawnPos;
		Quaternion spawnRot;

		spawnPos = Levels[currentLevel - 1].playerSpawnPoint.position;
		spawnRot = Levels[currentLevel - 1].playerSpawnPoint.rotation;

		// Debug.Log ("CP = " + GameManager.Instance.CurrentPlayer);
		player = Instantiate (allPlayers[GameManager.Instance.CurrentPlayer], spawnPos, spawnRot) as GameObject;
		if (!playerProgressTracker) playerProgressTracker = player.GetComponent<UnityStandardAssets.Utility.WaypointProgressTracker> ();
		yield return new WaitForSeconds (1);
		player.GetComponent<PlayerVehicleVFX> ().PlayDissolveBackward (3f);
		yield return new WaitForSeconds (3);
		RCC_CarControllerV3 playerRcc = player.GetComponent<RCC_CarControllerV3> ();
		// if (playerRcc && !playerRcc.engineRunning)
		playerRcc.StartEngine ();

		// if (currentLevel == 1) {
		// 	player.GetComponent<PlayerVehicleVFX> ().PlayDissolveBackward (0f);
		// 	RCC_CarControllerV3 playerRcc = player.GetComponent<RCC_CarControllerV3> ();
		// 	// if (playerRcc && !playerRcc.engineRunning)
		// 	playerRcc.StartEngine ();
		// } else {
		// 	yield return new WaitForSeconds (1);
		// 	player.GetComponent<PlayerVehicleVFX> ().PlayDissolveBackward (3f);
		// 	yield return new WaitForSeconds (3);
		// 	RCC_CarControllerV3 playerRcc = player.GetComponent<RCC_CarControllerV3> ();
		// 	// if (playerRcc && !playerRcc.engineRunning)
		// 	playerRcc.StartEngine ();
		// }

	}

	void SpecifyCurrentLevel () {
#if UNITY_EDITOR
		if (GameManager.Instance.EditorSession) {
			currentLevel = StartLevel;
		} else {
			currentLevel = GameManager.Instance.CurrentLevel;
		}
#else
		currentLevel = GameManager.Instance.CurrentLevel;
#endif

		// GA_TestLogEvents.Instance.GameAnalyticeEventStart ("Level " + currentLevel + " Start");
	}

	void SetItemPosition (GameObject Item, Transform Position) {
		Item.transform.position = Position.position;
		Item.transform.rotation = Position.rotation;
	}

	void ActivateLevel () {
		for (int i = 0; i < Levels.Length; i++) {
			if (i == currentLevel - 1) {
				Levels[i].LevelObject.SetActive (true);
			} else {
				Destroy (Levels[i].LevelObject);
			}
		}

		GameManager.Instance.Objectives = Levels[currentLevel - 1].Objectives.Length;
		//For Debug
		ObjectivesLeft = GameManager.Instance.Objectives;

		LevelTime = (Levels[currentLevel - 1].Minutes * 60) + Levels[currentLevel - 1].Seconds;
	}

	void ActivateFinishPoint () {
		if (FinishCount == 0) {
			if (Levels[currentLevel - 1].Objectives[FinishCount].FinishPoint != null)
				Levels[currentLevel - 1].Objectives[FinishCount].FinishPoint.SetActive (true);
			ShowInstruction ();
		} else if (FinishCount == Levels[currentLevel - 1].Objectives.Length) {
			if (Levels[currentLevel - 1].Objectives[FinishCount - 1].FinishPoint != null)
				Levels[currentLevel - 1].Objectives[FinishCount - 1].FinishPoint.SetActive (false);
		} else {
			if (Levels[currentLevel - 1].Objectives[FinishCount - 1].FinishPoint != null)
				Levels[currentLevel - 1].Objectives[FinishCount - 1].FinishPoint.SetActive (false);
			if (Levels[currentLevel - 1].Objectives[FinishCount].FinishPoint != null)
				Levels[currentLevel - 1].Objectives[FinishCount].FinishPoint.SetActive (true);
			ShowInstruction ();
		}
	}

	public void ShowInstruction () {
		Game_Elements.InstructionText.text = Levels[currentLevel - 1].Objectives[FinishCount].Instruction;
		FinishCount++;
	}

	void GameTimer () {
		if (!TimerPaused) {
			if (LevelTime >= 0.0f && GameManager.Instance.GameStatus != "Loose" && GameManager.Instance.Objectives > 0)
				LevelTime -= 1;
			minutes = ((int) LevelTime / 60);
			seconds = ((int) LevelTime % 60);
			time = minutes.ToString ("00") + ":" + seconds.ToString ("00");
			Game_Elements.Timer_txt.text = time;

			if (LevelTime <= 15.0f && LevelTime > 0.0f) {
				Game_Elements.Timer_txt.color = Color.red;
				SFX_Elements.CountDown.SetActive (true);
				if (GameManager.Instance.Objectives <= 0) {
					SFX_Elements.CountDown.SetActive (false);
				}
			} else if (LevelTime == 0.0f && GameManager.Instance.GameStatus != "Loose" && GameManager.Instance.Objectives > 0) {
				SFX_Elements.CountDown.SetActive (false);
				GameManager.Instance.GameLoose ();
			}
		}
	}

	#endregion

	#region Controller Logic

	public void OnLevelCheck (int reasonIndex) {
		//For Debug
		ObjectivesLeft = GameManager.Instance.Objectives;

		if (GameManager.Instance.Objectives > 0 && GameManager.Instance.GameStatus != "Loose") {
			if (Levels[currentLevel - 1].Objectives.Length != 0)
				ActivateFinishPoint ();
			else
				Debug.LogWarning ("No Objectives have been defined in the inspector !");
		} else if (GameManager.Instance.Objectives == 0) {
			if (Levels[currentLevel - 1].Objectives.Length != 0)
				ActivateFinishPoint ();
			else
				Debug.LogWarning ("No Objectives have been defined in the inspector !");

			//Calculate Reward
			// if (Levels[currentLevel - 1].GiveReward) {
			// 	GiveRewards ();
			// }
			DisableAudio ();
			FX_AudioSource.GetComponent<AudioSource> ().PlayOneShot (SFX_Elements.LevelCompleteSFX);
			StartCoroutine (OnLevelStatus ());
		} else if (GameManager.Instance.GameStatus == "Loose") {
			DisableAudio ();
			if (ReasonBased)
				SetGameOverReason (reasonIndex);
			FX_AudioSource.GetComponent<AudioSource> ().PlayOneShot (SFX_Elements.LevelFailedSFX);
			StartCoroutine (OnLevelStatus ());
		}
	}

	IEnumerator OnLevelStatus () {
		CancelInvoke ();
		GameManager.Instance.PauseTimer ();
		SFX_Elements.CountDown.SetActive (false);

        // disable player sounds
        foreach (AudioSource audio in player.GetComponentsInChildren<AudioSource>())
        {
            audio.enabled = false;
        }

        if (GameManager.Instance.GameStatus == "Loose") {
			yield return new WaitForSeconds (GameLooseDelay);
			Game_Elements.LevelFailed.SetActive (true);
			Helper.FindChildByName (Game_Elements.LevelFailed, "TotalCoinsValue").GetComponentInChildren<Text> ().text = SaveData.Instance.Coins.ToString ();
			Helper.FindChildByName (Game_Elements.LevelFailed, "TotalFlipsValue").GetComponentInChildren<Text> ().text = GameManager.Instance.totalFlips.ToString ();
		} else {
			UpdateLevel ();
			yield return new WaitForSeconds (GameWinDelay);

			if (currentLevel == PlayableLevels) {
				Game_Elements.FinalComplete.SetActive (true);
			} else {
				Game_Elements.LevelComplete.SetActive (true);
			}
			if (Levels[currentLevel - 1].GiveReward) {
				GiveRewards ();
			}

			// yield return new WaitForSeconds (2.5f);
			// if (!CheckEverythingUnlocked ())
			// 	Game_Elements.unlockEverythingDialog.SetActive (true);
		}

		Time.timeScale = 0.0001f;
		//yield return new WaitForSecondsRealtime (2.0f);
	}

	void SetGameOverReason (int reasonIndex) {
		if (States.Length > 0 && reasonIndex < States.Length) {
			Game_Elements.ReasonObject.SetActive (true);
			Game_Elements.GameOverLogo.sprite = States[reasonIndex].Icon;
			Game_Elements.GameOverText.text = States[reasonIndex].Reason;
		} else {
			Debug.LogError ("Game over reason for index " + reasonIndex + " does not exist !");
		}
	}

	void CalculateRewardAmount (int index) {
		TimePerct = (LevelTime / ((Levels[currentLevel - 1].Minutes * 60) + Levels[currentLevel - 1].Seconds)) * 100;
		if ((int) TimePerct >= Levels[currentLevel - 1].RewardLevels[index].MinTime && (int) TimePerct <= Levels[currentLevel - 1].RewardLevels[index].MaxTime) {
			for (int i = 0; i < Levels[currentLevel - 1].RewardLevels[index].RewardInfo.Length; i++) {
				reward = Levels[currentLevel - 1].RewardLevels[index].RewardInfo[i].RewardAmount;

				//Give Your Rewards Here
				switch (Levels[currentLevel - 1].RewardLevels[index].RewardInfo[i].RewardType) {
					case RewardTypes.Coins:
						Debug.Log ("Reward # " + i + "-> " + reward + " " + Levels[currentLevel - 1].RewardLevels[index].RewardInfo[i].RewardType);
						break;
					case RewardTypes.Other:
						Debug.Log ("Reward # " + i + "-> " + reward + " " + Levels[currentLevel - 1].RewardLevels[index].RewardInfo[i].RewardType);
						break;
				}
			}
		}
	}

	void GiveRewards () {
		int currentCoins = SaveData.Instance.Coins;
		int reward = Levels[currentLevel - 1].coinsReward * starsCatched;

		Debug.Log ("Reward Availed = " + reward);

		GameObject activeDialog = Game_Elements.LevelComplete;

		if (currentLevel == PlayableLevels)
			activeDialog = Game_Elements.FinalComplete;

		activeDialog.GetComponentInChildren<starFxController> ().Play (starsCatched);

		// double coins rewarded video
		if (activeDialog.GetComponentInChildren<CoinsRewardOfferwall> ())
			activeDialog.GetComponentInChildren<CoinsRewardOfferwall> ().coinsToGive = reward;

		Helper.FindChildByName (activeDialog, "RewardValue").GetComponentInChildren<DelayedCounter> ().StartCounting (1, reward);
		Helper.FindChildByName (activeDialog, "TotalCoinsValue").GetComponentInChildren<DelayedCounter> ().StartCounting (currentCoins, currentCoins + reward);
		Helper.FindChildByName (activeDialog, "TotalFlipsValue").GetComponentInChildren<DelayedCounter> ().StartCounting (1, GameManager.Instance.totalFlips);
		// Helper.FindChild (activeDialog, "ScoreValue").GetComponent<Text> ().text = score.ToString ();
		// int reward = Levels[currentLevel - 1].coinsBonus * currentLevel;
		// currentCoins += reward;
		// Helper.FindChild (activeDialog, "RewardValue").GetComponent<Text> ().text = reward.ToString ();
		// Helper.FindChild (activeDialog, "TotalCoinsValue").GetComponent<Text> ().text = (score + reward).ToString ();

		SaveData.Instance.Coins = currentCoins + reward;
		GSF_SaveLoad.SaveProgress ();

		// if (Levels[currentLevel - 1].RewardLevels.Length > 0) {
		// 	for (int i = 0; i < Levels[currentLevel - 1].RewardLevels.Length; i++) {
		// 		//Give reward here
		// 		CalculateRewardAmount (i);
		// 	}
		// } else {
		// 	Debug.LogError ("No rewards have been defined in the inspector !");
		// }
	}

	void DisableAudio () {
		for (int i = 0; i < SFX_Elements.BGMusicLoops.Length; i++) {
			SFX_Elements.BGMusicLoops[i].SetActive (false);
		}
	}

	void UpdateLevel () {
		if (currentLevel == SaveData.Instance.Level) {
			SaveData.Instance.Level++;
			GSF_SaveLoad.SaveProgress ();
		}
	}

	#endregion

	

	#region Interface-Logic

	public void PauseGame () {
		Time.timeScale = 0.0f;
		AudioListener.pause = true;
	}

	public void ResumeGame () {
		Time.timeScale = 1.0f;
		AudioListener.pause = false;
	}

	public void RetryLevel () {
		Game_Elements.LoadingScreen.SetActive (true);
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}

	public void ShowUnlockEverythingPopup () {
		if (!CheckEverythingUnlocked ())
			Game_Elements.unlockEverythingDialog.SetActive (true);
		else
			NextLevel ();
	}

	public void NextLevel () {
		if (currentLevel != PlayableLevels) {
#if UNITY_EDITOR
			GameManager.Instance.EditorSession = false;
#endif
			GameManager.Instance.CurrentLevel += 1;

			Game_Elements.LoadingScreen.SetActive (true);
			SceneManager.LoadScene (NextScene.ToString ());
		}
	}

	public void MainMenu () {
		Game_Elements.LoadingScreen.SetActive (true);
		SceneManager.LoadScene (PreviousScene.ToString ());
	}

	public void RepositionPlayer () {
		Debug.Log ("in RepositionPlayer => CP = " + GameManager.Instance.CurrentPlayer);
		if (player.GetComponent<Rigidbody> ()) {
			player.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		}
		if (lastSpawnPoint != null) {
			player.transform.position = lastSpawnPoint.position;
			player.transform.rotation = lastSpawnPoint.rotation;
		} else {
			player.transform.position = Levels[currentLevel - 1].playerSpawnPoint.position;
			player.transform.rotation = Levels[currentLevel - 1].playerSpawnPoint.rotation;
		}
		// GameObject player = Instantiate (allPlayers[GameManager.Instance.CurrentPlayer], newPoint.position, newPoint.rotation) as GameObject;
	}

	public void ResetPlayerCar () {
		RepositionPlayer ();

		if (starsCatched > 1)
			starsCatched--;
		// player.GetComponent<RCC_CarControllerV3> ().ResetCarImmediate ();
	}

	private void CheckDirection () {
		// if (!player) return;
		// if (!targetPoint) targetPoint = FindObjectOfType<Target> ().transform;

		// float dotProduct = Vector3.Dot (player.transform.forward.normalized, (targetPoint.position - player.transform.position).normalized);
		// if (dotProduct < 0f)
		// 	Game_Elements.wrongDirectionIndicator.gameObject.SetActive (true);
		// else
		// 	Game_Elements.wrongDirectionIndicator.gameObject.SetActive (false);
		if (!player) return;
		// if (!targetPoint) targetPoint = FindObjectOfType<Target> ().transform;
		// Vector3 tempVector = playerProgressTracker.target.position;

		// float dotProduct = Vector3.Dot (player.transform.forward.normalized, (new Vector3 (tempVector.x, player.transform.position.y, tempVector.z) - player.transform.position).normalized);
		float dotProduct = Vector3.Dot (player.transform.forward.normalized, (playerProgressTracker.target.position - player.transform.position).normalized);
		if (dotProduct < 0f)
			Game_Elements.wrongDirectionIndicator.gameObject.SetActive (true);
		else
			Game_Elements.wrongDirectionIndicator.gameObject.SetActive (false);
		// Debug.Log ("Dot Product : " + dotProduct);
	}

	public bool CheckEverythingUnlocked () {
		if (SaveData.Instance.Level > GameManager.totalLevelsCount)
			return false;
		if (!SaveData.Instance.RemoveAds)
			return false;

		// foreach (PlayerSaveableAttributes vehicle in SaveData.Instance.players) {
		// 	if (vehicle.Locked)
		// 		return false;
		// }

		return true;
	}

	public void UnlockEverything () {
		Helper.UnlockEverything ();
		MainMenu ();

		// GSF_InAppController.Instance.onPurchaseComplete = () => {
		// 	Helper.UnlockEverything ();
		// 	NPBinding.UI.ShowAlertDialogWithSingleButton ("Purchase Successful", "Everything has been unlocked! Going back to Menu now.", "Ok", null);
		// 	MainMenu ();
		// };

		// GSF_InAppController.Instance.BuyInAppProduct (inappIndex);
	}

	public void UnlockAllLevels () {
		Helper.UnlockAllLevels ();
		MainMenu ();
	}
	public void UnlockAllPlayers () {
		Helper.UnlockAllPlayers ();
		MainMenu ();
	}

	public void RemoveAds () {
		Helper.RemoveAds ();
	}

	public void RateUs () {
		Application.OpenURL ("https://play.google.com/store/apps/details?id=" + Application.identifier);
	}

	public void OnStarCatched () {
		starsCatched++;
	}

	#endregion
}