using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PartyInfo {
    public int id;
    public PlayerInfo host;
    public string partyName;
    public string partyPass;
    public List<PlayerInfo> userList = new List<PlayerInfo>();

    public PartyInfo(int _id, PlayerInfo _host, string _partyName, string _partyPass) {
        host = _host;
        userList[0] = host;
        partyName = _partyName;
        partyPass = _partyPass;
    }
}
