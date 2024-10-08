using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxRegistration : MonoBehaviour {

    PlayerStats ps;

    private void Awake() {
        ps = GetComponentInParent<PlayerStats>();
    }

    void Update() {
        if (ps == null) //destroy this if this is not a local player
            Destroy(this);
        GetComponent<Collider>().enabled = !ps.dodging;
    }
}
