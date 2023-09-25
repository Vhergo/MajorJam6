using log4net.Util;
using System.Collections;
using UnityEngine;

public class BulletShootingMechanism : IShootingMechanism
{
    private readonly RangeWeaponDataSO weaponData;
    private RangeWeaponLogic weaponLogic;
    private Coroutine burstCoroutine;

    public BulletShootingMechanism(RangeWeaponDataSO data, RangeWeaponLogic logic) {
        weaponData = data;
        weaponLogic = logic;
    }

    public void StopShoot() {
        CoroutineHandler.Instance.StopManagedCoroutine(burstCoroutine);
    }

    // Arguments aren't needed for the bullet mechanism
    // Need to include them because of the interface
    public void Shoot(IAmmoUsage ammoUsage, KeyCode fireKey) {
        if (!weaponLogic.ammoUsage.CanShoot()) return;

        if (weaponData.burstEnabled) {
            burstCoroutine = CoroutineHandler.Instance.StartManagedCoroutine(ShootBurst());
        }else {
            ShootBullet();
        }

        weaponLogic.fireRateTimer = 0;
        weaponLogic.ammoUsage.OnShoot();
    }

    private void ShootBullet() {
        for (int i = 0; i < weaponLogic.bulletsPerShot; i++) {
            FireBullet(CalculateSpread(i));
        }

        SelfKnockback(weaponLogic.selfKnockback);
        weaponLogic.MuzzleFlash();
    }

    private IEnumerator ShootBurst() {
        for (int i = 0; i < weaponLogic.burstCount; i++) {
            ShootBullet();
            yield return new WaitForSeconds(weaponLogic.burstRate);
        }
    }

    private void FireBullet(Quaternion spreadAngle) {
        GameObject bullet = Object.Instantiate(weaponLogic.bulletPrefab, weaponLogic.firePoint.position, spreadAngle);
        Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();
        bulletRB.velocity = GetShootingDirection() * weaponLogic.bulletSpeed;
        SetBulletValues(bullet);
    }

    public virtual void SetBulletValues(GameObject bullet) {
        PlayerBullet playerBullet = bullet.GetComponent<PlayerBullet>();
        playerBullet.BulletDamage = weaponLogic.finalDamage;
        playerBullet.BulletSpeed = weaponLogic.bulletSpeed;
        playerBullet.KnockbackForce = weaponLogic.knockback;
    }

    private Vector2 GetShootingDirection() {
        return weaponLogic.firePoint.right;
    }

    private Quaternion CalculateSpread(int i) {
        float fireAngle;
        if (weaponLogic.symmetricalSpread) {
            fireAngle = (-weaponLogic.bulletSpread / 2) + (weaponLogic.bulletsPerShot * i);
        } else {
            float adjustedBulletSpread = weaponLogic.bulletSpread / 2;
            fireAngle = Random.Range(-adjustedBulletSpread, adjustedBulletSpread);
        }

        Quaternion newRot = Quaternion.Euler(
            weaponLogic.firePoint.eulerAngles.x,
            weaponLogic.firePoint.eulerAngles.y,
            weaponLogic.firePoint.eulerAngles.z + fireAngle);

        return newRot;
    }

    public void SelfKnockback(float knockbackForce) {
        Vector2 knockbackDirection = (weaponLogic.transform.position - weaponLogic.firePoint.position).normalized;
        weaponLogic.player.rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
    }

    public IEnumerator ShootBeam(IAmmoUsage ammoUsage, KeyCode fireKey) {
        yield return null;
    }
}