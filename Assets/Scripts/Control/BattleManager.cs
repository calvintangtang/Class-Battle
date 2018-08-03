using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour {
    public static CharacterInfo charInfo = new CharacterInfo(ClassType.Swordsman, Weapon.Blaster, Boots.Armor);
    public static PlayerInfo playInfo = null;
    public static List<List<byte[]>> replayData = null;
    public static bool bReplay = false;

    void Start () {
        if (bReplay)
        {
            Debug.Log(replayData.Count);
            BattleSceneControl.Instance.initReplay(replayData);
        }
        else {
            BattleSceneControl.Instance.initGame(charInfo, playInfo, null);
        }
	}
}
