using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAnimations : MonoBehaviour {

    Movement mv;
    Animator anim;
    PlayerStats ps;

    public bool animLocked;

    private void Awake() {
        mv = GetComponent<Movement>();
        ps = GetComponent<PlayerStats>();
        anim = GetComponent<Animator>();

        GetAnimTimes();
    }

    private void Update() {
        //var currentAnim = anim.GetCurrentAnimatorStateInfo(0);
        //animLocked = (!currentAnim.IsName("Walk") && !currentAnim.IsName("Idle"));

        anim.SetFloat("X", mv.inpX);
        anim.SetFloat("Y", mv.inpY);

        if (!ps.moving)
            anim.SetTrigger("NotMoving");
    }

    void GetAnimTimes() {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips) {
            switch (clip.name) {
                case "Dodge":
                    break;
                case "Jump":
                    break;
            }
        }
    }

    public void OnJump() {
        anim.SetTrigger("Jumping");
    }

    public void OnDodge() {
        anim.SetTrigger("Dodging");
    }

}
