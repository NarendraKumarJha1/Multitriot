using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoControl : MonoBehaviour
{
	public GameObject StaticCamera;
	public GameObject FPSController;
	bool isFirstPerson = false;
	bool isFlyCam = false;

	bool textHidden = false;

	Vector3 savedLocalPos;
	Vector3 savedLocalRot;
	FlyCamera flyCam;
	private void Start()
	{
		if (StaticCamera != null && FPSController != null)
		{
			flyCam = StaticCamera.GetComponentInChildren<FlyCamera>();
			savedLocalPos = flyCam.transform.localPosition;
			savedLocalRot = flyCam.transform.localEulerAngles;

			StaticCamera.SetActive(true);
			FPSController.SetActive(false);
			isFirstPerson = false;
			isFlyCam = false;
			flyCam.enabled = false;
		}
	}
	private void Update()
	{
		if (StaticCamera != null && FPSController != null)
		{
			if (Input.GetKeyDown(KeyCode.F1))
			{
				if (StaticCamera.activeSelf)
				{
					StaticCamera.SetActive(false);
					FPSController.SetActive(true);
					isFirstPerson = true;
				}
				else
				{
					StaticCamera.SetActive(true);
					FPSController.SetActive(false);
					isFirstPerson = false;

					//always return to non-flycam.
					flyCam.transform.parent = StaticCamera.transform;
					flyCam.transform.localEulerAngles = savedLocalRot;
					flyCam.transform.localPosition = savedLocalPos;
					isFlyCam = false;
					flyCam.enabled = false;
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
				}
			}

			if (StaticCamera.activeSelf)
			{
				if (Input.GetKeyDown(KeyCode.F2))
				{
					if (isFlyCam)
					{
						flyCam.transform.parent = StaticCamera.transform;
						flyCam.transform.localEulerAngles = savedLocalRot;
						flyCam.transform.localPosition = savedLocalPos;
						isFlyCam = false;
						flyCam.enabled = false;
						Cursor.lockState = CursorLockMode.None;
						Cursor.visible = true;
					}
					else
					{
						isFlyCam = true;
						
						var cp = new Vector3(0, flyCam.transform.localEulerAngles.y, 0);
						flyCam.transform.parent = null;
						flyCam.transform.localEulerAngles = cp;
						flyCam.enabled = true;
					}
				}
			}
		}

		if (Input.GetKeyDown(KeyCode.F3))
		{
			textHidden = !textHidden;
		}

		if (Input.GetKeyDown(KeyCode.F5))
		{
			ScreenCapture.CaptureScreenshot("Assets/screenshots/screen_" + System.DateTime.Now.ToString("y.m.d h_i_s") + ".png");
		}
	}

	private void OnGUI()
	{
		if (!textHidden)
		{
			var label = "Press 'F1' To Switch to " + (isFirstPerson ? "DEMO CAMERA" : "FIRST PERSON MODE");
			GUI.Label(new Rect(10, 10, 400, 20), label);
			if (!isFirstPerson)
			{
				if (isFlyCam)
					label = "Press 'F2' To Resume Orbit";
				else
					label = "Press 'F2' To Free Fly";
				GUI.Label(new Rect(10, 30, 400, 20), label);
			}
		}
	}
}
