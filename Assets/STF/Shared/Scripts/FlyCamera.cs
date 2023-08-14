using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyCamera : MonoBehaviour
{
	public float MoveSpeed = 5;
	public STFMouseLook MouseLook;

	Transform xform;

	// Use this for initialization
	private void Start()
	{
		xform = transform;
		MouseLook.Init(xform, xform);
	}

	// Update is called once per frame
	private void Update()
	{
		MouseLook.LookRotation(xform, xform);
	}
	private void FixedUpdate()
	{
		var input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		if (input.sqrMagnitude > 1)
			input.Normalize();

		var move = (transform.forward * input.y * MoveSpeed) + (transform.right * input.x * MoveSpeed);
		var pos = xform.position;
		pos += move * Time.fixedDeltaTime;
		xform.position = pos;
	}
}
