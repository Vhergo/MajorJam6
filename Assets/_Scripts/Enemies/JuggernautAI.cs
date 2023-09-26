using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuggernautAI : Enemy
{
    [Header("Attack")]
    [SerializeField] private float attackDamage;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackDuration;
    [SerializeField] private bool allowMovement;
    private float attackCooldownCounter;
    private GameObject attackCollider;
    private bool isDead = false;

    private bool followEnabled = true;

    private JuggernautAnimation anim;
    private Vector2 direction;
    private Vector2 targetVelocity;

    protected override void Start(){
        base.Start();
        PrepShieldBash();
        attackCooldownCounter = 0;

        anim = GetComponent<JuggernautAnimation>();
    }

    void Update() {
        if (attackCooldownCounter <= 0) {
            if (!isDead && !PlayerIsDead()) {
                followEnabled = true;
                if (PlayerInRange()) {
                    Attack();
                    followEnabled = false;
                    attackCooldownCounter = attackCooldown;
                }
            }
        }else {
            attackCooldownCounter -= Time.deltaTime;
        }

        Flip();
        MoveAnim();
    }

    void FixedUpdate() {
        if (followEnabled && !isDead && allowMovement) Move();
    }

    void Move() {
        direction = (target.position - transform.position).normalized;
        targetVelocity = new Vector2(direction.x * moveSpeed, 0);
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, Time.deltaTime * acceleration);
    }

    void MoveAnim() {
        if (Mathf.Abs(rb.velocity.x) > 0.1f) {
            anim.PlayWalkAnim();
        }else {
            anim.PlayIdleAnim();
        }
    }

    void Attack() {
        attackCollider.SetActive(true);
        anim.PlayAttackAnim();
        Invoke("StopAttack", attackDuration);
    }

    void StopAttack() {
        attackCollider.SetActive(false);
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
        isDead = true;
        ObjectPermanence();
        anim.PlayDeathAnim();
    }

    void Flip() {
        if (direction.x > 0) {
            transform.localScale = new Vector3(1, 1, 1);
        }else if (direction.x < 0) {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void ObjectPermanence() {
        Physics2D.IgnoreCollision(GetComponent<PolygonCollider2D>(), target.GetComponent<BoxCollider2D>());
        rb.mass = 1000;
    }

    void PrepShieldBash() {
        attackCollider = transform.GetChild(0).gameObject;
        attackCollider.GetComponent<ShieldBash>().Damage = attackDamage;
    }

    bool PlayerInRange() {
        return Vector2.Distance(transform.position, target.position) <= attackRange;
    }

    bool PlayerIsDead() {
        
        return target.GetComponent<Player>().CurrentHealth <= 0;
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "PlayerBullet") {
            TakeDamage(col.gameObject.GetComponent<PlayerBullet>().BulletDamage);
        }
    }
}
