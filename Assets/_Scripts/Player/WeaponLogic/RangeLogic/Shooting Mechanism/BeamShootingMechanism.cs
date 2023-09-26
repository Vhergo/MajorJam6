using System.Collections;
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

    public BeamShootingMechanism(RangeWeaponLogic logic) {
        weaponLogic = logic;
        inputReleased = true;
        
        layerMask = LayerExclusion();
    }

    public void Shoot() {
        if (inputReleased) {
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
            yield return null;
        }

        StartDeactivationCoroutine();
        inputReleased = true;
    }

    private IEnumerator BeamActivation() {
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

    private IEnumerator BeamDeactivation() {
        Debug.Log("Deactivate");
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
        Debug.DrawRay(GetBeamStartPoint(), weaponLogic.firePoint.right * range, Color.red, 1f);
        if (hit.collider != null) {
            DrawCrosshair(hit.point);
            return hit.point;
        }else {
            DrawCrosshair(GetBeamStartPoint() + weaponLogic.firePoint.right * range);
            return GetBeamStartPoint() + weaponLogic.firePoint.right * range;
        }
    }

    private void DrawCrosshair(Vector2 point) {
        float crosshairSize = 1f;
        Vector2 verticalStart = point + Vector2.up * crosshairSize;
        Vector2 verticalEnd = point - Vector2.up * crosshairSize;
        Vector2 horizontalStart = point + Vector2.right * crosshairSize;
        Vector2 horizontalEnd = point - Vector2.right * crosshairSize;

        Debug.DrawLine(verticalStart, verticalEnd, Color.green);
        Debug.DrawLine(horizontalStart, horizontalEnd, Color.green);
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
        weaponLogic.beamRenderer.enabled = false;

        StopBeamCoroutine();
        StopActivationCoroutine();
        StopDeactivationCoroutine();
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
        activationCoroutine = CoroutineHandler.Instance.StartManagedCoroutine(BeamActivation());
    }

    private void StartDeactivationCoroutine() {
        deactivationCoroutine = CoroutineHandler.Instance.StartManagedCoroutine(BeamDeactivation());
    }
}