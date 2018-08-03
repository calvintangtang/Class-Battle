using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum SceneType
{
    Title = 0,
    Main = 1,
    OfflineMenu = 11,
    Create = 2,
    Party = 21,
    ClassSelect = 22,
    Join = 3,
    Training = 4,
    Settings = 5,
    OfflineSettings = 51,
    Error = 6,
    CreatePlayer = 7,
    ReplayScreen = 8,
    OfflineReplay = 81
}

public class SceneControl : Singleton<SceneControl> {
    [System.Serializable]
    public class SceneInfo {
        public SceneType type;
        public GameObject sceneObject;
    }

    public List<SceneInfo> sceneList;

    public void goToScene(SceneType type) {
        foreach (SceneInfo item in sceneList) {
            if (item.type != type) {
                item.sceneObject.SetActive(false);
            } else {
                item.sceneObject.SetActive(true);
            }
        }
    }

    public void goToSceneID(int id) {
        goToScene((SceneType)id);
    }

    public Text errorText;
    public GameObject backButtonerror;
    public int prevScreen = 7;

    public void error(string message, int _prevScreen) {
        errorText.text = message;
        prevScreen = _prevScreen;
        goToSceneID(6);
    }

    public void SetBackButton() {
        goToSceneID(prevScreen);
    }

    public Text partyName;
    public void setPartyName(string name) {
        partyName.text = name;
    }

    public void startBattle() {
        SceneManager.LoadScene("BattleScene");
    }
    
	void Start () {
        foreach (SceneInfo item in sceneList)
        {
            if (item.type != 0)
            {
                item.sceneObject.SetActive(false);
            } else {
                item.sceneObject.SetActive(true);
            }
        }
    }
}
