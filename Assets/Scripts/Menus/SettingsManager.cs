using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour {

    public static SettingsManager sm;

    public float sensitivity = 160;

    private void Awake() {
        sm = this;
    }

}
