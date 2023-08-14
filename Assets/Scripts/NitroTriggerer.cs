using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NitroTriggerer : MonoBehaviour {

	private RCC_CarControllerV3 playerCar;
	public float forceAmount = 1000f;
	public float durationOfNitro = 1.5f;
	private AudioSource thisAudioSource;

	// Use this for initialization
	void Start () {
		thisAudioSource = GetComponent<AudioSource> ();
		// playerCar = RCC_SceneManager.Instance.activePlayerVehicle;
	}

	private void OnTriggerEnter (Collider other) {
		if (other.transform.root.CompareTag ("Player")) {
			thisAudioSource.Play ();
			RCC_SceneManager.Instance.activePlayerVehicle.GetComponent<PlayerNitro> ().ActivateNitro (2, 5f);
			RCC_SceneManager.Instance.activePlayerVehicle.GetComponent<PlayerNitro> ().ApplyInstantForce(forceAmount);
			RCC_SceneManager.Instance.activePlayerVehicle.GetComponent<VehicleNitro> ().ActivateNitro (durationOfNitro, forceAmount, 5000);

		}
	}

}