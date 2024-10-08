using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerManager : MonoBehaviour {

    public static PlayerManager pm;

    public GameObject myPlayer;

    public bool globalPaused;

    private void Awake() {
        pm = this;
    }

    public void SpawnMyPlayer() {
        myPlayer = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        myPlayer.name = PhotonNetwork.NickName;
        myPlayer.transform.parent = RespawnManager.rm.activeRespawnPoint.transform;
        myPlayer.transform.position = RespawnManager.rm.GetRespawnPosition();
        myPlayer.GetComponent<PlayerUI>().GetUI();
    }

    private void Update() {
        if (myPlayer != null) { //we are in game
            globalPaused = false;

            bool allPaused = true;
            foreach (PlayerStats ps in FindObjectsOfType<PlayerStats>())
                if (!ps.localPaused) { //one of the players is not paused
                    allPaused = false;
                    break;
                }

            globalPaused = allPaused;
        }

        Time.timeScale = (globalPaused) ? 0 : 1;
    }



}
