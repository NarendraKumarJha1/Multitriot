using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RCC_CameraStopFollow : MonoBehaviour {
    private void OnTriggerEnter (Collider other) {
        if (other.transform.root.CompareTag ("Player")) {
            RCC_Camera rcc_Camera = FindObjectOfType<RCC_Camera> ();
            // rcc_Camera.enabled = false;
            rcc_Camera.StopFollow();
            rcc_Camera.GetComponentInChildren<Camera> ().DOFieldOfView (20, 2.5f);
            // rcc_Camera.transform.DORotate()
            // rcc_Camera.GetComponentInChildren<Camera> ().transform.DOLookAt (other.transform.root.position, 1);
        }
    }
}