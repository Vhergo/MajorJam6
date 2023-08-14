using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDataSO : ScriptableObject
{
    public new string name;
    public WeaponType weaponType;
    public FiringType firingType;

    public float damage;
    public float bulletSpeed;
    public float fireRate;
/*    public int burstCount = 1;
    public int burstRate = 0;*/

    public float knockback;
    public float selfKnockback;

    public GameObject bulletPrefab;
    public RuntimeAnimatorController muzzleFlash;

    public enum WeaponType
    {
        Pistol,
        Shotgun,
        SMG,
        Rifle,
        Sniper,
        Railgun
    }

    public enum FiringType
    {
        SemiAuto,
        FullAuto,
        Charge
    }
}
