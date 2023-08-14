using UnityEngine;

[System.Serializable]
public class STFMouseLook
{
	public float Sensitivity = 2f;
	public float PitchMin = -90F;
	public float PitchMax = 90F;
	public float SmoothingTime = 5f;
	public bool LockCursor = true;

	private Quaternion YawTarget;
	private Quaternion PitchTarget;
	private bool cursorLocked = true;

	Vector3 TargetAngles;

	public void Init(Transform PitchTarget, Transform YawTarget)
	{
		this.PitchTarget = PitchTarget.localRotation;
		this.YawTarget = YawTarget.localRotation;
	}

	void ClampAngles()
	{
		TargetAngles.x = Mathf.Clamp(TargetAngles.x, PitchMin, PitchMax);

		if (TargetAngles.y > 180)
			TargetAngles.y -= 360;
		if (TargetAngles.x > 180)
			TargetAngles.x -= 360;
		if (TargetAngles.y < -180)
			TargetAngles.y += 360;
		if (TargetAngles.x < -180)
			TargetAngles.x += 360;
	}

	public void LookRotation(Transform pitchTransform, Transform yawTransform)
	{
		UpdateCursorLock();
		if (LockCursor && !cursorLocked)
			return;

		TargetAngles.y += Input.GetAxis("Mouse X") * Sensitivity;
		TargetAngles.x -= Input.GetAxis("Mouse Y") * Sensitivity;

		ClampAngles();

		//only updating one object, then we need to do it a little different.
		if (pitchTransform == yawTransform)
		{
			pitchTransform.localEulerAngles = TargetAngles;
		}
		else
		{
			var pitch = pitchTransform.localEulerAngles;
			pitch.x = TargetAngles.x;
			pitchTransform.localEulerAngles = pitch;

			var yaw = yawTransform.localEulerAngles;
			yaw.y = TargetAngles.y;
			yawTransform.localEulerAngles = yaw;
		}
	}

	public void SetCursorLock(bool value)
	{
		LockCursor = value;

		if (!LockCursor)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

	public void UpdateCursorLock()
	{
		if (LockCursor)
		{
			if (Input.GetKeyUp(KeyCode.Escape))
			{
				cursorLocked = false;
			}
			else if (Input.GetMouseButtonUp(0))
			{
				cursorLocked = true;
			}

			if (cursorLocked)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
			else if (!cursorLocked)
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}
	}

}
