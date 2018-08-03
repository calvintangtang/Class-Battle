using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : Singleton<PlayerControl> {

    public PlayerInfo curPlayer;

    public bool createPlayer(string playerName) {
        if (playerName.Equals("")) {
            return false;
        }
        curPlayer = new PlayerInfo(playerName);
        return true;
    }

    // Use this for initialization
    void Start () {
        SaveLoad.Load();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
