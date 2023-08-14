using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableObjects : MonoBehaviour {
    public GameObject[] objectsToEnable;
    int calledTimes = 0;
    private void OnEnable () {
        if (calledTimes >= 1) {
            foreach (GameObject go in objectsToEnable) {
                go.SetActive (true);
            }
        }
        calledTimes++;
    }
}