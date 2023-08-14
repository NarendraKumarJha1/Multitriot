using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskExecuter : MonoBehaviour {

    public string tagToCompare = "Player";
    public GameObject objectToSendMessage;
    public string message = "";
    public GameObject[] objectsToEnableUponTriggerEnter;
    public GameObject[] objectsToDisableUponTriggerEnter;
    public UnityEngine.UI.Text infoText;
    public string infoToBeDisplayedUponTrigger;

    private void OnTriggerEnter (Collider other) {
        if (other.transform.root.CompareTag (tagToCompare)) {
            foreach (GameObject go in objectsToEnableUponTriggerEnter) {
                go.SetActive (true);
            }

            foreach (GameObject go in objectsToDisableUponTriggerEnter) {
                go.SetActive (false);
            }

            if (objectToSendMessage)
                objectToSendMessage.SendMessage (message, SendMessageOptions.DontRequireReceiver);

            if (infoText && infoToBeDisplayedUponTrigger.Length > 0) {
                Helper.ShowMessage (infoText, infoToBeDisplayedUponTrigger, Color.green, 1.5f);
            }
        }
    }

}