using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlayerInfo  {
    public int id;
    public string playerName;
    public int classType;
    public bool isHost;
    public bool isReady;
    public bool isLoaded;

    public PlayerInfo(string _playerName) {
        playerName = _playerName;
        isReady = false;
        isLoaded = false;
        classType = (int)ClassType.Swordsman;
    }
}