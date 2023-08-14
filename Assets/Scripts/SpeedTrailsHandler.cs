using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
public class SpeedTrailsHandler : MonoBehaviour {
    public PigeonCoopToolkit.Effects.Trails.Trail[] trails;
    private bool trailsEmission = false;
    private RCC_CarControllerV3 this_rcc;
    private Rigidbody thisRigidbody;
    bool isGrounded = true;
    public float raycastLength = 1f;
    public LayerMask raycastCollisionLayer;
    WheelCollider wheel_RL, wheel_FL;
    private CameraShake cameraShake;
    float airTime = 0;
    void Start () {
        thisRigidbody = GetComponent<Rigidbody> ();
        this_rcc = GetComponent<RCC_CarControllerV3> ();
        wheel_RL = this_rcc.RearLeftWheelCollider.GetComponent<WheelCollider> ();
        wheel_FL = this_rcc.FrontLeftWheelCollider.GetComponent<WheelCollider> ();
        cameraShake = FindObjectOfType<CameraShake> ();
    }

    void Update () {
        // #if UNITY_EDITOR
        //         // Debug.DrawLine (transform.position, transform.position + (Vector3.down * raycastLength), Color.blue);
        //         Debug.DrawRay (transform.position, -transform.up * raycastLength, Color.green);
        // #endif

        //         if (Physics.Raycast (transform.position, -transform.up, raycastLength, raycastCollisionLayer)) {
        //             // Debug.Log ("Hitted");
        //             isGrounded = true;
        //         } else {
        //             isGrounded = false;
        //         }

        if (wheel_FL.isGrounded || wheel_RL.isGrounded)
            isGrounded = true;
        else
            isGrounded = false;

        if (!isGrounded && thisRigidbody.velocity.magnitude >= 50f) TrailsStateChange (true);
        else TrailsStateChange (false);

        if (isGrounded && airTime >= 2f && thisRigidbody.velocity.magnitude >= 70f) {

            // Debug.Log ("Shaking Camera here " + thisRigidbody.velocity.magnitude);
            if (!cameraShake) cameraShake = FindObjectOfType<CameraShake> ();
            cameraShake.ShakeCamera (2f, 0.015f);
            // cameraShake.GetComponent<Camera> ().DOShakePosition (0.3f, 2, 5);

        }

        if (isGrounded) airTime = 0;
        else airTime += Time.deltaTime;

    }

    public void TrailsStateToggle () {
        if (trailsEmission) {
            foreach (PigeonCoopToolkit.Effects.Trails.Trail trail in trails) {
                trail.Emit = !trailsEmission;
            }
        }

        trailsEmission = !trailsEmission;
    }

    public void TrailsStateChange (bool state) {
        foreach (PigeonCoopToolkit.Effects.Trails.Trail trail in trails) {
            trail.Emit = state;
        }

        trailsEmission = state;
    }
}