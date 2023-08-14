using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupType {
	Star,
	Nitro
}

public class Pickup : MonoBehaviour {
	public PickupType pickupType;

	public string tagToCompareOnTriggerEnter = "Player";
	public float pickupValue = 0;
	public bool destroyAfterUse = true;
	public bool respawnAfterTime = true;
	public float respawnDuration = 5f;
	private AudioSource thisAudioSource;

	private void Start () {
		thisAudioSource = GetComponent<AudioSource> ();
	}

	private void OnTriggerEnter (Collider other) {
		if (other.transform.root.CompareTag (tagToCompareOnTriggerEnter)) {
			thisAudioSource.Play ();
			switch (pickupType) {

				case PickupType.Star:
					FindObjectOfType<GSF_GameController> ().OnStarCatched ();
					break;
				case PickupType.Nitro:
					NitroManager.Instance.AddNitroAmount (pickupValue);
					break;

			}
			if (destroyAfterUse) {
				DestroyThis ();
				if (respawnAfterTime)
					Invoke ("Respawn", respawnDuration);
				// Destroy (this.gameObject);
			}
		}
	}

	private void DestroyThis () {
		GetComponent<Collider> ().enabled = false;
		GetComponent<Renderer> ().enabled = false;
	}

	private void Respawn () {
		GetComponent<Collider> ().enabled = true;
		GetComponent<Renderer> ().enabled = true;
	}
}