using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
	public float RotationSpeed = 45;

	// Update is called once per frame
	void Update()
	{
		var rot = transform.localEulerAngles;
		rot.y += RotationSpeed * Time.deltaTime;
		transform.localEulerAngles = rot;
	}
}
