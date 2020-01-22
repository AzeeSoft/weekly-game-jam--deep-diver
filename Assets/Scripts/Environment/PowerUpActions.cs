using UnityEngine;

public class PowerUpActions : MonoBehaviour
{
    [Header("Health")]
    public float healthToAdd;

    [Header("Shield")]
    public float shieldDuration;

    public void Health(DiverModel diverModel)
    {
        diverModel.health.UpdateHealth(healthToAdd);
    }

    public void Shield(DiverModel diverModel)
    {
        diverModel.ActivateShield(shieldDuration);
    }
}