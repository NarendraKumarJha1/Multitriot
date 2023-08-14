using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedIndicator : MonoBehaviour {

	private RCC_CarControllerV3 carController;
	private Text textComponent;

	// Use this for initialization
	void Start () {
		textComponent = GetComponent<Text> ();
	}
    private void OnEnable()
    {
		if (GameController.instance && GameController.instance.CurrentPlayer.GetComponent<RCC_CarControllerV3>())
			carController = GameController.instance.CurrentPlayer.GetComponent<RCC_CarControllerV3>();
    }
    private void Update () {
		//if (!carController) {

		//	GameObject player = GameObject.FindGameObjectWithTag ("Player");
		//	if (player) carController = player.GetComponent<RCC_CarControllerV3> ();

		//	return;
		//}
		if(carController)
			textComponent.text = ((int) carController.speed).ToString () + " <size=48>mph</size>"; //hasnain
	}
}