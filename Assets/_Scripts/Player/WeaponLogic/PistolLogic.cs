using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolLogic : WeaponLogic
{
    protected PistolDataSO pistolData;
    protected float healAmount;

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
        pistolData = weapon as PistolDataSO;
        healAmount = pistolData.healAmount;
    }

    public override void SetBulletValues(GameObject bullet) {
        base.SetBulletValues(bullet);
        PlayerBullet playerBullet = bullet.GetComponent<PlayerBullet>();
        playerBullet.HealAmount = healAmount;
    }

    public override void Shoot() {
        print("PISTOL SHOOT");
        GameObject bullet = Instantiate(bulletPrefab, gunBarrel.position, gunBarrel.rotation);
        Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();
        bulletRB.velocity = GetShootingDirection() * bulletSpeed;
        SetBulletValues(bullet);

        SelfKnockback(knockback);
        MuzzleFlash();
    }
}
