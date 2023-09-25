using System.Collections;
using System.Net;
using UnityEngine;

public class BeamShootingMechanism : IShootingMechanism
{
    private readonly RangeWeaponDataSO weaponData;
    private RangeWeaponLogic weaponLogic;
    private bool inputReleased;

    private Coroutine beamCoroutine;
    private Coroutine activationCoroutine;
    private Coroutine deactivationCoroutine;

    public BeamShootingMechanism(RangeWeaponDataSO data, RangeWeaponLogic logic) {
        weaponData = data;
        weaponLogic = logic;
        inputReleased = true;
    }

    public void Shoot(IAmmoUsage ammoUsage, KeyCode fireKey) {
        if (inputReleased)
            beamCoroutine = CoroutineHandler.Instance.StartManagedCoroutine(HandleBeam(ammoUsage, fireKey));
    }

    public void StopShoot() {
        weaponLogic.beamRenderer.enabled = false;

        if (beamCoroutine != null) CoroutineHandler.Instance.StopManagedCoroutine(beamCoroutine);
        if (activationCoroutine != null) CoroutineHandler.Instance.StopManagedCoroutine(activationCoroutine);
        if (deactivationCoroutine != null) CoroutineHandler.Instance.StopManagedCoroutine(deactivationCoroutine);
    }

    private IEnumerator HandleBeam(IAmmoUsage ammoUsage, KeyCode fireKey) {
        float startTime = Time.time;
        inputReleased = false;

        // Activation Logic Here
        activationCoroutine = CoroutineHandler.Instance.StartManagedCoroutine(BeamActivation());

        yield return new WaitUntil(() => {
            inputReleased = Input.GetKeyUp(fireKey);
            return (Time.time - startTime >= weaponData.activationDuration || inputReleased);
        });

        if (inputReleased) {
            // Deactivation Logic Here
            deactivationCoroutine = CoroutineHandler.Instance.StartManagedCoroutine(BeamDeactivation());
            inputReleased = true;
            yield break;
        }

        while (!Input.GetKeyUp(fireKey) && ammoUsage.CanShoot()) {
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
        float startTime = Time.time;
        float progress = 0;

        while (progress < 1) {
            float elapsedTime = Time.time - startTime;
            progress = elapsedTime / weaponData.activationDuration;
            endPoint = RaycastTargetPoint(weaponLogic.beamRange * progress);
            UpdateBeamEndPosition(endPoint);

            yield return null;
        }
    }

    private void BeamMaintain() {
        Vector2 endPoint = RaycastTargetPoint(weaponLogic.beamRange);
        UpdateBeamEndPosition(endPoint);
    }

    private IEnumerator BeamDeactivation() {
        Vector2 endPoint = Vector2.zero;

        float startTime = Time.time;
        float progress = 0;

        while (progress < 1) {
            float elapsedTime = Time.time - startTime;
            progress = elapsedTime / weaponData.deactivationDuration;
            endPoint = RaycastTargetPoint(weaponLogic.beamRange * (1 - progress));
            UpdateBeamEndPosition(endPoint);

            yield return null;
        }
        UpdateBeamEndPosition((Vector3)endPoint);
        weaponLogic.beamRenderer.enabled = false;
    }

    private Vector2 RaycastTargetPoint(float range) {
        RaycastHit2D hit = Physics2D.Raycast(GetBeamStartPoint(), weaponLogic.firePoint.right, range, LayerExclusion());
        // Debug.DrawRay(GetBeamStartPoint(), weaponLogic.firePoint.right * range, Color.red, 1f);
        if (hit.collider != null) {
            DrawCrosshair(hit.point);
            Debug.Log("Hit: " + hit.point);
            Debug.Log("The Raycast hit " + hit.collider.gameObject.name);
            return hit.point;
        }else {
            DrawCrosshair(GetBeamStartPoint() + weaponLogic.firePoint.right * range);
            Debug.Log("NO hit: " + GetBeamStartPoint() + weaponLogic.firePoint.right * range);
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
}