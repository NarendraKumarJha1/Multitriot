using System;
using UnityEngine;

public class Screenshotter : MonoBehaviour
{
	private float lastShot = 0;

	void Update()
	{
		if (Input.GetKey(KeyCode.F) && lastShot + 0.2f <= Time.time)
		{
			ScreenCapture.CaptureScreenshot("Assets/screenshots/screen_" + DateTime.Now.ToString("y.m.d h_i_s") + ".png");
			lastShot = Time.time;
			Debug.Log("Screenshot written");
		}

	}
}
