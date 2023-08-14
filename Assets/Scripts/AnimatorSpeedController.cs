using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSpeedController : MonoBehaviour {

	public float speed = 1f;
	private Animator animator;
	private void Start () {
		animator = GetComponent<Animator> ();
		animator.speed = speed;
	}
}