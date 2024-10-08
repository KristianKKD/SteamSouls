using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbSurface : MonoBehaviour {

    [SerializeField]
    PlayerClimbing nearbyPlayer = null;

    private void OnTriggerEnter(Collider other) {
        PlayerClimbing pc = other.gameObject.gameObject.GetComponentInParent<PlayerClimbing>();
        if (pc != null && nearbyPlayer == null) { //we won't sync this over the network and playerclimbing will be destroyed on synced objects so we don't need to worry about managing this
            nearbyPlayer = pc;
            pc.CanClimb(this.gameObject);
        }
    }

    private void OnTriggerExit(Collider other) {
        PlayerClimbing pc = other.gameObject.gameObject.GetComponentInParent<PlayerClimbing>();

        if (nearbyPlayer != null && pc == nearbyPlayer) {
            nearbyPlayer = null;
            pc.DisconnectFromSurface(this.gameObject);
        }
    }

}
