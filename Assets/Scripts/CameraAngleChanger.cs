using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAngleChanger : MonoBehaviour {

	public RCC_Camera playerCamera;
	public Transform cameraPositionToReach;
	public float rotationSpeed = 2f;
	public bool timescaleIndependent = false;
	public float resetTime = 2f;

	private void Start () {
		playerCamera = FindObjectOfType<RCC_Camera> ();
	}

	private void OnTriggerEnter (Collider other) {
		// Debug.Log ("==== " + other.transform.parent.tag);
		if (other.transform.root.CompareTag ("Player")) {
			if (!timescaleIndependent) Time.timeScale = 0.5f;
			if (!playerCamera) playerCamera = FindObjectOfType<RCC_Camera> ();
			playerCamera.SwitchPivotAngle (cameraPositionToReach.localPosition, cameraPositionToReach.localRotation, rotationSpeed, timescaleIndependent);
			Invoke ("ResetCamera", resetTime);
		}
	}

	public void ResetCamera () {
		playerCamera.SwitchPivotAngle (Vector3.zero, Quaternion.identity, rotationSpeed, timescaleIndependent);
		if (!timescaleIndependent) Time.timeScale = 1f;
	}
}