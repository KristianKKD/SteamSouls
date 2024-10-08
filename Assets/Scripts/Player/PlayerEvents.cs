using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerEvents : MonoBehaviour {

    QuickReferences qr;
    PlayerAnimations am;
    PlayerStats ps;
    PlayerUI pu;
    StepClimber sc;

    private void Awake() {
        qr = GetComponent<QuickReferences>();
        am = GetComponent<PlayerAnimations>();
        ps = GetComponent<PlayerStats>();
        pu = GetComponent<PlayerUI>();
        sc = GetComponent<StepClimber>();
    }

    public void CallJump() {
        if (am != null)
            am.OnJump();
        if (ps != null)
            ps.OnJump();
    }

    public void CallDodge() {
        if (am != null)
            am.OnDodge();
        if (ps != null)
            ps.OnDodge();
    }

    public void CallMoving(bool isMoving) {
        if (ps != null)
            ps.OnMove(isMoving);
    }

    public void CallSprint(bool isSprinting) {
        if (ps != null)
            ps.OnSprint(isSprinting);
    }

    public void CallClimbable(bool climbable) {
        if (pu != null)
            pu.OnClimbable(climbable);
    }

    public void CallClimb(GameObject climbedSurface) {
        if (ps != null)
            ps.OnClimb(climbedSurface);
        if (pu != null)
            pu.OnClimb(climbedSurface != null);
    }

    public void CallSteppingOn(GameObject anchor) {
        if (ps != null)
            ps.OnSteppingOn(anchor);
    }

    public void CallSteppingOff(GameObject anchor) {
        if (ps != null)
            ps.OnSteppingOff(anchor);
        if (sc != null)
            sc.OnSteppingOff(anchor);
    }
}

