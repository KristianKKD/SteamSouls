using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchingGround : MonoBehaviour {

    public GameObject playerParent;

    QuickReferences qr;
    PlayerEvents pe;

    public bool touching = false;

    private void Awake() {
        qr = playerParent.GetComponent<QuickReferences>();
        pe = playerParent.GetComponent<PlayerEvents>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other != qr.bodyBox && !other.isTrigger)
            pe.CallSteppingOn(other.gameObject);
    }

    private void OnTriggerExit(Collider other) {
        pe.CallSteppingOff(other.gameObject);
    }

}

