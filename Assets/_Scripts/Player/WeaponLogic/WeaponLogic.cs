using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponLogic : MonoBehaviour
{
    protected WeaponDataSO weaponData;
    protected float damage;
    protected float bulletSpeed;
    protected float fireRate;
    protected float knockback;
    protected float selfKnockback;

    protected GameObject bulletPrefab;
    protected RuntimeAnimatorController muzzleFlash;
    protected Transform gunBarrel;
    protected KeyCode fireKey;

    protected float fireRateTimer;

    protected PlayerMovement player;
    protected GameObject muzzleFlashObject;

    public virtual void InitializeWeaponData(
        WeaponDataSO weapon, 
        Transform gunBarrelPos, 
        GameObject muzzleFlashGameObject,
        KeyCode fireKeyCode) {

        weaponData = weapon;
        damage = weaponData.damage;
        bulletSpeed = weaponData.bulletSpeed;
        fireRate = weaponData.fireRate;
        knockback = weaponData.knockback;
        selfKnockback = weaponData.selfKnockback;

        bulletPrefab = weaponData.bulletPrefab;
        muzzleFlash = weaponData.muzzleFlash;
        gunBarrel = gunBarrelPos;

        fireKey = fireKeyCode;
        fireRateTimer = fireRate;

        muzzleFlashObject = muzzleFlashGameObject;
        muzzleFlashObject.GetComponent<Animator>().runtimeAnimatorController = muzzleFlash;
    }

    //public abstract void Trigger();
    public abstract void Shoot();

    public virtual void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    protected Vector2 GetShootingDirection() {
        return (gunBarrel.position + gunBarrel.right) - gunBarrel.position;
    }

    public virtual void SetBulletValues(GameObject bullet) {
        PlayerBullet playerBullet = bullet.GetComponent<PlayerBullet>();
        playerBullet.BulletDamage = damage;
        playerBullet.BulletSpeed = bulletSpeed;
        playerBullet.KnockbackForce = knockback;
    }

    protected void SelfKnockback(float knockbackForce) {
        Vector2 knockbackDirection = (transform.position - gunBarrel.position).normalized;
        player.rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
    }

    protected void MuzzleFlash() {
        muzzleFlashObject.SetActive(true);
        Invoke("MuzzleFlashOff", muzzleFlashObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
    }

    void MuzzleFlashOff() {
        muzzleFlashObject.SetActive(false);
    }
}
