using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNitro : MonoBehaviour {

	public GameObject[] nitroParticles;
	public float increaseInVelocity = 50000f;
	private Rigidbody thisRb;

	private float velocityBeforeNitro;
	bool activated = false;
	private void Start () {
		thisRb = GetComponent<Rigidbody> ();
	}

	private void Update () {
		if (activated) {
			thisRb.velocity = transform.forward * (velocityBeforeNitro + increaseInVelocity);
		}
	}

	public void ApplyInstantForce (float fAmount) {
		thisRb.AddRelativeForce (Vector3.forward * thisRb.mass * fAmount, ForceMode.Impulse);
	}

	public void ActivateNitro (float durationInSeconds, float increaseInVelocity) {
		this.increaseInVelocity = increaseInVelocity;
		if (nitroParticles.Length > 0) {
			foreach (GameObject nitroParticle in nitroParticles) {
				nitroParticle.SetActive (true);
			}
		}
		velocityBeforeNitro = thisRb.velocity.magnitude;
		Debug.Log ("Velocity Before " + velocityBeforeNitro);
		activated = true;
		CancelInvoke ("StopNitro");
		Invoke ("StopNitro", durationInSeconds);
	}

	void StopNitro () {
		activated = false;
		// thisRb.velocity = transform.forward * velocityBeforeNitro;
	}
}