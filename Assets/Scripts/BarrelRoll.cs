using System.Collections;
using UnityEngine;

public enum RollDirection
{
    Neutral = 0,
    Left = 1,
    Right = -1
}

public class BarrelRoll : MonoBehaviour
{

    private Rigidbody thisRigidbody;
    Vector3 initialRotation;
    bool isDoingBarrelRoll = false;

    private void Start()
    {
        thisRigidbody = GetComponent<Rigidbody>();
    }

    public void DoBarrelRoll(RollDirection diection, int rollCount = 1)
    {
        if (!isDoingBarrelRoll)
        {
            Vector3 _thisVelocity = thisRigidbody.velocity;
            Debug.Log("Initial velocity is = " + _thisVelocity.magnitude);

            initialRotation = transform.eulerAngles;
            Debug.Log(initialRotation);

            Vector3 _velY = new Vector3(0, _thisVelocity.y, 0);
            float timeToLand = 2 * _velY.magnitude / (9.80665f);
            float rollDuration = timeToLand / rollCount;
            Debug.Log("Time = " + timeToLand + "     Duration = " + rollDuration);

            StartCoroutine(BarrelRollCoroutine(diection, rollDuration, rollCount));

            isDoingBarrelRoll = true;
        }
    }

    private IEnumerator BarrelRollCoroutine(RollDirection direction, float duration, int loops = 1)
    {
        // float xyLerpTime = Time.deltaTime / (duration * loops);
        // Debug.Log (Time.deltaTime + "    XY lrp time:  ==== " + xyLerpTime + " ====== " + initialRotation.y);

        // float xRotation = 0, yRotation = 0;
        for (int i = 0; i < loops; i++)
        {
            float startRotationZ = transform.eulerAngles.z;

            if (direction == RollDirection.Left && startRotationZ > 90f) startRotationZ = 1f;
            else if (direction == RollDirection.Right && startRotationZ < 90f) startRotationZ = 359f;

            float remainingRotation = 0f;
            if (direction == RollDirection.Right)
                remainingRotation = 360.0f - (360.0f - startRotationZ);
            else if (direction == RollDirection.Left)
                remainingRotation = 360.0f - startRotationZ;
            else
                remainingRotation = 0f;

            // Debug.Log ("Remaining Rotation = " + remainingRotation);

            float endRotation = 0;
            if (i == loops - 1)
                endRotation = startRotationZ + (remainingRotation * (int)direction);
            else
                endRotation = startRotationZ + (360 * (int)direction);

            // Debug.Log ("Start Rot: " + startRotationZ + "   End Rot: " + endRotation);

            float elapsedTime = 0.0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;

                // float xRotation = Mathf.LerpAngle (transform.eulerAngles.x, 0, (duration / loops) * Time.deltaTime * 0.1f);
                // float yRotation = Mathf.LerpAngle (transform.eulerAngles.y, initialRotation.y, (duration / loops) * Time.deltaTime * 0.1f);

                // Debug.Log ("==================== " + t + "===" + t / (duration * loops));
                // Debug.Log ("--------------------" + Time.deltaTime * (duration * loops));

                // Debug.Log ("==================== " + t + "===" + (t / duration) / loops);

                // float xRotation = Mathf.LerpAngle (transform.eulerAngles.x, 0, xyLerpTime);

                // if (transform.eulerAngles.x < 359 )
                //     xRotation = Mathf.LerpAngle (transform.eulerAngles.x, 0, xyLerpTime);

                // // float yRotation = Mathf.LerpAngle (transform.eulerAngles.y, initialRotation.y, xyLerpTime);
                // yRotation = initialRotation.y;

                // float xRotation = Mathf.Lerp (transform.localRotation.eulerAngles.x, 0, xyLerpTime);
                // float yRotation = Mathf.Lerp (transform.localRotation.eulerAngles.y, initialRotation.y, xyLerpTime);

                float xRotation = Mathf.LerpAngle(transform.eulerAngles.x, 0, elapsedTime / (duration * loops));
                float yRotation = Mathf.LerpAngle(transform.eulerAngles.y, initialRotation.y, elapsedTime / (duration * loops));

                float zRotation = Mathf.Lerp(startRotationZ, endRotation, elapsedTime / duration) % 360;

                // Debug.Log ("final V ====== " + new Vector3 (xRotation, yRotation, zRotation));
                // transform.eulerAngles = new Vector3 (xRotation, yRotation, zRotation);

                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, zRotation);

                Quaternion targetRot = Quaternion.Euler(new Vector3(xRotation, yRotation, transform.eulerAngles.z));
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, elapsedTime / duration);

                yield return null;
            }
        }
        thisRigidbody.angularVelocity = Vector3.zero;
        isDoingBarrelRoll = false;
    }
}