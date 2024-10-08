using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class StepClimber : MonoBehaviour {

    public GameObject red;
    public GameObject blue;
    public GameObject green;

    public float maxStepHeight = 0.3f;
    public float minStepDepth = 0.2f;

    QuickReferences qr;
    PlayerStats ps;
    Rigidbody rb;

    private void Awake() {
        qr = GetComponent<QuickReferences>();
        ps = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionStay(Collision collision) {
        OnCollisionEnter(collision);
    }

    private void OnCollisionEnter(Collision collision) {
        if ((Mathf.Abs(rb.velocity.x) < 0.05f && Mathf.Abs(rb.velocity.z) < 0.05) ||
                Mathf.Abs(rb.velocity.y) > 5f ||
                Mathf.Abs(rb.velocity.y) < 0.1f ||
                rb.velocity.y < -0.1f)
            return;

        for (int i = 0; i < collision.contactCount; i++) {
            ContactPoint contact = collision.GetContact(i);
            if (contact.otherCollider != qr.bodyBox && //don't look at the player
                  !contact.otherCollider.isTrigger && //ignore trigger colliders
                  contact.point.y > qr.floorBox.transform.position.y + 0.1f //&& //ignore floor
                   ) { //max step height

                MeshCollider col = contact.otherCollider.gameObject.GetComponent<MeshCollider>();
                if (col)
                    collision.gameObject.GetComponent<MeshCollider>().convex = true;

                Vector3 checkPoint = contact.point + -contact.normal * 0.1f + Vector3.up * maxStepHeight;
                RaycastHit[] hits = Physics.RaycastAll(checkPoint, Vector3.down);
                for (int j = 0; j < hits.Length; j++)
                    if (hits[j].collider.gameObject.layer != LayerMask.GetMask("Player") &&
                            hits[j].collider == contact.otherCollider && //the relevant object (ignore the floor below for example)
                            hits[j].point.y <= qr.floorBox.transform.position.y + maxStepHeight && //is within step range
                            hits[j].point.y > qr.floorBox.transform.position.y + 0.05f) { //min step height

                        transform.position += hits[j].point - qr.floorBox.transform.position;
                        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                        if (col)
                            collision.gameObject.GetComponent<MeshCollider>().convex = false;
                        break;
                    }
            }
        }
    }

    public void OnSteppingOff(GameObject anchor) {
        if (rb.velocity.y >= 0)
            return;

        RaycastHit[] hits = Physics.BoxCastAll(qr.floorBox.transform.position + (Vector3.down * maxStepHeight), qr.floorBox.bounds.extents, Vector3.one, transform.rotation, maxStepHeight);
        foreach (RaycastHit hit in hits) {
            if (hit.collider != qr.bodyBox && //don't look at the player
                  !hit.collider.isTrigger && //ignore trigger colliders
                  hit.point.y < qr.floorBox.transform.position.y && //the hit point is below the feet
                  (qr.floorBox.transform.position - hit.point).magnitude <= maxStepHeight) { //max step height

                Instantiate(red, transform.parent).transform.position = hit.point;
                Debug.Log(hit.point);
            }
        }
    }

}
