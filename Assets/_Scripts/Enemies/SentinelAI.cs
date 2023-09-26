using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentinelAI : Enemy
{
    [Header("Attack")]
    [SerializeField] private GameObject sentinelBulletPrefab;
    [SerializeField] private float bulletSpawnOffset; // offset for attack spawn position
    [SerializeField] private float bulletDamage;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletFireRate;
    [SerializeField] private float initialBulletSpawnDelay;
    [Tooltip("Delay until the next orb is summoned")] [SerializeField] private float launchDelayOffset;

    [Header("Toggles")]
    [Tooltip("Need to toggle before runtime")] [SerializeField] private bool canFire;

    private SentinelAnimation anim;
    GameObject latestBullet;

    protected override void Start(){
        base.Start();
        if (canFire) TriggerAttack();
        anim = GetComponent<SentinelAnimation>();
    }

    private void Update() {
        Flip();
    }

    public override void TakeDamage(float damageAmount) {
        currentHealth -= damageAmount;
        if (currentHealth <= 0) {
            EnemyDeath();
        }else {
            anim.PlayHurtAnim();
        }
    }

    void EnemyDeath() {
        CancelAttack();
        ObjectPermanence();
        anim.PlayDeathAnim();
        Destroy(gameObject, deathDelay);
    }

    void TriggerAttack() {
        // Subtract launch delay to account for the extra wait time
        InvokeRepeating("LaunchAttack", initialBulletSpawnDelay, bulletFireRate);
    }

    void LaunchAttack() {
        Invoke("DelayAttackAnim", bulletFireRate - launchDelayOffset);
        latestBullet = Instantiate(sentinelBulletPrefab, GetAttackSpawnPosition(), Quaternion.identity);
        SentinelBullet bullet = latestBullet.GetComponent<SentinelBullet>();
        bullet.BulletDamage = bulletDamage;
        bullet.BulletSpeed = bulletSpeed;
        bullet.LaunchDelay = bulletFireRate - launchDelayOffset;
    }

    void CancelAttack() {
        CancelInvoke("LaunchAttack");
        CancelInvoke("DelayAttackAnim");
        if (latestBullet != null) {
            if (!latestBullet.GetComponent<SentinelBullet>().CanMove) Destroy(latestBullet);
        }
    }

    void DelayAttackAnim() {
        anim.PlayAttackAnim();
    }

    void Flip() {
        if (target.position.x > transform.position.x) {
            transform.localScale = new Vector3(1, 1, 1);
        }else if (target.position.x < transform.position.x) {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void ObjectPermanence() {
        // Instantiate permanence object
    }

    Vector3 GetAttackSpawnPosition() {
        return new Vector3(transform.position.x, transform.position.y + bulletSpawnOffset, transform.position.z);
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "PlayerBullet") {
            TakeDamage(col.gameObject.GetComponent<PlayerBullet>().BulletDamage);
        }
    }
}
