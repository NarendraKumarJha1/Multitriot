using UnityEngine;

public class BarrelRollTrigger : MonoBehaviour
{
    public RollDirection rollDirection = RollDirection.Left;
    public float minVelocity = 40;
    public int rollCount = 1;
    public bool automaticRollCount = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {

            if (other.transform.root.GetComponent<BarrelRoll>())
            {

                if (other.attachedRigidbody.velocity.magnitude < minVelocity) return; // Early out if velocity is too low
                if (automaticRollCount) rollCount = FindRollCountByVelocity(other.attachedRigidbody.velocity.magnitude);
                if (rollDirection == RollDirection.Neutral) rollCount = 1;

                //other.transform.root.GetComponent<BarrelRoll>(). DoBarrelRoll(rollDirection, rollCount);

                if (rollDirection != RollDirection.Neutral)
                {
                    GameManager.Instance.totalFlips += rollCount;
                }

            }
            else
            {
                Debug.Log("No BarrelRoll Component Found On Triggered Object " + other.transform.root.name);
            }

        }
    }

    private int FindRollCountByVelocity(float velocity)
    {
        if (velocity > 40f && velocity < 60f)
            return 1;
        else if (velocity > 60f && velocity < 70f)
            return 2;
        else return 3;
    }
}