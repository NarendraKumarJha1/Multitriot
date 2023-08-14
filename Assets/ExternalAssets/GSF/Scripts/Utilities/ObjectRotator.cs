using System.Collections;
using UnityEngine;

public class ObjectRotator : MonoBehaviour {
    public enum UpdateMode {
        FixedUpdate,
        Update,
        LateUpdate
    }
    public enum Axis {
        X,
        Y,
        Z,
    }

    public Axis RotateAxis;
    public float RotarSpeed;
    public UpdateMode updateMode = UpdateMode.Update;

    private float rotateDegree;
    private Vector3 OriginalRotate;

    void Start () {
        OriginalRotate = transform.localEulerAngles;
    }
    void FixedUpdate () {
        if (updateMode == UpdateMode.FixedUpdate)
            Rotate ();
    }
    void Update () {
        if (updateMode == UpdateMode.Update)
            Rotate ();
    }

    void LateUpdate () {
        if (updateMode == UpdateMode.LateUpdate)
            Rotate ();
    }

    void Rotate () {
        rotateDegree += RotarSpeed * Time.deltaTime;
        rotateDegree = rotateDegree % 360;

        switch (RotateAxis) {
            case Axis.Y:
                transform.localRotation = Quaternion.Euler (OriginalRotate.x, rotateDegree, OriginalRotate.z);
                break;
            case Axis.Z:
                transform.localRotation = Quaternion.Euler (OriginalRotate.x, OriginalRotate.y, rotateDegree);
                break;
            default:
                transform.localRotation = Quaternion.Euler (rotateDegree, OriginalRotate.y, OriginalRotate.z);
                break;
        }
    }
}