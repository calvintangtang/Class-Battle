using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReplayControl : Singleton<ReplayControl> {
    public GameObject buttonPanel;
    public Button buttonTemplate;
    public static Dictionary<string, List<List<byte[]>>> SaveBattleDict = new Dictionary<string, List<List<byte[]>>>();

    void Start()
    {
        SaveLoad.Load(); 
        foreach (string name in SaveBattleDict.Keys) {
            addButton(name, SaveBattleDict[name]);
        }
    }

    private void addButton(string replayName, List<List<byte[]>> list)
    {
        Button replayBtn = Instantiate<Button>(buttonTemplate);
        replayBtn.GetComponentInChildren<Image>().GetComponentInChildren<Text>().text = replayName;
        replayBtn.GetComponentInChildren<Text>().text = replayName;
        replayBtn.transform.SetParent(buttonPanel.transform);
        replayBtn.onClick.AddListener(delegate { startReplay(list); });
    }


    public void startReplay(List<List<byte[]>> replayData) {
        BattleManager.replayData = replayData;
        BattleManager.bReplay = true;
        SceneManager.LoadScene("BattleScene");
    }
}
