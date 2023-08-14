using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomkinAnimation : MonoBehaviour
{
    private Animator anim;
    private AnimationState currentState;

    public Animator Anim { get; }

    public enum AnimationState {
        BoomkinRun,
        BoomkinTrigger,
        BoomkinHurt
    }

    void Start() {
        anim = GetComponent<Animator>();
    }

    void ChangeAnimationState(AnimationState newState) {
        if (currentState == newState) return;

        anim.Play(newState.ToString());
        currentState = newState;
    }

    public void PlayRunAnim() {
        ChangeAnimationState(AnimationState.BoomkinRun);
    }

    public void PlayTriggerAnim() {
        ChangeAnimationState(AnimationState.BoomkinTrigger);
    }

    public void PlayHurtAnim() {
        ChangeAnimationState(AnimationState.BoomkinHurt);
        Invoke("PlayRunAnim", 0.1f);
    }
}
