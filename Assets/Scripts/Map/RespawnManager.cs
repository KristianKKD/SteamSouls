using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour { //place this on each scene

    public static RespawnManager rm;

    public RespawnSite activeRespawnPoint;

    private void Awake() {
        rm = this;
    }

    public Vector3 GetRespawnPosition() {
        return activeRespawnPoint.GetSpawnPosition() + Vector3.up * 2;
    }

    public Quaternion GetRespawnRotation() {
        return activeRespawnPoint.GetSpawnRotation();
    }

}
