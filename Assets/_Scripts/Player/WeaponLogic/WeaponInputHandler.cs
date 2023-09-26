using System.Collections;
using UnityEngine;

public class WeaponInputHandler : MonoBehaviour
{
    private WeaponLogic currentWeaponLogic;
    private Coroutine beamCoroutine;

    private void OnEnable() {
        WeaponManager.OnWeaponEquipped += UpdateWeaponLogic;
    }

    private void OnDisable() {
        WeaponManager.OnWeaponEquipped -= UpdateWeaponLogic;
    }

    private void Update() {
        HandleInput();
    }

    private void UpdateWeaponLogic(WeaponLogic newWeaponLogic) {
        currentWeaponLogic = newWeaponLogic;
    }

    private void HandleInput() {
        if (currentWeaponLogic is RangeWeaponLogic rangeWeaponLogic) {
            switch (rangeWeaponLogic.weaponData.firingType) {
                case RangeFiringType.SemiAuto:
                    TapFireInput(rangeWeaponLogic);
                    break;
                case RangeFiringType.FullAuto:
                    HoldFireInput(rangeWeaponLogic);
                    break;
                case RangeFiringType.Charge:
                    ChargeFireInput(rangeWeaponLogic);
                    break;
                case RangeFiringType.Beam:
                    BeamFireInput(rangeWeaponLogic);
                    break;
            }
        } else if (currentWeaponLogic is MeleeWeaponLogic meleeWeaponLogic) {
            // Similar structure for melee input handling
        }
    }

    protected void TapFireInput(RangeWeaponLogic weaponLogic) {
        if (Input.GetKeyDown(weaponLogic.fireKey) && weaponLogic.fireRateTimer >= weaponLogic.fireRate) {
            weaponLogic.ActivateWeapon();
        } else {
            weaponLogic.fireRateTimer += Time.deltaTime;
        }
    }

    protected void HoldFireInput(RangeWeaponLogic weaponLogic) {
        if (Input.GetKey(weaponLogic.fireKey) && weaponLogic.fireRateTimer >= weaponLogic.fireRate) {
            weaponLogic.ActivateWeapon();
        } else {
            weaponLogic.fireRateTimer += Time.deltaTime;
        }
    }

    protected void ChargeFireInput(RangeWeaponLogic weaponLogic) {
        weaponLogic.fireRateTimer += Time.deltaTime;

        if (weaponLogic.fireRateTimer < weaponLogic.fireRate) {
            weaponLogic.chargeTimer = Time.time;
            return;
        }

        if (Input.GetKeyDown(weaponLogic.fireKey)) {
            weaponLogic.chargeTimer = Time.time;
        }

        if (Input.GetKey(weaponLogic.fireKey)) {
            float chargeDuration = Time.time - weaponLogic.chargeTimer;
            float chargeLevel = Mathf.Clamp(chargeDuration / weaponLogic.chargeTime, 0f, 1f);

            weaponLogic.finalDamage = Mathf.Lerp(weaponLogic.damage.minDamage, weaponLogic.damage.maxDamage, chargeLevel);

            if (weaponLogic.chargeAutoFire && chargeLevel >= 1f) {
                StartCoroutine(DelayedActivateRangeWeapon(weaponLogic));
            }
        }

        if (Input.GetKeyUp(weaponLogic.fireKey)) {
            weaponLogic.ActivateWeapon();
        }
    }

    protected void BeamFireInput(RangeWeaponLogic weaponLogic) {
        if (Input.GetKey(weaponLogic.fireKey)) {
            weaponLogic.ActivateWeapon();
        }
    }

    private IEnumerator DelayedActivateRangeWeapon(RangeWeaponLogic weaponLogic) {
        yield return new WaitForSeconds(weaponLogic.chargeAutoFireDelay);
        weaponLogic.ActivateWeapon();
    }
}