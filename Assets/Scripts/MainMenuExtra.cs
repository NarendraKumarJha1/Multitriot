using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuExtra : MonoBehaviour {
    // Start is called before the first frame update
    public PlayerVehicleVFX menuCar;
    IEnumerator Start () {
        yield return new WaitForSeconds (0.5f);
        // Debug.Log("askhgjfgasjkfbasdbfad");
        if(menuCar)
        menuCar.PlayDissolveBackward (3f);
    }

    // Update is called once per frame
    void Update () {

    }
}