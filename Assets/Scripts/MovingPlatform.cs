using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

	Transform objectCatched = null;

	private void OnTriggerEnter(Collider other) {
		if(other.gameObject.layer == 8 && !objectCatched){	// RCC Layer
		objectCatched = other.transform.root;
		objectCatched.SetParent(this.transform);
		}
	}
	private void OnTriggerExit(Collider other) {
		if(other.gameObject.layer == 8){	// RCC Layer
		Debug.Log("Trigger Exit with "+ other.gameObject.name + " "+other.gameObject.layer);
		objectCatched.parent = null;
		objectCatched.SetParent(null);
		}
	}
}
