using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbing : MonoBehaviour {

    QuickReferences qr;
    PlayerStats ps;
    PlayerEvents pe;

    public bool canClimb = false;
    public GameObject climbeableSurface = null;

    private void Awake() {
        qr = GetComponent<QuickReferences>();
        ps = GetComponent<PlayerStats>();
        pe = GetComponent<PlayerEvents>();
    }

    public void CanClimb(GameObject surface) {
        canClimb = true;
        climbeableSurface = surface;
        pe.CallClimbable(true);
    }

    public void DisconnectFromSurface(GameObject surface) {
        canClimb = false;
        climbeableSurface = null;
        pe.CallClimb(null); //call the event but with no surface so we disconnect from it
        pe.CallClimbable(false);
    }

    public void Update() {
        if (ps.localPaused)
            return;

        if (ps.climbing && ps.stamina < ps.climbCost) //ran out of stamina whilst climbing
            pe.CallClimb(null);

        if (Input.GetButtonUp("Interact") && canClimb && ps.stamina > ps.climbCost) { //interact button is pressed to climb, we are near a climbeable surface and have stamina
            if (ps.climbing) //already climbing, disconnect from the surface
                pe.CallClimb(null); //call the event but with no surface so we disconnect from it
            else //we aren't currently climbing
                pe.CallClimb(climbeableSurface); //call the event with the surface we want to set as the one we are climbing
        }

        if (ps.climbing) { //set the rotation to stick to the wall
            qr.head.transform.Rotate(0, -climbeableSurface.transform.rotation.y, 0);
            qr.body.transform.Rotate(0, -climbeableSurface.transform.rotation.y, 0);
        }
    }

}
