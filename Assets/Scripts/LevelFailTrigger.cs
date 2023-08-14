using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFailTrigger : MonoBehaviour {

	private ReviveOfferWall reviveOfferWall;
	private void Start () {
		reviveOfferWall = FindObjectOfType<ReviveOfferWall> ();
	}

	private void OnTriggerEnter (Collider other) {
		if (other.transform.root.CompareTag ("Player")) {

			if (GSF_GameController.lastSpawnPoint != null && reviveOfferWall.IsRewardedVideoAvailable ()) {
				Debug.Log ("Showing Offerwall");
				reviveOfferWall.ShowOfferWall ();
			} else {
				GameManager.Instance.GameLoose (1);
				FindObjectOfType<RCC_Camera> ().StopFollow ();
			}
		}
	}
}