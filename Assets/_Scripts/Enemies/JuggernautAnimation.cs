using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuggernautAnimation : MonoBehaviour
{
    private Animator anim;
    private AnimationState currentState;
    private bool onPause;

    public Animator Anim { get; }

    public enum AnimationState {
        JuggernautIdle,
        JuggernautWalk,
        JuggernautAttack,
        JuggernautHurt,
        JuggernautDeath
    }

    void Start() {
        anim = GetComponent<Animator>();
        onPause = false;
    }

    void ChangeAnimationState(AnimationState newState) {
        if (currentState == newState) return;

        anim.Play(newState.ToString());
        currentState = newState;
    }

    public void PlayIdleAnim() {
        if (!onPause) ChangeAnimationState(AnimationState.JuggernautIdle);
    }

    public void PlayWalkAnim() {
        if (!onPause) ChangeAnimationState(AnimationState.JuggernautWalk);
    }

    public void PlayAttackAnim() {
        onPause = true;
        ChangeAnimationState(AnimationState.JuggernautAttack);
        Invoke("Unpause", 0.5f);
    }

    public void PlayHurtAnim() {
        onPause = true;
        ChangeAnimationState(AnimationState.JuggernautHurt);
        Invoke("Unpause", 0.1f);
    }

    public void PlayDeathAnim() {
        onPause = true;
        ChangeAnimationState(AnimationState.JuggernautDeath);
    }

    void Unpause() {
        onPause = false;
    }
}
