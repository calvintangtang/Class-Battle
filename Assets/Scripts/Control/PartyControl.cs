using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyControl : Singleton<PartyControl> {

    public List<PartyInfo> partyList = new List<PartyInfo>();
    public PartyInfo curParty;
    
    public bool createParty(string partyName, string password, PlayerInfo host)
    {
        if (partyName.Equals("")) {
            return false;
        }
        partyList.Add(curParty);
        Websocket.Instance.CreateParty(host, partyName, password);
        return true;
    }
}
