using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))] // Insure this gameObject has a LineRenderer component
public class RangeWeaponLogic : WeaponLogic
{
    #region (IMMUTABLE) SCRIPTABLE OBJECT VARIABLES
    public RangeWeaponDataSO weaponData;

    // These two are only to display values temporarily
    public float ammoLeftInClip;
    public float ammoLeftInTotal;

    public float bulletSpeed;
    public float fireRate;

    public float bulletSpread;
    public int bulletsPerShot;
    public bool symmetricalSpread;

    public bool burstEnabled; // allows for player controlled burst
    public float burstCount;
    public float burstRate;

    public float chargeTime;
    public bool chargeAutoFire;
    public float chargeAutoFireDelay;

    public LineRenderer beamRenderer;
    public float beamRange;
    public float beamDetectionRadius;
    public float activationDuration;
    public float deactivationDuration;
    public bool limitRange;
    public bool singleTarget;

    public AmmoUsageType ammoUsageType;
    public IAmmoUsage ammoUsage;

    public IShootingMechanism shootingMechanism;

    public float swapTime;
    public float selfKnockback;
    public float healAmount;

    // NOTE: FIGURE OUT PERMANENT SOLUTION FOR THIS
    public Transform firePoint;

    public GameObject bulletPrefab;
    public Transform firePosition;

    public GameObject muzzleFlash;

    public RuntimeAnimatorController weaponStance;
    #endregion

    #region (MUTABLE) VARIABLES
    public float fireRateTimer;
    public float chargeTimer;
    public float finalDamage;
    #endregion

    protected override void Awake() {
        base.Awake();
        // fire point should be weapon specific
        firePoint = GameObject.FindGameObjectWithTag("FirePoint").transform;
        beamRenderer = GetComponent<LineRenderer>();
    }

    private void Start() {
        fireRateTimer = fireRate;
    }

    private void Update() {
        // These two are only to display values temporarily
        ammoLeftInClip = ammoUsage.GetAmmoRemainingInClip();
        ammoLeftInTotal = ammoUsage.GetAmmoRemainingInTotal();

        if (Input.GetKeyDown(KeyCode.R)) {
            if (ammoUsageType == AmmoUsageType.Standard && ammoUsage.CanReload()) {
                ammoUsage.StartReload();
            }
        }
        ammoUsage.OnUpdate();
    }

    public override void InitializeSavedData(IAmmoUsage cachedWeaponData) {
        ammoUsage = cachedWeaponData;
        ammoUsage.ApplyTimeAtWeaponSwap();
    }

    public override IAmmoUsage GetMutableData() {
        ammoUsage.SaveTimeAtWeaponSwap();
        return ammoUsage;
    }

    public override void ActivateWeapon() {
        shootingMechanism.Shoot();
    }

    #region HANDLE MUZZLE FLASH
    public void InstantiateMuzzleFlash() {
        muzzleFlash = Instantiate(
            weaponData.muzzleFlashPrefab,
            firePoint.position,
            firePoint.rotation,
            tempWeaponPosition);

        muzzleFlash.GetComponent<Animator>().runtimeAnimatorController = weaponData.muzzleFlashAnimation;
        muzzleFlash.SetActive(false);
    }

    public void MuzzleFlash() {
        muzzleFlash.SetActive(true);
        Invoke("MuzzleFlashOff", muzzleFlash.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
    }

    void MuzzleFlashOff() {
        muzzleFlash.SetActive(false);
    }
    #endregion
}
