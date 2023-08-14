using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("General")]
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float currentHealth;
    [SerializeField] protected float deathDelay;

    [Header("Movement")]
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float acceleration;
    
    protected Transform target;
    protected Rigidbody2D rb;
    protected bool isCrawler;

    public bool IsCrawler { get; set; }
    
    protected virtual void Start() {
        target = GameObject.Find("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        IsCrawler = false;
    }

    // protected virtual void TakeDamage(float damageAmount) {
    //     print("TOOK DAMAGE: " + damageAmount);
    //     currentHealth -= damageAmount;
    //     if (currentHealth <= 0) EnemyDeath();
    // }

    // protected virtual void EnemyDeath() {
    //     // Play Death animation
    //     // Destroy(gameObject, deathDelay);
    // }

    // void OnTriggerEnter2D(Collider2D col) {
    //     if (col.gameObject.tag == "PlayerBullet") {
    //         TakeDamage(col.gameObject.GetComponent<PlayerBullet>().BulletDamage);
    //     }
    // }
}
