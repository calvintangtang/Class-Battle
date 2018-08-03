using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public enum CommandType
{ 
    CREATE,
    JOIN,
    LEAVE,
    PARTY_LIST,
    ERROR,
    APPROVE_CREATE,
    APPROVE_JOIN,
    APPROVE_LEAVE,
    RELOAD_VIEW,
    CREATE_PLAYER,
    READY,
    LOAD_CONFIRM
}

[System.Serializable]
public class Command {
    public int type;
}

[System.Serializable]
public class CreateCommand : Command {
    public PlayerInfo creator;
    public string partyName;
    public string partyPass;
}

[System.Serializable]
public class JoinCommand : Command {
    public string partyName;
    public PlayerInfo player;
}

[System.Serializable]
public class LeaveCommand : Command {
    public PartyInfo party;
    public PlayerInfo player;
}

[System.Serializable]
public class PartyListCommand : Command {
    public PlayerInfo player;
}

[System.Serializable]
public class ErrorMessage : Command {
    public string errorMessage;
    public int prevScreen;
}

[System.Serializable]
public class ApproveCreate : Command {
    public PartyInfo party;
}

[System.Serializable]
public class ApproveJoin : Command{
    public PartyInfo party;
}

[System.Serializable]
public class PartyList : Command {
    public List<PartyInfo> partyList;
    public PlayerInfo player;
}

[System.Serializable]
public class ReloadView : Command {
    public PartyInfo party;
}

[System.Serializable]
public class CreatePlayer : Command
{
    public int playerID;
}

[System.Serializable]
public class ReadyCommand : Command {
    public string playerName;
    public string partyName;
    public bool isReady;
}

[System.Serializable]
public class LoadConfirmation : Command {
    public string playerName;
}

public class Websocket : MonoBehaviour {
    public static Websocket Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public bool connected;
    private WebSocket currentSocket = null;
    public Queue<string> messageQueue = new Queue<string>();

    // UI Elements
    public InputField partyName;
    public InputField partyPass;

    // Data
    public string playerName;
    public PlayerInfo curPlayer;
    public PartyInfo curParty;
    public bool load = false;
	
	void Update () {
        if (messageQueue.Count > 0) {
            string message = messageQueue.Dequeue();
            Command curCommand = JsonUtility.FromJson<Command>(message);
            if (curCommand.type == (int)CommandType.ERROR)
            {
                ErrorMessage eMessage = JsonUtility.FromJson<ErrorMessage>(message);
                SceneControl.Instance.error(eMessage.errorMessage, eMessage.prevScreen);
            }
            else if (curCommand.type == (int)CommandType.APPROVE_CREATE)
            {
                ApproveCreate appr = JsonUtility.FromJson<ApproveCreate>(message);
                curParty = appr.party;
                PartyControl.Instance.curParty = appr.party;
                curPlayer.isHost = true;
                SceneControl.Instance.goToSceneID(21);
            }
            else if (curCommand.type == (int)CommandType.APPROVE_JOIN)
            {
                ApproveJoin appr = JsonUtility.FromJson<ApproveJoin>(message);
                curParty = appr.party;
                PartyControl.Instance.curParty = appr.party;
                SceneControl.Instance.goToSceneID(21);
            }
            else if (curCommand.type == (int)CommandType.APPROVE_LEAVE)
            {
                curParty = null;
                PartyControl.Instance.curParty = null;
                curPlayer.isHost = false;
                curPlayer.isReady = false;
            }
            else if (curCommand.type == (int)CommandType.READY)
            {
                ReadyCommand cmd = JsonUtility.FromJson<ReadyCommand>(message);
                foreach (PlayerInfo player in PartyControl.Instance.curParty.userList)
                {
                    if (player.playerName.Equals(cmd.playerName))
                    {
                        player.isReady = cmd.isReady;
                    }
                }
                PartyScreenViewModel.Instance.reloadView();
            }
            else if (curCommand.type == (int)CommandType.PARTY_LIST)
            {
                PartyList pList = JsonUtility.FromJson<PartyList>(message);
                JoinPartyViewModel.Instance.displayParties(pList.partyList);
            }
            else if (curCommand.type == (int)CommandType.RELOAD_VIEW)
            {
                ReloadView reloadView = JsonUtility.FromJson<ReloadView>(message);
                curParty = reloadView.party;
                PartyControl.Instance.curParty = reloadView.party;
                PartyScreenViewModel.Instance.reloadView();
            }
            else if (curCommand.type == (int)CommandType.CREATE_PLAYER)
            {
                CreatePlayer crtPlay = JsonUtility.FromJson<CreatePlayer>(message);
                curPlayer.id = crtPlay.playerID;
            }
            else if (curCommand.type == (int)CommandType.LOAD_CONFIRM)
            {
                load = true;
            }
        }
	}

    public void connect() {
        if (!connected && !playerName.Equals("")) {
            var ws = new WebSocket("ws://localhost:9000/connect?username=" + playerName);
            {
                ws.Connect();
                currentSocket = ws;
                curPlayer = new PlayerInfo(playerName);
                connected = true;

                ws.OnMessage += (Sender, e) =>
                {
                    messageQueue.Enqueue(e.Data);
                };
            }
        }
    }

    public void CreateParty(PlayerInfo creator, string partyName, string partyPass) {
        CreateCommand cmd = new CreateCommand();
        cmd.creator = creator;
        cmd.partyName = partyName;
        cmd.partyPass = partyPass;

        string jsonCmd = JsonUtility.ToJson(cmd); 
        currentSocket.Send(jsonCmd);
    }

    public void JoinParty(PartyInfo party)
    {
        JoinCommand cmd = new JoinCommand();
        cmd.type = (int)CommandType.JOIN;
        cmd.partyName = party.partyName;
        cmd.player = curPlayer;
        string jsonCmd = JsonUtility.ToJson(cmd);
        currentSocket.Send(jsonCmd);
    }

    public void sendReady() {
        ReadyCommand cmd = new ReadyCommand();
        cmd.type = (int)CommandType.READY;
        cmd.playerName = curPlayer.playerName;
        cmd.partyName = curParty.partyName;
        cmd.isReady = !curPlayer.isReady;

        string jsonCmd = JsonUtility.ToJson(cmd);
        currentSocket.Send(jsonCmd);
        curPlayer.isReady = !curPlayer.isReady;
    }

    public void leaveParty() {
        LeaveCommand cmd = new LeaveCommand();
        cmd.type = (int)CommandType.LEAVE;
        cmd.party = curParty;
        cmd.player = curPlayer;

        string jsonCmd = JsonUtility.ToJson(cmd);
        currentSocket.Send(jsonCmd);
    }

    public void requestPartyList() {
        PartyList request = new PartyList();
        request.type = (int)CommandType.PARTY_LIST;
        request.player = curPlayer;

        string jsonCmd = JsonUtility.ToJson(request);
        currentSocket.Send(jsonCmd);
    }

    public void sendLoadConfirm() {
        LoadConfirmation load = new LoadConfirmation();
        load.type = (int)CommandType.LOAD_CONFIRM;
        load.playerName = playerName;

        string jsonCmd = JsonUtility.ToJson(load);
        currentSocket.Send(jsonCmd);
    }
}
