using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatePlayerViewModel : MonoBehaviour {
    public InputField playerName;
    public Button goButton;

    public void createPlayer() {
        if (playerName.text.Equals("")) {
            SceneControl.Instance.error("Please enter a Player Name.", 7);
            return;
        }
        Websocket.Instance.playerName = playerName.text;
        Websocket.Instance.connect();
        SceneControl.Instance.goToSceneID(1);
    }
}
