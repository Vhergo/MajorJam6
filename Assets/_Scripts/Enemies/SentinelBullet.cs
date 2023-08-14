using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentinelBullet : Bullet
{
    [SerializeField] private float maxBulletHealth;
    [SerializeField] private float currentBulletHealth;
    private float launchDelay;
    private float launchDelayCounter;
    private bool canMove;

    public float LaunchDelay { get; set; }
    public bool CanMove { get; set; }

    private Transform target;
    private Vector2 direction;
    private Vector2 targetVelocity;

    protected override void Start() {
        base.Start();
        target = GameObject.Find("Player").transform;
        currentBulletHealth = maxBulletHealth;
        launchDelayCounter = 0;
    }

    void Update() {
        if (!CanMove) {
            BulletFollowDelay();
        }else {
            FollowPlayer();
        }
    }

    void BulletFollowDelay() {
        if (launchDelayCounter >= LaunchDelay) {
            CanMove = true;
        }else {
            launchDelayCounter += Time.deltaTime;
        }
    }

    void FollowPlayer() {
        direction = (target.position - transform.position).normalized;
        rb.velocity = direction * BulletSpeed;
    }

    void TakeDamage(float bulletDamage) {
        currentBulletHealth -= bulletDamage;
        if (currentBulletHealth <= 0) {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "PlayerBullet") {
            TakeDamage(col.gameObject.GetComponent<PlayerBullet>().BulletDamage);
            Destroy(col.gameObject);
        }
    }
}
