using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Movement : MonoBehaviour {

    Rigidbody rb;
    QuickReferences qr;
    PlayerEvents pe;
    PlayerStats ps;
    CameraControl cc;

    bool queuedJump, queuedDodge;
    public float inpX, inpY;
    bool lastMovingState, lastSprintingState;
    float sprintTime = 0;
    float dodgeTime = 0;
    float jumpDelay = 0;
    Vector3 dodgeDirection;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        qr = GetComponent<QuickReferences>();
        pe = GetComponent<PlayerEvents>();
        ps = GetComponent<PlayerStats>();
        cc = GetComponent<CameraControl>();
    }

    void Update() {
        if (ps.localPaused) //no input changes when paused
            return;

        inpX = Input.GetAxisRaw("Horizontal"); //get the axis movement (left/right and up/down)
        inpY = Input.GetAxisRaw("Vertical");

        bool moving = (Mathf.Abs(inpX) > 0.1 || Mathf.Abs(inpY) > 0.1) && (Mathf.Abs(rb.velocity.x) > 0.1 || Mathf.Abs(rb.velocity.z) > 0.1 || (ps.climbing && Mathf.Abs(rb.velocity.y) > 0.1));
        if (moving != lastMovingState) {
            pe.CallMoving(moving);
            lastMovingState = moving;
        }

        bool sprinting = (ps.touchingGround || ps.climbing) && ((sprintTime > ps.sprintTimeThreshold && moving && ps.stamina > 0) || (Input.GetButton("Sprint") && moving));
        if (sprinting != lastSprintingState) {
            pe.CallSprint(sprinting);
            lastSprintingState = sprinting;
            if (!sprinting)
                sprintTime = -999;
        }

        if (!queuedJump && !queuedDodge && !ps.climbing && ps.touchingGround) {
            if (Input.GetButtonDown("Jump")) { //player is holding the jump button
                queuedJump = true; //queue the jump
            } else if (Input.GetButtonUp("Dodge") && !sprinting && !ps.climbing && sprintTime >= 0) {
                if (inpX != 0 || inpY != 0)
                    dodgeDirection = (HeadRotation() * Vector3.forward * inpY + HeadRotation() * Vector3.right * inpX).normalized;
                else
                    dodgeDirection = HeadRotation() * Vector3.back;

                queuedDodge = true;
            }
        }

        if (Input.GetButton("Dodge") && ps.touchingGround)
            sprintTime += Time.deltaTime;
        else
            sprintTime = 0;

        if(dodgeTime > 0)
            dodgeTime -= Time.deltaTime;
        if(jumpDelay > 0)
            jumpDelay -= Time.deltaTime;
    }

    void FixedUpdate() { //applies Time.deltaTime to all movement so it doesn't depend on framerate
        if (ps.localPaused || !ps.localOwner) //no input changes when paused
            return;

        if (queuedJump) {
            queuedJump = false;
            if (ps.touchingGround && ps.stamina > ps.jumpCost && jumpDelay <= 0) { //we are grounded
                jumpDelay = 0.1f; //prevent the player from jumping more than once every jumpDelay seconds
                rb.velocity += Vector3.up * ps.jumpHeight; //add jump height
                pe.CallJump();
            }
        }

        if (queuedDodge) {
            queuedDodge = false;
            if (ps.stamina > ps.sprintCost && dodgeTime <= 0) {
                pe.CallDodge();
                rb.velocity = dodgeDirection * ps.dodgeDistance;
                dodgeTime = ps.dodgeCooldownTime;
            }
            dodgeDirection = Vector3.zero;
        }

        if (!ps.animLocked) {
            Vector3 newVelocity = Vector3.zero;
            Vector3 friction = new Vector3(-rb.velocity.x, ((ps.climbing) ? -rb.velocity.y : 0), -rb.velocity.z) * ps.friction * ((Mathf.Abs(inpX) > 0.1 || Mathf.Abs(inpY) > 0.1) ? ps.frictionMovingMultiplier : 1); //attach friction depending on current velocity

            if (ps.climbing) { //climbing movement
                Vector3 leftright = ClimbableRotation() * Vector3.right * inpX * ps.acceleration * ps.climbSpeedMultiplier;
                Vector3 updown = ClimbableRotation() * Vector3.up * inpY * ps.acceleration * ps.climbSpeedMultiplier;
                newVelocity = (updown + leftright).normalized * ((ps.sprinting) ? ps.climbingSprintMultiplier : 1); //resultant velocity from the inputs
                friction *= ps.climbingFriction; //apply extra friction for tuned movespeed
            } else { //normal movement
                Vector3 leftright = HeadRotation() * Vector3.right * inpX * ps.acceleration;
                Vector3 forwardback = HeadRotation() * Vector3.forward * inpY * ps.acceleration;
                newVelocity = (forwardback + leftright).normalized * ((ps.sprinting) ? ps.sprintMultiplier : 1); //resultant velocity from the inputs
            }

            if(inpY < -0.75f && ps.climbing && ps.touchingGround) {
                pe.CallClimb(null);
                return;
            }

            rb.velocity = Vector3.ClampMagnitude(rb.velocity + newVelocity + friction, ps.maxSpeed);
        }
    }

    Quaternion HeadRotation() {
        return Quaternion.AngleAxis(cc.cam.transform.rotation.eulerAngles.y, Vector3.up);
    }

    Quaternion ClimbableRotation() {
        return Quaternion.AngleAxis(ps.climbedSurface.transform.rotation.eulerAngles.y, Vector3.up);
    }

}
