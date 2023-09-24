using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    protected float bulletDamage;
    protected float bulletSpeed;

    public float BulletDamage { get; set; }
    public float BulletSpeed { get; set; }
    
    protected Rigidbody2D rb;

    protected virtual void Start() {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
}
