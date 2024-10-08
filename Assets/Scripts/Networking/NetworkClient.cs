using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkClient : MonoBehaviourPunCallbacks {

    public static NetworkClient nc;
    
    string lastRoom = ""; //for status message

    private void Awake() {
        nc = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Connect() { //called on click of the 'play' button
        PhotonNetwork.ConnectUsingSettings();

        int rand = Random.Range(0, 10000);
        string name = "Player" + rand;
        PhotonNetwork.NickName = name;
        //update name for all
    }

    public override void OnConnectedToMaster() {
        base.OnConnectedToMaster();

        PhotonNetwork.AutomaticallySyncScene = true;
        MenuNavigation.mn.ConnectedToMaster();
    }

    public void Host(string lobbyID) {
        RoomOptions room = new RoomOptions() {
            IsVisible = false,
            IsOpen = true,
            MaxPlayers = 4
        };

        lastRoom = lobbyID;
        PhotonNetwork.CreateRoom(lobbyID, room);
    }

    public void Join(string roomName) {
        lastRoom = roomName;
        PhotonNetwork.JoinRoom(roomName);
    }

    public void Disconnect() {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        base.OnCreateRoomFailed(returnCode, message);

        MenuNavigation.mn.GotoMenu();
        MenuNavigation.mn.ShowStatus("Failed to create room: " + lastRoom);
        Debug.Log("Failed create room");
    }

    public override void OnJoinRoomFailed(short returnCode, string message) {
        base.OnJoinRoomFailed(returnCode, message);

        MenuNavigation.mn.GotoLobby();
        MenuNavigation.mn.ShowStatus("Failed to join room: " + lastRoom);
        Debug.Log("Failed join room");
    }

    public override void OnCreatedRoom() {
        base.OnCreatedRoom();

        MenuNavigation.mn.HostingRoom();
    }

    public override void OnJoinedRoom() {
        base.OnJoinedRoom();

        MenuNavigation.mn.JoinedRoom(PhotonNetwork.IsMasterClient);
    }

}
