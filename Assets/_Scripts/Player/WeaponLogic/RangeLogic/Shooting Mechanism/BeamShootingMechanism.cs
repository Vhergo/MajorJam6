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
    private Vector2 endPoint;

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
        SetBeamRotationSpeed();

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
            HandleBeamInteractions();
            yield return null;
        }

        StartDeactivationCoroutine();
        inputReleased = true;
    }

    private IEnumerator BeamActivationCoroutines() {
        float startTime = Time.time - (beamProgress * weaponLogic.activationDuration);

        while (beamProgress < 1) {
            if (!Input.GetKey(weaponLogic.fireKey)) {
                inputReleased = true;
                yield break;
            }

            float elapsedTime = Time.time - startTime;
            beamProgress = elapsedTime / weaponLogic.activationDuration;
            endPoint = RaycastTargetPoint(weaponLogic.beamRange * beamProgress);
            UpdateBeamEndPosition();

            yield return null;
        }
    }

    private IEnumerator BeamDeactivationCoroutines() {
        endPoint = Vector2.zero;
        float startTime = Time.time - ((1 - beamProgress) * weaponLogic.deactivationDuration);

        while (beamProgress > 0) {
            float elapsedTime = Time.time - startTime;
            beamProgress = 1 - (elapsedTime / weaponLogic.deactivationDuration);
            endPoint = RaycastTargetPoint(weaponLogic.beamRange * (beamProgress));
            UpdateBeamEndPosition();

            yield return null;
        }

        weaponLogic.beamRenderer.enabled = false;
        ResetBeamRotationSpeed();
    }

    private void BeamMaintain() {
        endPoint = RaycastTargetPoint(weaponLogic.beamRange);
        UpdateBeamEndPosition();
    }

    private void HandleBeamInteractions() {
        fireRateTimer -= Time.deltaTime;
        if (fireRateTimer <= 0) {
            ConsumeAmmo();
            DealDamage();
        }
    }

    private void ConsumeAmmo() {
        if (!weaponLogic.ammoUsage.CanShoot()) return;

        weaponLogic.ammoUsage.OnShoot();
        fireRateTimer = weaponLogic.fireRate;

        SoundManager.Instance.PlaySound(weaponLogic.firingSound);
    }

    private void DealDamage() {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(endPoint, weaponLogic.beamDetectionRadius);
        if (enemies.Length == 0) return;

        if (weaponLogic.singleTarget) {
            Collider2D nearestEnemy = null;
            float nearestSquaredDistance = float.MaxValue;
            foreach (Collider2D enemy in enemies) {
                if ((!enemy.CompareTag("Enemy") || enemy.GetComponent<Enemy>() == null)) continue;
                float squaredDistance = (endPoint - (Vector2)enemy.gameObject.transform.position).sqrMagnitude;
                if (squaredDistance < nearestSquaredDistance) {
                    nearestSquaredDistance = squaredDistance;
                    nearestEnemy = enemy;
                }
            }

            if (nearestEnemy != null) {
                nearestEnemy.GetComponent<Enemy>().TakeDamage(weaponLogic.finalDamage);
                ApplyKnockback(nearestEnemy.gameObject);
            }
        }else {
            foreach (Collider2D enemy in enemies) {
                if (enemy.CompareTag("Enemy") && enemy.GetComponent<Enemy>() != null) {
                    enemy.GetComponent<Enemy>().TakeDamage(weaponLogic.finalDamage);
                    ApplyKnockback(enemy.gameObject);
                }
            }
        }
    }

    private void ApplyKnockback(GameObject enemy) {
        Vector2 knockbackDirection = (enemy.transform.position - weaponLogic.transform.position).normalized;
        enemy.GetComponent<Rigidbody2D>().AddForce(knockbackDirection * weaponLogic.knockback, ForceMode2D.Impulse);
    }

    private Vector2 RaycastTargetPoint(float range) {
        RaycastHit2D hit = Physics2D.Raycast(GetBeamStartPoint(), weaponLogic.firePoint.right, range, layerMask);
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (hit.collider != null) {
            if (weaponLogic.limitRange) {
                float distanceToMouse = ((Vector2)GetBeamStartPoint() - mousePos).sqrMagnitude;
                if (distanceToMouse < (hit.distance * hit.distance)) {
                    return GetMouseEndPoint(distanceToMouse);
                }
                return hit.point;
            }
            return hit.point;
        } else {
            if (weaponLogic.limitRange) {
                // Distance optimization
                // float distance = Vector2.Distance(GetBeamStartPoint(), Camera.main.ScreenToWorldPoint(Input.mousePosition));
                // Need to typecast both to Vector2 to use sqrMagnitude in a 2D context. Otherwise the Z axis will skew the results
                float distanceToMouse = ((Vector2)GetBeamStartPoint() - mousePos).sqrMagnitude;
                if (distanceToMouse < (weaponLogic.beamRange * weaponLogic.beamRange)) {
                    float distanceToBeamEnd = (GetBeamStartPoint() - (GetCurrentBeamEndPoint(range))).sqrMagnitude;
                    if (distanceToBeamEnd < distanceToMouse) {
                        return GetCurrentBeamEndPoint(range);
                    }
                    return GetMouseEndPoint(distanceToMouse);
                }
            }
            return GetCurrentBeamEndPoint(range);
        }
    }

    private int LayerExclusion() {
        int layerMask = 0;
        layerMask |= 1 << LayerMask.NameToLayer("Player");
        layerMask |= 1 << LayerMask.NameToLayer("CameraConfiner");
        return ~layerMask;
    }

    private Vector2 GetBeamStartPoint() {
        return weaponLogic.firePoint.position;
    }

    private Vector2 GetCurrentBeamEndPoint(float range) {
        return GetBeamStartPoint() + (Vector2)weaponLogic.firePoint.right * range;
    }

    private Vector2 GetMouseEndPoint(float distanceToMouse) {
        return GetBeamStartPoint() + (Vector2)weaponLogic.firePoint.right * Mathf.Sqrt(distanceToMouse);
    }

    private void UpdateBeamEndPosition() {
        weaponLogic.beamRenderer.SetPosition(0, GetBeamStartPoint());
        weaponLogic.beamRenderer.SetPosition(1, endPoint);
    }

    private void SetBeamRotationSpeed() {
        RotateWithMouse.Instance.rotationSpeed = weaponLogic.beamRotationSpeed;
    }

    private void ResetBeamRotationSpeed() {
        RotateWithMouse.Instance.ResetRotationSpeed();
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