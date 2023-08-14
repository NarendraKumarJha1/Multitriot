using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObjects : MonoBehaviour {
    public GameObject[] objectsToDisable;
    int callTimes = 0;
    private void OnEnable () {
        if (callTimes >= 0) {
            foreach (GameObject go in objectsToDisable) {
                go.SetActive (false);
            }
        }
        callTimes++;
    }
}