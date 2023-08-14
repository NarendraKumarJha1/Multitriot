using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Store : MonoBehaviour {

	public Text availableCoinsText;

	private void OnEnable () {
		availableCoinsText.text = SaveData.Instance.Coins.ToString ();
	}

	public void BuyCoinsStoreItem (int productIdIndex) {
		// GSF_InAppController.Instance.onPurchaseComplete = () => {
		// 	availableCoinsText.text = SaveData.Instance.Coins.ToString ();
		// };
		// GSF_InAppController.Instance.BuyInAppProduct (productIdIndex);
	}

	public void GiveCoins (int coins) {
		SaveData.Instance.Coins += coins;
		// SaveData.Instance.RemoveAds = true; // remove ads with all coins inapp purchases
		GSF_SaveLoad.SaveProgress ();

		availableCoinsText.text = SaveData.Instance.Coins.ToString ();
	}

	public void UnlockAllPlayers () {
		Helper.UnlockAllPlayers ();

	}

	public void UnlockAllLevels () {
		Helper.UnlockAllLevels ();
	}
	public void UnlockEverythingInApp () {
		Helper.UnlockEverything ();
		// NPBinding.UI.ShowAlertDialogWithSingleButton ("Purchase Successful", "Everything has been unlocked! Going back to Menu now.", "Ok", null);
	}

	public void RemoveAds () {
		SaveData.Instance.RemoveAds = true;
		GSF_SaveLoad.SaveProgress ();
		//GSF_AdsManager.RemoveAdvertisements ();
	}
}