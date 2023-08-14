using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    public float health = 100;
    public PowerUpsHandler parentPowerUp = null;
    private void OnEnable()
    {
        parentPowerUp = GetComponent<PowerUpsHandler>();
    }
    public void ApplyDamage(float damage,string parentName)
    {
    }
}
