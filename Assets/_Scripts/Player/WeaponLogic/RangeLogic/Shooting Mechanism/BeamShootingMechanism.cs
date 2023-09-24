using System.Collections;
using UnityEngine;

public class BeamShootingMechanism : IShootingMechanism
{
    private readonly RangeWeaponDataSO weaponData;

    public BeamShootingMechanism(RangeWeaponDataSO rangeWeaponData) {
        weaponData = rangeWeaponData;
    }

    public IEnumerator ShootBeam(IAmmoUsage ammoUsage, KeyCode fireKey) {
        float startTime = Time.time; ;
        bool inputReleased = false;

        // Activation Logic Here

        yield return new WaitUntil(() => {
            inputReleased = Input.GetKeyUp(fireKey);
            return (Time.time - startTime >= weaponData.activationDuration || inputReleased);
        });

        if (inputReleased) {
            // Deactivation Logic Here
            yield break;
        }

        while (!Input.GetKeyUp(fireKey) && ammoUsage.CanShoot()) {
            // Maintain Logic Here
        }

        // Deactivation Logic Here
        yield return new WaitForSeconds(weaponData.deactivationDuration);
    }

    public void ShootBullet(RangeWeaponLogic weaponLogic) { }
}