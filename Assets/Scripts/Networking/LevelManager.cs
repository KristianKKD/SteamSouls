using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;
using System;

public class LevelManager : MonoBehaviourPunCallbacks {

    public static LevelManager lm;

    private void Awake() {
        lm = this;

        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    public void StartGame() {
        PhotonNetwork.AutomaticallySyncScene = true;
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(1);
    }

    void OnSceneChanged(Scene currentScene, Scene nextScene) {
        if(nextScene.buildIndex > 0)
            PlayerManager.pm.SpawnMyPlayer();
    }

    public void BackToMain() {
        PhotonNetwork.Disconnect();
        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
    }

}
