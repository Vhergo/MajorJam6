using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private ParticleSystem dustEffect;
    [SerializeField] private ParticleSystem healEffect;
    [SerializeField] private float healCooldown;
    [SerializeField] private bool dustOnLand;
    [SerializeField] private bool dustOnJump;
    private float healCooldownCounter;
    private PlayerAnimation anim;

    [Header("Player")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    public float MaxHealth { get; set; }
    public float CurrentHealth {
        get { return currentHealth; }
        set { currentHealth = value; }
    }

    void Start() {
        anim = GameObject.Find("PlayerSprite").GetComponent<PlayerAnimation>();
        currentHealth = maxHealth;
        healCooldownCounter = healCooldown;
    }

    void Update() {
        Timer();
    }
    
    void TakeDamage(float takenDamage) {
        currentHealth -= takenDamage;

        if (currentHealth <= 0) {
            PlayerDeath();
        }else {
            anim.PlayHurtAnim();
        }
    }

    void PlayerDeath() {
        anim.PlayDeathAnim();
        gameObject.GetComponent<PlayerMovement>().IsDead = true;
    }

    void Heal(float healAmount) {
        if (currentHealth <= maxHealth) currentHealth += healAmount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        HealAnimation();

        healCooldownCounter = 0f;
    }

    void DustAnimation() {
        dustEffect.Play();
    }

    void HealAnimation() {
        healEffect.Play();
    }

    void Timer() {
        healCooldownCounter += Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "PlayerBullet") {
            PlayerBullet playerBullet = col.gameObject.GetComponent<PlayerBullet>();
            if (playerBullet.Bounced) {
                if (healCooldownCounter >= healCooldown) Heal(playerBullet.HealAmount);
                Destroy(col.gameObject);
            }
        }else if (col.gameObject.tag == "SentinelBullet") {
            TakeDamage(col.gameObject.GetComponent<SentinelBullet>().BulletDamage);
            Destroy(col.gameObject);
        }

        if (col.gameObject.tag == "Explosion") {
            TakeDamage(col.gameObject.GetComponent<Explosion>().Damage);
        }

        if (col.gameObject.tag == "ShieldBash") {
            TakeDamage(col.gameObject.GetComponent<ShieldBash>().Damage);
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "Ground" || col.gameObject.tag == "Platform") {
            if (dustOnLand) DustAnimation();
        }
    }

    void OnCollisionExit2D(Collision2D col) {
        if (col.gameObject.tag == "Ground" || col.gameObject.tag == "Platform") {
            if (dustOnJump) DustAnimation();
        }
    }
}
