using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayerVehicle : MonoBehaviour {
    public GSF_GameController gameController;
    // Start is called before the first frame update
    int calledTimes = 0;
    void OnEnable () {
        if (calledTimes >= 1)
            StartCoroutine (gameController.SpawnPlayer ());

        calledTimes++;
    }
}