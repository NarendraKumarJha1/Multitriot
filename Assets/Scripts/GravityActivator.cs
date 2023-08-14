using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityActivator : MonoBehaviour {

	public string tagToCompare = "Player";
	public Rigidbody[] objectsToActivateGravityUpon;

	private void OnTriggerEnter (Collider other) {
		if (other.transform.root.CompareTag (tagToCompare)) {
			ActivateGravityOnObjects ();
		}
	}

	void ActivateGravityOnObjects () {
		foreach (Rigidbody rb in objectsToActivateGravityUpon) {
			rb.isKinematic = false;
			rb.useGravity = true;
		}
	}
}