using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(PhotonView))]
public class SyncPlayers : MonoBehaviourPunCallbacks, IPunObservable {

    QuickReferences qr;

    float yRot;

    public float smoothing = 20;

    private void Awake() {
        qr = GetComponent<QuickReferences>();
    }

    void Update() { //if we are the owner of this object, we broadcast the rotation, otherwise we set the object's rotation depending on the received information
        if (!photonView.IsMine) {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, yRot, 0), Time.deltaTime * smoothing);
        } else {
            yRot = qr.head.transform.rotation.eulerAngles.y;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) { //sending
            stream.SendNext(yRot);
        } else { //receiving
            yRot = (float)stream.ReceiveNext();
        }
    }

}
