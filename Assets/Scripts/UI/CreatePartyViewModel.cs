using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatePartyViewModel : MonoBehaviour {

    public InputField partyName;
    public InputField partyPass;

    public void createParty()
    {
        
        if (partyName.text.Equals("")) {
            SceneControl.Instance.error("Please enter a Party Name.", 2);
            return;
        }

        bool success = PartyControl.Instance.createParty(partyName.text, partyPass.text, PlayerControl.Instance.curPlayer);
    }
}
