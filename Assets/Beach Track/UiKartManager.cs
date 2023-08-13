using UnityEngine;

public enum Direction { x, y, z };

public class UiKartManager : MonoBehaviour
{
    public GameObject currentKart;

    public Direction direction = Direction.x;

    [SerializeField] private float rotateSpeed = 1f;
    [SerializeField] private bool antiClockwise = false;

    private void Update()
    {
        if (currentKart != null)
        {
            if (direction == Direction.x)
                currentKart.transform.Rotate(antiClockwise ? (-rotateSpeed) : (+rotateSpeed), 0, 0);
            else if (direction == Direction.y)
                currentKart.transform.Rotate(0, antiClockwise ? (-rotateSpeed) : (+rotateSpeed), 0);
            else if (direction == Direction.z)
                currentKart.transform.Rotate(0, 0, antiClockwise ? (-rotateSpeed) : (+rotateSpeed));
        }
    }
}