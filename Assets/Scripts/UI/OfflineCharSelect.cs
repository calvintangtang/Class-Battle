using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class OfflineCharSelect : Singleton<OfflineCharSelect> {
    public Sprite swordsman;
    public Sprite marksman;
    public Sprite mage;
    public Image curClass;
    public Button classUp;
    public Button classDown;

    public CharacterInfo character;

    public void changeClass(bool up)
    {
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
        if (classIndex == 0)
        {
           curClass.sprite = swordsman;
        }
        else if (classIndex == 1)
        {
            curClass.sprite = marksman;
        }
        else {
            curClass.sprite = mage;
        }

        character.charClass = (ClassType)classIndex;
    }

    public void startGame()
    {
        BattleManager.charInfo = character;
        BattleManager.playInfo = null;
        SceneManager.LoadScene("BattleScene");
        SoundManager.Instance.enterGame();
    }
}
