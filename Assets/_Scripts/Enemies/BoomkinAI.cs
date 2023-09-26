using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class BoomkinAI : Enemy
{
    [Header("Explosion")]
    [SerializeField] private float explosionDamage;
    [SerializeField] private float explosionDelay;
    [SerializeField] private Transform explosionLocation;
    [SerializeField] private float explosionRadius;
    [SerializeField] private float deathExplosionModifier;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject explosionPrefab;

    [Header("Components")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDetectionRadius;
    [SerializeField] private float playerDetectionRadius;

    [Header("Pathfinding")]
    [SerializeField] private float activateDistance = 50f;
    [SerializeField] private float pathUpdateSeconds = 0.5f;
    [SerializeField] private float nextWaypointDistance;
    [SerializeField] private float jumpNodeHeightRequirement;
    [SerializeField] private float jumpModifier;

    [Header("Toggles")]
    [SerializeField] private bool followEnabled = true;
    [SerializeField] private bool jumpEnabled = true;

    private BoomkinAnimation anim;

    private Path path;
    private int currentWaypoint = 0;
    private Seeker seeker;
    private Vector2 direction;
    private bool isDead;

    protected override void Start() {
        base.Start();
        seeker = GetComponent<Seeker>();
        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);

        IsCrawler = true;
        anim = GetComponent<BoomkinAnimation>();
    }

    private void Update() {
        Flip();
    }

    void FixedUpdate() {
        if (isDead) return;

        if (TargetInDistance() && followEnabled) {
            if (!InRangeOfPlayer()) {
                PathFollow();
            }else {
                seeker.CancelCurrentPathRequest();
                rb.velocity = Vector2.zero;
                followEnabled = false;

                TriggerExplosion();
            }
        }
    }

    void UpdatePath() {
        if (followEnabled && TargetInDistance() && seeker.IsDone() && IsGrounded()) {
            seeker.StartPath(rb.position, target.position, OnPathComplete); // not function call but a callback
        }
    }

    void PathFollow() {
        if (path == null) return;
        if (currentWaypoint >= path.vectorPath.Count) return;

        Move();
        if (jumpEnabled && IsGrounded() && AllowJump()) Jump();

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance) currentWaypoint++;
    }

    void Move() {
        direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;

        Vector2 targetVelocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, Time.deltaTime * acceleration);
    }

    void Jump() {
        rb.velocity = Vector2.zero;
        rb.AddForce(Vector2.up * jumpModifier, ForceMode2D.Impulse);
    }

    void Flip() {
        if (target.position.x > transform.position.x) {
            transform.localScale = new Vector3(1, 1, 1);
        } else if (target.position.x < transform.position.x) {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public override void TakeDamage(float damageAmount) {
        if (isDead) return;

        currentHealth -= damageAmount;
        if (currentHealth <= 0) {
            EnemyDeath();
        } else {
            anim.PlayHurtAnim();
        }
    }

    void EnemyDeath() {
        isDead = true;
        rb.mass = 1000;
        followEnabled = false;
        Explode();
    }

    void TriggerExplosion() {
        anim.PlayTriggerAnim();
        Invoke("Explode", explosionDelay);
    }

    void Explode() {
        GameObject explosionObject = Instantiate(explosionPrefab, explosionLocation.position, Quaternion.identity);
        explosionObject.GetComponent<Explosion>().Damage = explosionDamage;
        // explosionObject.transform.localScale = new Vector3(deathExplosionModifier, deathExplosionModifier, deathExplosionModifier);
        Destroy(gameObject, deathDelay);
    }

    public void CrawlerKnockback(float knockbackForce, Vector2 knockbackDirection) {
        print("ITS A CRAWLER");
        seeker.CancelCurrentPathRequest();
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
    }

    bool AllowJump() {
        return direction.y > jumpNodeHeightRequirement;
    }

    bool IsGrounded() {
        return Physics2D.OverlapCircle(groundCheck.position, groundDetectionRadius, groundLayer);
    }

    bool TargetInDistance() {
        return Vector2.Distance(transform.position, target.transform.position) < activateDistance;
    }
    
    void OnPathComplete(Path p) {
        if (!p.error) {
            path = p;
            currentWaypoint = 0;
        }
    }

    bool InRangeOfPlayer() {
        return Vector2.Distance(transform.position, target.position) < playerDetectionRadius;
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "PlayerBullet") {
            TakeDamage(col.gameObject.GetComponent<PlayerBullet>().BulletDamage);
        }
    }
}
