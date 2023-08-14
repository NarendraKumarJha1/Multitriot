using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxcelAdjuster : MonoBehaviour {

	// public RCC_CarControllerV3 carControllerV3;
	public Transform frontAxcel;
	public Transform rearAxcel;

	public Transform frontLeftAxcelJoint;
	public Transform frontRightAxcelJoint;
	public Transform rearLeftAxcelJoint;
	public Transform rearRightAxcelJoint;

	//private VehicleHealth thisVehicleHealth;
	private void Start () {
		//thisVehicleHealth = GetComponent<VehicleHealth> ();
		
	}

	void Update () {
		// if (!thisVehicleHealth.IsDead)
		 {
			frontAxcel.transform.position = (frontLeftAxcelJoint.position + frontRightAxcelJoint.position) / 2;
			// frontAxcel.transform.position = new Vector3 ((frontLeftAxcelJoint.position.x + frontRightAxcelJoint.position.x) / 2f,
			// 	(frontLeftAxcelJoint.position.y + frontRightAxcelJoint.position.y) / 2f,
			// 	(frontLeftAxcelJoint.position.z + frontRightAxcelJoint.position.z) / 2f);
			frontAxcel.transform.rotation = Quaternion.LookRotation (frontLeftAxcelJoint.position - frontRightAxcelJoint.position, transform.up) * Quaternion.Euler (new Vector3 (0, 90, 0));

			rearAxcel.transform.position = (rearLeftAxcelJoint.position + rearRightAxcelJoint.position) / 2;
			// rearAxcel.transform.position = new Vector3 (rearAxcel.transform.position.x,
			// 	(rearLeftAxcelJoint.position.y + rearRightAxcelJoint.position.y) / 2f,
			// 	(rearLeftAxcelJoint.position.z + rearRightAxcelJoint.position.z) / 2f);
			rearAxcel.transform.rotation = Quaternion.LookRotation (rearLeftAxcelJoint.position - rearRightAxcelJoint.position, transform.up) * Quaternion.Euler (new Vector3 (0, 90, 0));
		}
	}
}