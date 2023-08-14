using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentinelAnimation : MonoBehaviour
{
    private Animator anim;
    private AnimationState currentState;

    public Animator Anim { get; }

    public enum AnimationState
    {
        SentinelIdle,
        SentinelAttack,
        SentinelHurt,
        SentinelDeath
    }

    void Start() {
        anim = GetComponent<Animator>();
    }

    void ChangeAnimationState(AnimationState newState) {
        if (currentState == newState) return;

        anim.Play(newState.ToString());
        currentState = newState;
    }

    public void PlayIdleAnim() {
        ChangeAnimationState(AnimationState.SentinelIdle);
    }

    public void PlayAttackAnim() {
        ChangeAnimationState(AnimationState.SentinelAttack);
    }

    public void PlayHurtAnim() {
        ChangeAnimationState(AnimationState.SentinelHurt);
        Invoke("PlayIdleAnim", 0.1f);
    }

    public void PlayDeathAnim() {
        ChangeAnimationState(AnimationState.SentinelDeath);
    }
}
