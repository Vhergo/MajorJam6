using UnityEngine;

public abstract class WeaponLogic : MonoBehaviour
{
    [Header("General")]
    public DamageRange damage;
    public float knockback;
    public KeyCode fireKey;

    [Header("Prefabs")]
    public GameObject weaponPrefab;
    public Transform handlePosition;

    [Header("Script References")]
    public PlayerMovement player;
    protected Transform tempWeaponPosition;

    protected virtual void Awake() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        tempWeaponPosition = GameObject.FindGameObjectWithTag("Weapon").transform;
    }

    public abstract void ActivateWeapon();

    public abstract void InitializeSavedData(IAmmoUsage cachedWeaponData);
    public abstract IAmmoUsage GetMutableData();
}

