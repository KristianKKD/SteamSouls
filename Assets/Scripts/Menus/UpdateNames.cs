using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class UpdateNames : MonoBehaviour {

    public TMP_Text[] names;
    public TMP_InputField nameText;

    bool nameSelected = true;

    private void Awake() {
    }


    void FixedUpdate() {
        UpdateMyName();

        for (int i = 0; i < names.Length; i++)
            names[i].text = (PhotonNetwork.PlayerList.Length - 1 >= i && PhotonNetwork.PlayerList.Length > 0) ?
                PhotonNetwork.PlayerList[i].NickName : "";
    }

    void UpdateMyName() {
        string myName = nameText.text.Trim();
        if (myName.Length-1 <= 0) {
            if (nameSelected)
                return;

            string tempName = RandomName();
            PhotonNetwork.NickName = tempName;
            nameText.text = tempName;
            return;
        }

        PhotonNetwork.NickName = myName;
    }

    string RandomName() {
        return "Player" + Random.Range(0, 1000);
    }

    public void InputSelected() {
        nameSelected = true;
    }

    public void InputDeselected() {
        nameSelected = false;
    }


}
