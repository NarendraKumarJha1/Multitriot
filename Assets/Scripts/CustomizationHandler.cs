using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationHandler : MonoBehaviour {

    public GSF_PlayerSelection playerSelectionScript;
    public Toggle[] bodyColorButtons;
    public Toggle[] rimColorButtons;
    VehicleCustomizer currentVehicle = null;

    private void Awake () {
        Debug.Log ("CP " + GameManager.Instance.CurrentPlayer);
        currentVehicle = playerSelectionScript.Players[GameManager.Instance.CurrentPlayer].PlayerObject.GetComponentInChildren<VehicleCustomizer> ();

        playerSelectionScript.onSelectionChanged.AddListener (() => {
            currentVehicle = playerSelectionScript.Players[GameManager.Instance.CurrentPlayer].PlayerObject.GetComponentInChildren<VehicleCustomizer> ();
            SetupBodyColorButtons ();
            SetupRimColorButtons ();
        });

    }

    private void SetupBodyColorButtons () {
        for (int i = 0; i < bodyColorButtons.Length; i++) {
            Helper.FindChildByName (bodyColorButtons[i].gameObject, "Background").GetComponent<Image> ().color = currentVehicle.bodyPaintColors[i];
            int index = i;

            bodyColorButtons[i].onValueChanged.RemoveAllListeners ();
            bodyColorButtons[i].onValueChanged.AddListener ((bool isOn) => {
                if (isOn) {
                    playerSelectionScript.Players[GameManager.Instance.CurrentPlayer].currentBodyColorIndex = index;
                    // currentVehicle.CurrentBodyColorIndex = index;
                    currentVehicle.ApplyBodyColor (index);
                    SaveData.Instance.players[GameManager.Instance.CurrentPlayer].playerCustomization.bodyColorIndex = index;
                    GSF_SaveLoad.SaveProgress ();
                }
            });

            bodyColorButtons[i].isOn = false;
        }

        int selBodyColor = SaveData.Instance.players[GameManager.Instance.CurrentPlayer].playerCustomization.bodyColorIndex;
        Debug.Log ("sel Body Color Index: " + selBodyColor);
        if (selBodyColor >= 0) {
            bodyColorButtons[selBodyColor].isOn = true;
        }
    }

    public void ResetCurrentPlayerBodyColor () {
        foreach (Toggle t in bodyColorButtons) {
            t.isOn = false;
        }
        playerSelectionScript.Players[GameManager.Instance.CurrentPlayer].currentBodyColorIndex = -1;
        // currentVehicle.CurrentBodyColorIndex = -1;
        currentVehicle.ApplyBodyColor (-1);
        SaveData.Instance.players[GameManager.Instance.CurrentPlayer].playerCustomization.bodyColorIndex = -1;
        GSF_SaveLoad.SaveProgress ();
    }

    private void SetupRimColorButtons () {
        for (int i = 0; i < rimColorButtons.Length; i++) {
            Helper.FindChildByName (rimColorButtons[i].gameObject, "Background").GetComponent<Image> ().color = currentVehicle.rimEmissionColors[i];
            int index = i;

            rimColorButtons[i].onValueChanged.RemoveAllListeners ();
            rimColorButtons[i].onValueChanged.AddListener ((bool isOn) => {
                if (isOn) {
                    playerSelectionScript.Players[GameManager.Instance.CurrentPlayer].currentRimEmissionColorIndex = index;
                    // currentVehicle.CurrentRimEmissionColorIndex = index;
                    currentVehicle.ApplyRimEmissionColor (index);
                    SaveData.Instance.players[GameManager.Instance.CurrentPlayer].playerCustomization.rimEmissionIndex = index;
                    GSF_SaveLoad.SaveProgress ();
                }
            });

            rimColorButtons[i].isOn = false;
        }

        int selRimColor = SaveData.Instance.players[GameManager.Instance.CurrentPlayer].playerCustomization.rimEmissionIndex;
        Debug.Log ("sel Rim Color Index: " + selRimColor);
        if (selRimColor >= 0) {
            rimColorButtons[selRimColor].isOn = true;
        }
    }

    public void ResetCurrentPlayerRimColor () {
        foreach (Toggle t in rimColorButtons) {
            t.isOn = false;
        }
        playerSelectionScript.Players[GameManager.Instance.CurrentPlayer].currentRimEmissionColorIndex = -1;
        // currentVehicle.CurrentRimEmissionColorIndex = -1;
        currentVehicle.ApplyRimEmissionColor (-1);
        SaveData.Instance.players[GameManager.Instance.CurrentPlayer].playerCustomization.rimEmissionIndex = -1;
        GSF_SaveLoad.SaveProgress ();
    }

}