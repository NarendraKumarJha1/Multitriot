// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class PlayerFallDetector : MonoBehaviour {

// 	private ReviveOfferWall reviveOfferWall;
// 	public float waitForLevelFailed = 3f;
// 	public float mintimeforstartDrag = 2f;
// 	public float maxtimeforendDrag = 3f;
// 	private RCC_CarControllerV3 playerVehicle;
// 	private float time = 0f;
// 	private bool fellDown = false;

// 	private bool showingRevivePopup = false;
// 	private void Start () {
// 		playerVehicle = GetComponent<RCC_CarControllerV3> ();
// 		reviveOfferWall = FindObjectOfType<ReviveOfferWall> ();
// 		// InvokeRepeating("FaheemUpdate",0,1);
// 	}

// 	private void Update () {
// 		if (reviveOfferWall.isSkipped)
// 			showingRevivePopup = false;

// 		if (!fellDown) {
// 			if (!playerVehicle.RearLeftWheelCollider.GetComponent<WheelCollider> ().isGrounded) {
// 				time += Time.deltaTime;
// 				if (time >= mintimeforstartDrag && time <= maxtimeforendDrag) {
// 					// if (GameManager.Instance.CurrentLevel == 1)
// 					// 	GetComponent<Rigidbody> ().drag = 1f;
// 				}
// 				if (time >= waitForLevelFailed) {

// 					if (showingRevivePopup)
// 						return;
// 					// print ("fdjkfbkadfbaf,a f");
// 					if (!showingRevivePopup && !reviveOfferWall.isSkipped && GSF_GameController.lastSpawnPoint != null && reviveOfferWall.ShowOfferWall ()) {
// 						showingRevivePopup = true;
// 						// time = 0f;
// 						// fellDown = true;
// 						// print ("bla bla bal00");
// 					} else {
// 						fellDown = true;
// 						GameManager.Instance.GameLoose (1);
// 					}
// 				}
// 			} else {
// 				time = 0f;
// 				// GetComponent<Rigidbody> ().drag = 0f;
// 				if (showingRevivePopup)
// 					showingRevivePopup = false;
// 			}
// 		}
// 	}

// 	public void FallDownReset () {
// 		fellDown = false;
// 		time = 0f;
// 	}
// }