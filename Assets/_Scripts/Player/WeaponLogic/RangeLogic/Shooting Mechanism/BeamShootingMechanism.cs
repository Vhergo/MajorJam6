using System.Collections;
using UnityEngine;

public class BeamShootingMechanism : IShootingMechanism
{
    private readonly RangeWeaponDataSO weaponData;
    private bool inputReleased;
    private Coroutine beamCoroutine;

    public BeamShootingMechanism(RangeWeaponDataSO rangeWeaponData) {
        weaponData = rangeWeaponData;
        inputReleased = true;
    }

    public void Shoot(IAmmoUsage ammoUsage, KeyCode fireKey) {
        if (inputReleased)
            beamCoroutine = CoroutineHandler.Instance.StartManagedCoroutine(HandleBeam(ammoUsage, fireKey));
    }

    public void StopShoot() {
        CoroutineHandler.Instance.StopManagedCoroutine(beamCoroutine);
    }

    private IEnumerator HandleBeam(IAmmoUsage ammoUsage, KeyCode fireKey) {
        float startTime = Time.time; ;
        inputReleased = false;

        // Activation Logic Here
        BeamActivation();

        yield return new WaitUntil(() => {
            inputReleased = Input.GetKeyUp(fireKey);
            return (Time.time - startTime >= weaponData.activationDuration || inputReleased);
        });

        if (inputReleased) {
            // Deactivation Logic Here
            BeamDeactivation();
            yield break;
        }

        while (!Input.GetKeyUp(fireKey) && ammoUsage.CanShoot()) {
            // Maintain Logic Here
            BeamMaintain();
        }

        // Deactivation Logic Here
        BeamDeactivation();
        yield return new WaitForSeconds(weaponData.deactivationDuration);
        inputReleased = true;
    }

    private void BeamActivation() {

    }

    private void BeamMaintain() {

    }

    private void BeamDeactivation() {

    }
}