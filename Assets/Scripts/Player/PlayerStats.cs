using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Realtime;

public class PlayerStats : MonoBehaviourPunCallbacks {

    public bool localGame = false;

    //health
    public float health = 100;
    public float maxHealth = 100;

    //stamina
    public float stamina = 100;
    public float maxStamina = 100;

    //food
    public float food = 100;
    public float maxFood = 100;

    //variables affecting variable logic
    public bool inCombat = true;
    public bool moving = false;
    public bool localPaused = false;
    public bool localOwner = false;

    //states
    public bool climbing = false;
    public bool touchingGround = false;
    public bool sprinting = false;
    public bool dodging = false;
    public bool animLocked = false;

    //basic movement
    public float maxSpeed = 20;
    public float acceleration = 7;
    public float friction = 1;
    public float frictionMovingMultiplier = 0.5f; //friction applied harder when we aren't trying to move

    //other movement
    public Transform defaultParent;
    public GameObject anchorObj = null;

    //jumping
    public float jumpHeight = 5;
    public float jumpCost = 15;

    //dodging
    public float dodgeDistance = 5;
    public float dodgeTime = 0.75f;
    public float dodgeCost = 15;
    public float dodgeCooldownTime = 0.5f;

    //climbing
    public float climbCost = 1;
    public float climbSpeedMultiplier = 0.7f;
    public float climbingSprintMultiplier = 2.5f;
    public float climbingFriction = 2f;
    public GameObject climbedSurface = null;

    //sprinting
    public float sprintTimeThreshold = 1;
    public float sprintCost = 2;
    public float sprintMultiplier = 2;

    //stamina logic
    public bool stamRegening = false; //stamina is currently regenerating
    public float stamMaxRegenTime = 5; //maximum time to regen stamina from 0->100%
    public float stamRegenCooldownTime = 1.5f; //time before stamina begins regening
    float stamCooldownTimeElapsed = 0; //time it takes before stamina regens
    float stamTimeToRegen = 0; //time it will take to regen from the current stamina to 100%

    //invuln logic
    public float invulnTimeRemaining = 0;
    public float invulnTimeGained = 0.25f;

    private void Awake() {
        localGame = !FindObjectOfType<PlayerManager>();

        if (localGame) //debug mode
            localOwner = true;
        else    
            localOwner = photonView.AmOwner;

        defaultParent = transform.parent;

        if (!localOwner) {
            QuickReferences qr = GetComponent<QuickReferences>();

            Destroy(qr.floorBox.GetComponent<TouchingGround>());
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<Movement>());
            Destroy(GetComponent<CameraControl>());
            Destroy(GetComponent<PlayerAnimations>());
            Destroy(GetComponent<PlayerUI>());
            Destroy(GetComponent<PlayerEvents>());
            Destroy(GetComponent<PlayerClimbing>());
            Destroy(GetComponent<StepClimber>());
            Destroy(qr.camParent.GetComponent<Camera>());
            Destroy(this);
        }
    }

    public void OnMove(bool isMoving) {
        moving = isMoving;
    }

    public void OnSprint(bool state) {
        sprinting = state;
    }

    public void OnJump() {
        LoseStamina(jumpCost);
    }

    public void OnDodge() {
        invulnTimeRemaining = invulnTimeGained;
        LoseStamina(dodgeCost);
    }

    public void OnClimb(GameObject surface) {
        climbedSurface = surface;
        climbing = (surface != null);
        GetComponent<Rigidbody>().useGravity = !climbing;
    }

    public void OnSteppingOn(GameObject anchor) {
        touchingGround = (anchor != null);
        anchorObj = anchor;
        transform.parent = anchor.transform;
    }

    public void OnSteppingOff(GameObject anchor) {
        if (anchor != anchorObj)
            return;

        touchingGround = false;
        anchorObj = null;
        transform.parent = defaultParent;
    }

    private void Update() {
        if (!localOwner || localPaused)
            return;

        Invulnerability();
        Stamina();
    }

    void Invulnerability() {
        if (invulnTimeRemaining >= 0)
            invulnTimeRemaining = (invulnTimeRemaining - Time.deltaTime >= 0) ? invulnTimeRemaining - Time.deltaTime : 0; //don't go below 0 time

        dodging = invulnTimeRemaining > 0;
    }

    void Stamina() {
        if (stamCooldownTimeElapsed > 0) //waiting for cooldown before regen
            stamCooldownTimeElapsed -= Time.deltaTime;

        stamRegening = (stamina < maxStamina && stamCooldownTimeElapsed <= 0); //stamina regening when there is stam to regen and we aren't doing something that would create a cooldown (used stamina)

        if (stamRegening) {
            stamTimeToRegen -= Time.deltaTime;
            stamina = Mathf.Clamp((stamMaxRegenTime - stamTimeToRegen) / stamMaxRegenTime * maxStamina, 0, maxStamina); //stamMaxRegenTime - stamTimeToRegen = time to regen until max stam, is then scaled to be a percentage of maxStamina
        } else
            stamTimeToRegen = stamMaxRegenTime - ((stamina / maxStamina) * stamMaxRegenTime); //amount of time to regen stamina, gotten by using a percentage of max stamina and inverting the result

        if (climbing)
            LoseStaminaOverTime(climbCost * ((moving) ? 1 : 0)); //don't lose stamina if not moving on the wall
        if (sprinting)
            LoseStaminaOverTime(sprintCost * ((inCombat) ? 1 : 0)); //don't lose stamina if not in combat whilst sprinting
    }

    void LoseStaminaOverTime(float cost) {
        cost *= Time.deltaTime; //depending on the frame data
        LoseStamina(cost);
    }

    void LoseStamina(float cost) {
        stamina = (stamina - cost >= 0) ? stamina - cost : 0; //don't go below 0 stamina
        stamCooldownTimeElapsed = stamRegenCooldownTime; //apply the cooldown so it starts ticking down
    }

}
