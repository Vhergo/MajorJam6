using Codice.CM.Common.Merge;
using System.Collections;
using UnityEngine;

public class MeleeWeaponLogic : WeaponLogic
{
    // Different damage types(like piercing, slashing, etc.)

    [Header("Weapon Data")]
    protected MeleeWeaponDataSO weaponData;

    [Header("General")]
    public float attackSpeed;
    public float attackRange;

    [Header("Prefabs")]
    protected GameObject muzzleFlashPrefab;
    protected RuntimeAnimatorController weaponStance;

    protected float fireRateTimer;

    protected override void Awake() {
        base.Awake();
        // Range Weapon Custom Logic HERE
    }

    public override void ActivateWeapon() {
        
    }

    public override void InitializeSavedData(IAmmoUsage cachedWeaponData) {
        
    }

    public override IAmmoUsage GetMutableData() {
        return null;
    }
}