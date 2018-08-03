using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PartyScreenViewModel : Singleton<PartyScreenViewModel> {
    [System.Serializable]
    public class SlotUI {
        public Text playerName;
        public GameObject classType;
        public GameObject classUp;
        public GameObject classDown;
        public GameObject charSelect;
        public Button readyButton;

        public void disableUI() {
            this.classType.SetActive(false);
            this.classUp.SetActive(false);
            this.classDown.SetActive(false);
            this.charSelect.SetActive(false);
            this.readyButton.interactable = false;
        }
    }

    public PartyInfo curParty;

    public Text partyName;
    public List<SlotUI> playerSlotList;
    public List<PlayerInfo> playerList = new List<PlayerInfo>();

    public CharacterInfo character;
    

    public GameObject charSelectView;
    public CharSelectViewModel charSelectViewModel;

    public void reloadView()
    {
        curParty = PartyControl.Instance.curParty;
        partyName.text = curParty.partyName;

        playerList = curParty.userList;
        for (int i = 0; i < 4; i++) {
            if (i > playerList.Count - 1) {
                SlotUI slot = playerSlotList[i];
                slot.disableUI();
                slot.playerName.text = "";
            }
            else if (playerList[i] != null) {
                SlotUI slot = playerSlotList[i];
                PlayerInfo player = (PlayerInfo) playerList[i];
                Debug.Log(player.id + " " + Websocket.Instance.curPlayer.id);
                if (player.id != Websocket.Instance.curPlayer.id) {
                    slot.disableUI();
                    slot.readyButton.GetComponent<ReadyButton>().player = player;
                    slot.readyButton.GetComponent<ReadyButton>().readyClick();
                }
                slot.playerName.text = player.playerName;
                slot.readyButton.GetComponent<ReadyButton>().player = player;
                slot.readyButton.GetComponent<ReadyButton>().readyClick();
            }
        }
    }

    private void OnEnable()
    {
        reloadView();
    }

    public void charSelect()
    {
        charSelectView.SetActive(true);
        charSelectViewModel.character = character;
        charSelectViewModel.init();
    }

    public void closecharSelect()
    {
        charSelectView.SetActive(false);
    }

    public void changeClass(bool up) {
        int classIndex = (int)character.charClass;
        if (up)
        {
            classIndex++;
            if (classIndex >= Enum.GetNames(typeof(ClassType)).Length)
                classIndex = 0;
        }
        else
        {
            classIndex--;
            if (classIndex < 0)
                classIndex = Enum.GetNames(typeof(ClassType)).Length - 1;
        }
        character.charClass = (ClassType)classIndex;
    }

    public void startGame() {
        foreach (PlayerInfo player in playerList)
        {
            if (!player.isReady)
            {
                return;
            }
        }
        BattleManager.charInfo = character;
        BattleManager.playInfo = null;
        SceneManager.LoadScene("BattleScene");
        SoundManager.Instance.enterGame();
    }
}