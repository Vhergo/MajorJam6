﻿using System.Collections;
using System.Net;
using UnityEngine;

public class BeamShootingMechanism : IShootingMechanism
{
    private RangeWeaponLogic weaponLogic;
    private bool inputReleased;

    private Coroutine beamCoroutine;
    private Coroutine activationCoroutine;
    private Coroutine deactivationCoroutine;

    private float beamProgress;
    private LayerMask layerMask;
    private float fireRateTimer;

    public BeamShootingMechanism(RangeWeaponLogic logic) {
        weaponLogic = logic;
        inputReleased = true;
        
        layerMask = LayerExclusion();
    }

    public void Shoot() {
        if (inputReleased && weaponLogic.ammoUsage.CanShoot()) {
            StopAllCoroutines();
            beamCoroutine = CoroutineHandler.Instance.StartManagedCoroutine(HandleBeam());
        }
    }

    private IEnumerator HandleBeam() {
        weaponLogic.beamRenderer.enabled = true;

        float startTime = Time.time;
        inputReleased = false;

        StartActivationCoroutine();

        yield return new WaitUntil(() => {
            return (beamProgress >= 1 || inputReleased);
        });

        if (inputReleased) {
            StartDeactivationCoroutine();
            yield break;
        }

        while (!Input.GetKeyUp(weaponLogic.fireKey) && weaponLogic.ammoUsage.CanShoot()) {
            BeamMaintain();
            ConsumeAmmo();
            yield return null;
        }

        StartDeactivationCoroutine();
        inputReleased = true;
    }

    private IEnumerator BeamActivationCoroutines() {
        Vector2 endPoint;
        float startTime = Time.time - (beamProgress * weaponLogic.activationDuration);

        while (beamProgress < 1) {
            if (!Input.GetKey(weaponLogic.fireKey)) {
                inputReleased = true;
                yield break;
            }

            float elapsedTime = Time.time - startTime;
            beamProgress = elapsedTime / weaponLogic.activationDuration;
            endPoint = RaycastTargetPoint(weaponLogic.beamRange * beamProgress);
            UpdateBeamEndPosition(endPoint);

            yield return null;
        }
    }

    private void BeamMaintain() {
        Vector2 endPoint = RaycastTargetPoint(weaponLogic.beamRange);
        UpdateBeamEndPosition(endPoint);
    }

    private void ConsumeAmmo() {
        fireRateTimer -= Time.deltaTime;
        if (fireRateTimer <= 0) {
            weaponLogic.ammoUsage.OnShoot();
            fireRateTimer = weaponLogic.fireRate;
        }
    }

    private IEnumerator BeamDeactivationCoroutines() {
        Vector2 endPoint = Vector2.zero;
        float startTime = Time.time - ((1 - beamProgress) * weaponLogic.deactivationDuration);

        while (beamProgress > 0) { 
            float elapsedTime = Time.time - startTime;
            beamProgress = 1 - (elapsedTime / weaponLogic.deactivationDuration);
            endPoint = RaycastTargetPoint(weaponLogic.beamRange * (beamProgress));
            UpdateBeamEndPosition(endPoint);

            yield return null;
        }

        weaponLogic.beamRenderer.enabled = false;
    }

    private Vector2 RaycastTargetPoint(float range) {
        RaycastHit2D hit = Physics2D.Raycast(GetBeamStartPoint(), weaponLogic.firePoint.right, range, layerMask);
        if (hit.collider != null) {
            return hit.point;
        }else {
            return GetBeamStartPoint() + weaponLogic.firePoint.right * range;
        }
    }

    private int LayerExclusion() {
        int layerMask = 0;
        layerMask |= 1 << LayerMask.NameToLayer("Player");
        layerMask |= 1 << LayerMask.NameToLayer("CameraConfiner");
        return ~layerMask;
    }

    private Vector3 GetBeamStartPoint() {
        return weaponLogic.firePoint.position;
    }

    private void UpdateBeamEndPosition(Vector3 endPoint) {
        weaponLogic.beamRenderer.SetPosition(0, GetBeamStartPoint());
        weaponLogic.beamRenderer.SetPosition(1, endPoint);
    }

    public void StopShoot() {
        beamProgress = 0;
        inputReleased = true;
        weaponLogic.beamRenderer.enabled = false;

        StopAllCoroutines();
    }

    private void StopAllCoroutines() {
        StopBeamCoroutine();
        StopActivationCoroutine();
        StopDeactivationCoroutine();
    }

    private void StopBeamCoroutine() {
        if (beamCoroutine != null) CoroutineHandler.Instance.StopManagedCoroutine(beamCoroutine);
    }

    private void StopActivationCoroutine() {
        if (activationCoroutine != null) CoroutineHandler.Instance.StopManagedCoroutine(activationCoroutine);
    }

    private void StopDeactivationCoroutine() {
        if (deactivationCoroutine != null) CoroutineHandler.Instance.StopManagedCoroutine(deactivationCoroutine);
    }

    private void StartActivationCoroutine() {
        activationCoroutine = CoroutineHandler.Instance.StartManagedCoroutine(BeamActivationCoroutines());
    }

    private void StartDeactivationCoroutine() {
        deactivationCoroutine = CoroutineHandler.Instance.StartManagedCoroutine(BeamDeactivationCoroutines());
    }
}