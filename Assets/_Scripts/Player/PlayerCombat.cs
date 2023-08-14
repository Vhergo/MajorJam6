using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private List<AttackSO> combo;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackTimer;
    [SerializeField] private float comboCooldown;
    [SerializeField] private float comboTimer;
    [SerializeField] private int comboCounter;
    
    private Animator anim;

    void Start() {
        anim = GetComponent<Animator>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.X)) Attack();
        ExitAttack();
    }

    void Attack() {
        if (Time.time - comboTimer > comboCooldown && comboCounter <= combo.Count) {
            CancelInvoke("EndCombo");
            if (Time.time - attackTimer >= attackCooldown) {
                anim.runtimeAnimatorController = combo[comboCounter].animOV;
                anim.Play("Attack", 0, 0);
                print("Attack Damage: " + combo[comboCounter].damage);
                comboCounter++;
                attackTimer = Time.time;

                if (comboCounter > combo.Count) comboCounter = 0;
            }
        }else {
            print("CAN'T ATTACK");
        }
    }

    void ExitAttack() {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9) {
            if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack")) {
                Invoke("EndCombo", 1);
            }
        }
    }

    void EndCombo() {
        comboCounter = 0;
        comboTimer = Time.time;
    }
}
