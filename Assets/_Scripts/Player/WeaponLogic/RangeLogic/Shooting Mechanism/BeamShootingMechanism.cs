using System.Collections;
using System.Net;
using UnityEngine;

public class BeamShootingMechanism : IShootingMechanism
{
    private RangeWeaponLogic weaponLogic;
    private bool inputReleased;

    private Coroutine beamCoroutine;
    private Coroutine activationCoroutine;
    public Coroutine deactivationCoroutine;

    private float beamProgress;

    public BeamShootingMechanism(RangeWeaponLogic logic) {
        weaponLogic = logic;
        inputReleased = true;
    }

    public void Shoot() {
        if (inputReleased)
            beamCoroutine = CoroutineHandler.Instance.StartManagedCoroutine(HandleBeam());
    }

    private IEnumerator HandleBeam() {
        Debug.Log("Input Key: " + weaponLogic.fireKey);
        float startTime = Time.time;
        inputReleased = false;

        // Activation Logic Here
        activationCoroutine = CoroutineHandler.Instance.StartManagedCoroutine(BeamActivation());

        yield return new WaitUntil(() => {
            inputReleased = Input.GetKeyUp(weaponLogic.fireKey);
            return (Time.time - startTime >= weaponLogic.activationDuration || inputReleased);
        });

        if (inputReleased) {
            // Deactivation Logic Here
            deactivationCoroutine = CoroutineHandler.Instance.StartManagedCoroutine(BeamDeactivation());
            inputReleased = true;
            yield break;
        }

        while (!Input.GetKeyUp(weaponLogic.fireKey) && weaponLogic.ammoUsage.CanShoot()) {
            // Maintain Logic Here
            BeamMaintain();
            yield return null;
        }

        // Deactivation Logic Here
        deactivationCoroutine = CoroutineHandler.Instance.StartManagedCoroutine(BeamDeactivation());
        inputReleased = true;
    }

    private IEnumerator BeamActivation() {
        weaponLogic.beamRenderer.enabled = true;

        Vector2 endPoint;
        float startTime = Time.time - (beamProgress * weaponLogic.activationDuration);

        while (beamProgress < 1) {
            if (!Input.GetKey(weaponLogic.fireKey)) yield break;

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
            if (Input.GetKeyDown(weaponLogic.fireKey)) {
                BeamReactivation();
                yield break;
            }   
            float elapsedTime = Time.time - startTime;
            beamProgress = 1 - (elapsedTime / weaponLogic.deactivationDuration);
            endPoint = RaycastTargetPoint(weaponLogic.beamRange * (beamProgress));
            UpdateBeamEndPosition(endPoint);

            yield return null;
        }
        UpdateBeamEndPosition((Vector3)endPoint);
        weaponLogic.beamRenderer.enabled = false;
    }

    private void BeamReactivation() {
        Debug.Log("Reactivate");
        StopShoot();
        beamCoroutine = CoroutineHandler.Instance.StartManagedCoroutine(HandleBeam());
    }

    private Vector2 RaycastTargetPoint(float range) {
        RaycastHit2D hit = Physics2D.Raycast(GetBeamStartPoint(), weaponLogic.firePoint.right, range, LayerExclusion());
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
        Debug.Log("Stop all active coroutines");
        StopBeamCoroutine();
        StopActivationCoroutine();
        StopDeactivationCoroutine();
    }

    public void StopBeamCoroutine() {
        if (beamCoroutine != null) CoroutineHandler.Instance.StopManagedCoroutine(beamCoroutine);
        weaponLogic.beamRenderer.enabled = false;
    }

    public void StopActivationCoroutine() {
        if (activationCoroutine != null) CoroutineHandler.Instance.StopManagedCoroutine(activationCoroutine);
        weaponLogic.beamRenderer.enabled = false;
    }

    public void StopDeactivationCoroutine() {
        if (deactivationCoroutine != null) CoroutineHandler.Instance.StopManagedCoroutine(deactivationCoroutine);
        weaponLogic.beamRenderer.enabled = true;
    }
}