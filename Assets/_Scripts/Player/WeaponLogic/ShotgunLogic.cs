using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunLogic : WeaponLogic
{
    protected ShotgunDataSO shotgunData;
    protected float arcBulletSpread;
    protected float arcBulletCount;

    void Update() {
        if (Input.GetKeyDown(fireKey)) {
            if (fireRateTimer >= fireRate) {
                Shoot();
                fireRateTimer = 0;
            }
        }else {
            fireRateTimer += Time.deltaTime;
        }
    }

    public override void InitializeWeaponData(WeaponDataSO weapon, Transform gunBarrel, GameObject muzzleFlashObject, KeyCode fireKey) {
        base.InitializeWeaponData(weapon, gunBarrel, muzzleFlashObject, fireKey);
        shotgunData = weapon as ShotgunDataSO;
        arcBulletSpread = shotgunData.arcBulletSpread;
        arcBulletCount = shotgunData.arcBulletCount;
    }

    public override void Shoot() {
        print("SHOTGUN SHOOT");
        for (int i = 0; i < arcBulletCount; i++) {
            Quaternion newRot = CalculateBulletSpread(i);
            Vector2 fireDirection = newRot * Vector2.right;

            GameObject bullet = Instantiate(bulletPrefab, gunBarrel.position, newRot);
            Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();
            bulletRB.velocity = fireDirection * bulletSpeed;
            SetBulletValues(bullet);
        }

        SelfKnockback(knockback);
        MuzzleFlash();
    }

    protected Quaternion CalculateBulletSpread(int i) {
        float fireAngle = (arcBulletSpread / arcBulletCount) * (i - (arcBulletCount - 1) / 2);
        Quaternion newRot = Quaternion.Euler(gunBarrel.eulerAngles.x, gunBarrel.eulerAngles.y, gunBarrel.eulerAngles.z + fireAngle);
        return newRot;
    }
}
