using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;

public class MenuNavigation : MonoBehaviour {

    public static MenuNavigation mn;

    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject multiplayerMenu;
    public GameObject lobbyMenu;
    public GameObject loadingScreen;

    public GameObject hostButton;
    public GameObject joinButton;

    public GameObject startButton;

    public GameObject statusText;

    public TMP_InputField multiID;
    public TMP_Text lobbyID;
    public TMP_InputField username;

    private void Awake() {
        mn = this;
    }

    public void GotoLobby() {
        statusText.SetActive(false);
        mainMenu.SetActive(false);
        multiplayerMenu.SetActive(true);

        multiID.text = "";
        lobbyID.text = "LOBBY:" + multiID.text;
        startButton.SetActive(false);

        if (!PhotonNetwork.IsConnected) {
            hostButton.SetActive(false);
            joinButton.SetActive(false);

            ShowStatus("Connecting...");
            NetworkClient.nc.Connect();
            username.text = PhotonNetwork.NickName;
        }
    }

    public void GotoSettings() {
        statusText.SetActive(false);
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void GotoMenu() {
        statusText.SetActive(false);
        settingsMenu.SetActive(false);
        multiplayerMenu.SetActive(false);
        lobbyMenu.SetActive(false);

        mainMenu.SetActive(true);
        startButton.SetActive(true);
    }

    public void DisconnectFromLobby() {
        NetworkClient.nc.Disconnect();
        GotoMenu();
    }

    public void ExitApp() {
        Application.Quit(0);
    }

    public void HostButtonClicked() {
        statusText.SetActive(false);

        /* FUNCTIONAL, but set to 1 for debugging
        string txt = multiID.text.Trim();
        if (txt.Length == 0)
            txt = Random.Range(0, 1000).ToString();
        */

        string txt = "1"; //debugging

        ShowStatus("Attempting to host room with ID: " + txt + "...");

        NetworkClient.nc.Host(txt);
    }

    public void JoinButtonClicked() {
        statusText.SetActive(false);

        /* FUNCTIONAL, but set to 1 for debugging
        string txt = multiID.text.Trim();
        if (txt.Length == 0) {
            ShowStatus("Please enter a lobby ID");
            return;
        }
        */

        string txt = "1"; //debugging

        ShowStatus("Attempting to join: " + txt + "...");

        NetworkClient.nc.Join(txt);
    }
    
    public void StartButtonClicked() {
        statusText.SetActive(false);
        lobbyMenu.SetActive(false);

        loadingScreen.SetActive(true);

        LevelManager.lm.StartGame();
    }

    public void ConnectedToMaster() {
        statusText.SetActive(false);

        hostButton.SetActive(true);
        joinButton.SetActive(true);
    }

    public void HostingRoom() { //callback from Host
        statusText.SetActive(false);

        multiplayerMenu.SetActive(false);
        lobbyMenu.SetActive(false);
    }

    public void JoinedRoom(bool isHosting) { //callback from Join/Host
        if (statusText == null) //this condition is true if the host is already in the next scene
            return;

        statusText.SetActive(false);

        multiplayerMenu.SetActive(false);
        lobbyMenu.SetActive(true);

        lobbyID.text = "LOBBY:" + PhotonNetwork.CurrentRoom.Name;

        if (isHosting)
            startButton.SetActive(true);
    }

    public void ShowStatus(string message) {
        statusText.SetActive(message.Length > 0);
        statusText.GetComponent<TMP_Text>().text = message.ToUpper();
    }
}
