using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyButton : MonoBehaviour {
    public Button readyButton;
    public Image color;

    public PlayerInfo player;

	// Use this for initialization
	void Awake () {
        color = GetComponent<Image>();
        if (player.isReady)
        {
            color.color = Color.green;
        }
        else {
            color.color = Color.red;
        }
	}

    public void readyClick() {
        if (color == null)
        {
            color = GetComponent<Image>();
        }
        if (player.isReady)
        {
            this.color.color = Color.green;
        }
        else {
            this.color.color = Color.red;
        }
    }
}
