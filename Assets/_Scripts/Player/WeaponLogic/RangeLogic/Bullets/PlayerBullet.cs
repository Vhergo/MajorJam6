using Codice.Client.Commands.TransformerRule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : Bullet
{
    #region VARIABLES
    [Header("Ricochet")]
    [SerializeField] private int bounceLimit;
    [SerializeField] private int bounceCounter;

    [Header("Other")]
    [SerializeField] private GameObject bulletImpactPrefab;
    [SerializeField] private float destroyDelay;

    private float knockbackForce;
    private float healAmount;
    private bool bounced;
    public float KnockbackForce { get; set; }
    public float HealAmount { get; set; }
    public bool Bounced { get; set; }

    public enum HitTarget {
        Enemy,
        Player,
        None
    }
    #endregion

    protected override void Start() {
        base.Start();
        bounceCounter = 0;

        Destroy(gameObject, destroyDelay);
    }

    void BulletBouce(Collider2D col) {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rb.velocity.normalized, 1f, LayerMask.GetMask("Wall"));
        Vector2 roundedNormal;
        if (Mathf.Abs(hit.normal.x) > Mathf.Abs(hit.normal.y)) {
            roundedNormal = new Vector2(Mathf.Sign(hit.normal.x), 0);
        }else {
            roundedNormal = new Vector2(0, Mathf.Sign(hit.normal.y));
        }
        rb.velocity = Vector2.Reflect(rb.velocity, roundedNormal).normalized * BulletSpeed;

        // Adjust bullet rotation to match bounce direction
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        bounceCounter++;
        Bounced = true;
    }

    void Knockback(GameObject enemy) {
        Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;
        enemy.GetComponent<Rigidbody2D>().AddForce(knockbackDirection * KnockbackForce, ForceMode2D.Impulse);
    }

    public void BulletImpact(Collider2D col, HitTarget hitTarget = HitTarget.None) {
        Vector2 impactPoint = col.ClosestPoint(transform.position);
        Vector2 collisionNormal = impactPoint - (Vector2)transform.position;
        float angle = Mathf.Atan2(collisionNormal.y, collisionNormal.x) * Mathf.Rad2Deg;

        GameObject bulletImpact = Instantiate(bulletImpactPrefab, impactPoint, Quaternion.Euler(0f, 0f, angle));
        if (hitTarget == HitTarget.Enemy) {
            bulletImpact.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.3f, 0.3f); // red
        }else if (hitTarget == HitTarget.Player) {
            bulletImpact.GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.8f, 0.2f); // green
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Enemy") {
            BulletImpact(col, HitTarget.Enemy);
            Knockback(col.gameObject);
            Destroy(gameObject);
        }

        if (col.gameObject.tag == "Player") {
            if (Bounced)  BulletImpact(col, HitTarget.Player);
        }

        if (col.gameObject.tag == "Bounce" || col.gameObject.tag == "Platform") {
            if (bounceCounter < bounceLimit) {
                BulletBouce(col);
                BulletImpact(col);
            }else {
                BulletImpact(col);
                Destroy(gameObject);
            }
        }

        if (col.gameObject.tag == "Ground") {
            BulletImpact(col);
            Destroy(gameObject);
        }
    }
}
