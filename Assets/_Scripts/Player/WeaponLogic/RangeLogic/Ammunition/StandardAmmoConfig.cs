using UnityEngine;

[System.Serializable]
public class StandardAmmoConfig
{
    public bool infiniteAmmo;

    public int maxAmmoCapacity;
    public int maxClipCapacity;
    public int ammoConsumption;

    public float reloadTime;
    public bool autoReload;
}
