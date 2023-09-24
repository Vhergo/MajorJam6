using UnityEngine;

[System.Serializable]
public class OverheatAmmoConfig
{
    public bool infiniteAmmo;

    public float maxEnergy;
    public float heatLimit;
    public float heatRate;

    public float lockoutTime;

    public float cooldownDelay;
    public float cooldownRate;
}
