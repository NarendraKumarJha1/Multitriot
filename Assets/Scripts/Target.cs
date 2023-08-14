using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Target : MonoBehaviour {
	public GameObject Sparkls;

	void Start () {
		//		Sparkls = GameObject.Find ("Sparks").gameObject;
	}

	private void OnTriggerEnter (Collider other) {
		if (other.transform.root.CompareTag ("Player")) {
			if (Sparkls) {
				Sparkls.SetActive (true);
				Sparkls.GetComponent<ParticleSystem> ().Play ();
			}
			Debug.Log ("Level Completed");
			other.GetComponentInParent<RCC_CarControllerV3> ().canControl = false;
			// FindObjectOfType<RCC_Camera> ().StopFollow ();
			other.GetComponentInParent<PlayerVehicleVFX> ().PlayDissolveForward (1);
			// other.GetComponentInParent<BikeControl> ().activeControl = false;
			other.GetComponentInParent<Rigidbody> ().velocity = Vector3.zero;
			other.GetComponentInParent<Rigidbody> ().useGravity = false;
			GameManager.Instance.TaskComplete ();
			GameManager.Instance.GameStatus = "Win";
		}
	}

}