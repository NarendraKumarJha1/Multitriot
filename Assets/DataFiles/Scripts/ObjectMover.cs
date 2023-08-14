using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    public float rotationSpeed = 100f;

    public GameObject objectToMove;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {


    }

    private void OnMouseDrag()
    {
        float horizontalMovement = Input.GetAxis("Mouse X") * rotationSpeed;
        float yaw = -horizontalMovement * Time.deltaTime;
        objectToMove.transform.Rotate(Vector3.up, yaw, Space.World);
    }

}
