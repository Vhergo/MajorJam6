using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    private AnimationState currentState;
    private bool onPause;

    public enum AnimationState {
        PlayerIdle,
        PlayerRun,
        PlayerJump,
        PlayerJumpUp,
        PlayerJumpDown,
        PlayerHurt,
        PlayerDeath,
        PlayerDead
    }
    
    void Start() {
        anim = GetComponent<Animator>();
        onPause = false;
    }

    public void ChangeAnimationState(AnimationState newState) {
        // stop the same animation from interrupting itself
        if (currentState == newState) return;

        anim.Play(newState.ToString()); // play animation
        currentState = newState; // upadate the current state
    }

    public void PlayIdleAnim() {
        if (!onPause) ChangeAnimationState(AnimationState.PlayerIdle);
    }

    public void PlayRunAnim() {
        if (!onPause) ChangeAnimationState(AnimationState.PlayerRun);
    }

    public void PlayJumpAnim() {
        ChangeAnimationState(AnimationState.PlayerJumpUp);
    }

    public void PlayHurtAnim() {
        onPause = true;
        ChangeAnimationState(AnimationState.PlayerHurt);
        Invoke("Unpause", 0.1f);
    }

    public void PlayDeathAnim() {
        onPause = true;
        ChangeAnimationState(AnimationState.PlayerDeath);
    }

    void Unpause() {
        onPause = false;
    }
}
