using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinPartyViewModel : Singleton<JoinPartyViewModel> {
    public Button partyButtonTemplate;
    public GameObject buttonPanel;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void displayParties(List<PartyInfo> partyList) {
        Debug.Log(partyList.Count);
        foreach (PartyInfo party in partyList) {
            Button partyBtn = Instantiate<Button>(partyButtonTemplate);
            partyBtn.transform.SetParent(buttonPanel.transform);
            partyBtn.onClick.AddListener(delegate { Websocket.Instance.JoinParty(party); });
        }
    }
}
