using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class PlayerUI : MonoBehaviour {

    public GameObject menuParent;

    public GameObject mainPauseMenu;
    public GameObject settingsMenu;

    public GameObject mainInventoryMenu;
    public GameObject subSkillsMenu;

    public GameObject guiMessageBack;
    public TMP_Text messageText;

    public GameObject climbPrompt;

    public Scrollbar healthBar;
    public Scrollbar staminaBar;
    public Scrollbar foodBar;

    PlayerStats ps;

    [SerializeField]
    GameObject currentMenu;

    private void Awake() {
        ps = GetComponent<PlayerStats>();

        GetUI();

        if (!ps.localGame) //lock mouse in play mode
            Cursor.lockState = CursorLockMode.Locked;
    }

    void OnDestroy() {
        Destroy(menuParent.transform.parent.gameObject);
    }

    public void GetUI() {
        CollectUI ui = FindObjectOfType<CollectUI>();

        menuParent = ui.menuBack;

        mainPauseMenu = ui.mainPauseMenu;
        settingsMenu = ui.subSettingsMenu;

        mainInventoryMenu = ui.mainInventoryMenu;
        subSkillsMenu = ui.subSkillsMenu;

        guiMessageBack = ui.guiMessageBack;
        messageText = ui.messageText;

        climbPrompt = ui.climbPrompt;

        healthBar = ui.healthBar;
        staminaBar = ui.staminaBar;
        foodBar = ui.foodBar;
    }

    private void Update() {
        ps.localPaused = mainPauseMenu.activeInHierarchy || settingsMenu.activeInHierarchy;

        if (Input.GetButtonDown("Pause")) {
            if (currentMenu != null)
                ResumeGame();
            else
                SwapToMenu(mainPauseMenu);
        }

        if (Input.GetButtonDown("Inventory") && !ps.localPaused)
            SwapToMenu(mainInventoryMenu);

        StatBars();

        //Cursor.lockState = ((currentMenu != null) ? CursorLockMode.None : CursorLockMode.Locked);
    }

    public void OnClimbable(bool climbable) {
        climbPrompt.SetActive(climbable && !ps.climbing);
    }

    public void OnClimb(bool climbing) {
        climbPrompt.SetActive(!climbing);
    }

    void StatBars() {
        healthBar.size = (ps.health / ps.maxHealth);
        staminaBar.size = (ps.stamina / ps.maxStamina);
        foodBar.size = (ps.food / ps.maxFood);
    }

    void SwapToMenu(GameObject menu) {
        if (currentMenu != null)
            currentMenu.SetActive(false);

        menuParent.SetActive(true);
        menu.SetActive(true);
        currentMenu = menu;
    }

    public void OpenSettings() {
        SwapToMenu(settingsMenu);
    }

    public void ResumeGame() {
        if (currentMenu != null)
            currentMenu.SetActive(false);

        menuParent.SetActive(false);
        currentMenu = null;
    }

    public void ExitGame() {
        LevelManager.lm.BackToMain();
    }

    public void BackToMain() {
        SwapToMenu(mainPauseMenu);
    }

}
