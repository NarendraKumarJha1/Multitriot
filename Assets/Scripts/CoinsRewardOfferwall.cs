using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CoinsRewardOfferwall : MonoBehaviour {

	[Header ("UI")]
	public GameObject OfferWallDialog;
	public Button watchAdButton;
	public GameObject infoText;

	[Header ("Ad Sequence ID")]
	public int SequenceID;

	[Header ("Other Settings")]
	public bool showOneTime = false;
	public int coinsToGive = 1000;
	public GameObject objectToSendMessage;
	public string messageMethodName;

	public UnityEvent onComplete;

	void OnEnable () {
		//ConsoliAds.onRewardedVideoAdCompletedEvent += RewardedVideoCompleted;
	}

	void OnDisable () {
		//ConsoliAds.onRewardedVideoAdCompletedEvent -= RewardedVideoCompleted;
	}

	void Start () {
		if (OfferWallDialog) {
			OfferWallDialog.SetActive (false);
		}

		if (watchAdButton) {
			watchAdButton.interactable = false;
			if (watchAdButton.GetComponent<Animator> ())
				watchAdButton.GetComponent<Animator> ().enabled = false;
		}

		//ConsoliAds.Instance.LoadRewarded (SequenceID);
		InvokeRepeating ("CheckRewardedVideo", 0, 1f);
	}

	public void ShowRewardedVideo () {
		//GSF_AdsManager.ShowRewardedVideo (SequenceID, "Rewarded Video");

		if (showOneTime) {
			if (IsInvoking ("CheckRewardedVideo"))
				CancelInvoke ("CheckRewardedVideo");
		}
	}

	void RewardedVideoCompleted () {
		if (OfferWallDialog)
			OfferWallDialog.SetActive (false);
		if (watchAdButton) {
			watchAdButton.interactable = false;
			if (watchAdButton.GetComponent<Animator> ())
				watchAdButton.GetComponent<Animator> ().enabled = false;
		}

		SaveData.Instance.Coins += coinsToGive;
		GSF_SaveLoad.SaveProgress ();

		if (infoText)
			Helper.ShowMessage (infoText.GetComponentInChildren<Text> (), "Congrats! You have got " + coinsToGive + " Coins.", Color.green, 2);

		onComplete.Invoke ();

		if (objectToSendMessage)
			objectToSendMessage.SendMessage (messageMethodName, SendMessageOptions.DontRequireReceiver);

		//ConsoliAds.Instance.LoadRewarded (SequenceID);

	}

	void CheckRewardedVideo () {
		//if (ConsoliAds.Instance.IsRewardedVideoAvailable (SequenceID)) {
		//	if (OfferWallDialog)
		//		OfferWallDialog.SetActive (true);
		//	if (watchAdButton) {
		//		watchAdButton.gameObject.SetActive (true);
		//		watchAdButton.interactable = true;
		//		if (watchAdButton.GetComponent<Animator> ())
		//			watchAdButton.GetComponent<Animator> ().enabled = true;
		//	}
		//} else {
		//	Debug.Log ("Rewarded Video Not Available !");
		//	ConsoliAds.Instance.LoadRewarded (SequenceID);
		//}
	}
}