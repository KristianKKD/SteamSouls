using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectUI : MonoBehaviour {

    public GameObject menuBack;

    public GameObject mainPauseMenu;
    public GameObject subSettingsMenu;

    public GameObject mainInventoryMenu;
    public GameObject subSkillsMenu;

    public GameObject guiMessageBack;
    public TMP_Text messageText;

    public GameObject climbPrompt;

    public Scrollbar healthBar;
    public Scrollbar staminaBar;
    public Scrollbar foodBar;

    //private void Awake() {
    //    foreach (PlayerUI pu in FindObjectsOfType<PlayerUI>())
    //        if (pu.gameObject.GetComponent<PlayerStats>())
    //            pu.TransferOwnership(this);
    //}

}
