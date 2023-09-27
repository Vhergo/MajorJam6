using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Range Weapon", menuName = "Weapons/Range Weapon")]
public class RangeWeaponDataSO : WeaponDataSO
{
    public RangeWeaponDataSO() {
        weaponCategory = WeaponCategory.Range;
    }

    public RangeFiringType firingType;
    public RangeWeaponType rangeWeaponType;
    public IShootingMechanism shootingMechanism;

    public AmmoUsageType ammoUsageType;
    public StandardAmmoConfig standardAmmoConfig;
    public OverheatAmmoConfig overheatAmmoConfig;
    public UnlimitedAmmoConfig unlimitedAmmoConfig;

    public float bulletSpeed;
    public float fireRate;
    public float bulletSpread;
    public int bulletsPerShot;
    public bool symmetricalSpread;

    public bool burstEnabled;
    public float burstCount;
    public float burstRate;

    public float chargeTime;
    public bool chargeAutoFire;
    public float chargeAutoFireDelay;

    public float beamRange;
    public float beamDetectionRadius;
    public float activationDuration;
    public float deactivationDuration;
    public bool limitRange;
    public bool singleTarget;

    public float swapTime;
    public float selfKnockback;
    public float healAmount;

    public GameObject bulletPrefab;
    public Transform firePosition;
    public Transform handlePosition;

    public GameObject muzzleFlashPrefab;
    public Transform muzzleFlashPosition;
    public RuntimeAnimatorController muzzleFlashAnimation;

    public override WeaponLogic AddWeaponLogicComponent(GameObject weapon) {
        return weapon.AddComponent<RangeWeaponLogic>();
    }

    public override void InitializeWeaponData(WeaponLogic weaponLogic, KeyCode fireInput) {
        RangeWeaponLogic rangeWeaponLogic = (RangeWeaponLogic)weaponLogic;

        rangeWeaponLogic.weaponData = this;

        rangeWeaponLogic.damage = this.damage;
        rangeWeaponLogic.finalDamage = this.damage.maxDamage;
        rangeWeaponLogic.bulletSpeed = this.bulletSpeed;
        rangeWeaponLogic.fireRate = this.fireRate;
        rangeWeaponLogic.bulletSpread = this.bulletSpread;
        rangeWeaponLogic.bulletsPerShot = this.bulletsPerShot;
        rangeWeaponLogic.symmetricalSpread = this.symmetricalSpread;

        rangeWeaponLogic.burstEnabled = this.burstEnabled;
        rangeWeaponLogic.burstCount = this.burstCount;
        rangeWeaponLogic.burstRate = this.burstRate;

        rangeWeaponLogic.chargeTime = this.chargeTime;
        rangeWeaponLogic.chargeAutoFire = this.chargeAutoFire;
        rangeWeaponLogic.chargeAutoFireDelay = this.chargeAutoFireDelay;

        rangeWeaponLogic.beamRange = this.beamRange;
        rangeWeaponLogic.beamDetectionRadius = this.beamDetectionRadius;
        rangeWeaponLogic.activationDuration = this.activationDuration;
        rangeWeaponLogic.deactivationDuration = this.deactivationDuration;
        rangeWeaponLogic.limitRange = this.limitRange;
        rangeWeaponLogic.singleTarget = this.singleTarget;

        rangeWeaponLogic.ammoUsageType = this.ammoUsageType;
        rangeWeaponLogic.ammoUsage = this.CreateAmmoData();

        rangeWeaponLogic.shootingMechanism = this.DefineShootingMechanism(rangeWeaponLogic);

        rangeWeaponLogic.swapTime = this.swapTime;
        rangeWeaponLogic.knockback = this.knockback;
        rangeWeaponLogic.selfKnockback = this.selfKnockback;

        rangeWeaponLogic.weaponPrefab = this.weaponPrefab;
        rangeWeaponLogic.bulletPrefab = this.bulletPrefab;
        rangeWeaponLogic.firePosition = this.firePosition;

        rangeWeaponLogic.weaponStance = this.weaponStance;
        rangeWeaponLogic.handlePosition = this.handlePosition;

        rangeWeaponLogic.InstantiateMuzzleFlash();

        rangeWeaponLogic.fireKey = fireInput;
    }

    public IAmmoUsage CreateAmmoData() {
        switch (ammoUsageType) {
            case AmmoUsageType.Standard:
                return new StandardAmmoData(standardAmmoConfig);
            case AmmoUsageType.Overheat:
                return new OverheatAmmoData(overheatAmmoConfig);
            case AmmoUsageType.Unlimited:
                return new UnlimitedAmmoData(unlimitedAmmoConfig);
            default:
                throw new ArgumentException("Invalid ammo configuration provided!");
        }
    }

    public IShootingMechanism DefineShootingMechanism(RangeWeaponLogic rangeWeaponLogic) {
        if (firingType == RangeFiringType.Beam) {
            return new BeamShootingMechanism(rangeWeaponLogic);
        } else {
            return new BulletShootingMechanism(rangeWeaponLogic);
        }
    }

    // OnValidate is called when the script is loaded or a value is changed in the inspector
    // It is strictly an Editor Only method and will not be called during runtime
    private void OnValidate() {
        if (firingType == RangeFiringType.Beam) {
            rangeWeaponType = RangeWeaponType.Laser;
            burstEnabled = false;
        }
    }
}

public enum AmmoUsageType
{
    Standard,
    Overheat,
    Unlimited
}

public enum RangeWeaponType
{
    Pistol,
    Shotgun,
    SMG,
    Rifle,
    Sniper,
    Railgun,
    Laser
}

public enum RangeFiringType
{
    SemiAuto,
    FullAuto,
    Charge,
    Beam
    // Beam is fundamentally different from the others
    // Beam is a continuous input while the others are discrete actions
    // Beam needs 3 stages (Activation, Sustain, and Deactivation)
}