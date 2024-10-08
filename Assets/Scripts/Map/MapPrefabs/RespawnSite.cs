using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RespawnSite : MonoBehaviour {

    public Transform[] spawnPoints = new Transform[4];

    public Vector3 GetSpawnPosition() {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            if (PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer)
                return spawnPoints[i].position;

        return spawnPoints[0].position;
    }

    public Quaternion GetSpawnRotation() {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            if (PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer)
                return spawnPoints[i].rotation;

        return spawnPoints[0].rotation;
    }

}
