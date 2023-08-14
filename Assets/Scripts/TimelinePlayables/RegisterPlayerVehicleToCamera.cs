using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterPlayerVehicleToCamera : MonoBehaviour {

    public GSF_GameController gameController;
    public RCC_Camera rcc_camera;
    private void OnEnable () {
        if (gameController.player)
            rcc_camera.SetTarget (gameController.player);
    }
}