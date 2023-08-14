using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableNitroBtn : MonoBehaviour {
    public GameObject nitroBtn;
    public GameObject nitroBar;
    void Start () {

        nitroBtn.SetActive (false);
        if (nitroBar) nitroBar.SetActive (false);

    }

}