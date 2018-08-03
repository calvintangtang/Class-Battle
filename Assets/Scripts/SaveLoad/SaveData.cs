using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{

    public static SaveData current;

    public int money;
    public int level;
    public Dictionary<string, List<List<byte[]>>> SaveBattleDict = new Dictionary<string, List<List<byte[]>>>();

    public SaveData()
    {
        current = this;
    }
    public void refreshCurrent()
    {

        //money = Globals.sharedInstance.money;
        //level = Globals.sharedInstance.level;
        SaveBattleDict.Add(Time.time.ToString(), TickManager.Instance.BattleTicks);
    }


    public void updateToGame()
    {
        //Globals.sharedInstance.money = money;
        //Globals.sharedInstance.level = level;

        ReplayControl.SaveBattleDict = SaveBattleDict;
    }

    public void setupNewData()
    {

    }


}
