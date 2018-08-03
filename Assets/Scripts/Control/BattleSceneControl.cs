using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class BattleSceneControl : Singleton<BattleSceneControl>, ITickRecv {
    public GameObject swordsmanTemplate;
    public GameObject marksmanTemplate;
    public GameObject mageTemplate;
    public GameObject enemyTemplate;
    public GameObject HealthTemplate;
    public Transform startGameAnchor;
    public Transform characterHolder;

    public List<GameObject> playerList;
    public List<GameObject> enemyList;
    public List<CharacterCombat> playerCombatList;
    public List<CharacterCombat> enemyCombatList;
    public int enemyCounter;

    private GameObject player;
    public CharacterInfo charInfo;
    public HPBar playerHPBar;
    public Button atkButton;
    public Button specialButton;

    public GameObject pauseScreen;
    public GameObject endGameScreen;
    public Text endGameText;
    public bool bReplay;

    public void initReplay(List<List<byte[]>> replayData)
    {
        bReplay = true;
        initGame(BattleManager.charInfo, BattleManager.playInfo, replayData);
    }

    public void initGame(CharacterInfo charInfo, PlayerInfo playInfo, List<List<byte[]>> replayData) {
        if (BattleManager.bReplay)
        {
            TickManager.Instance.StartReplayTick(replayData);
        }
        else
        {
            TickManager.Instance.StartTick();
        }
        if (charInfo == null)
        {
            this.charInfo = new CharacterInfo(ClassType.Swordsman, Weapon.Blaster, Boots.Armor);
        }
        else {
            this.charInfo = charInfo;
        }
        RegisterTickRev();
        TickManager.Instance.bRunning = true;
    }

    public void spawnPlayer(CharacterInfo charInfo) {
        if (playerList.Count <= 4) {
            GameObject charTemplate = null;
            switch (charInfo.charClass)
            {
                case ClassType.Swordsman:
                    charTemplate = swordsmanTemplate;
                    break;
                case ClassType.Marksman:
                    charTemplate = marksmanTemplate;
                    break;
                case ClassType.Mage:
                    charTemplate = mageTemplate;
                    break;
                default:
                    break;
            }
            
            player = Instantiate<GameObject>(charTemplate);
            player.transform.position = startGameAnchor.position;
            player.GetComponent<PlayerMovement>().charInfo = charInfo;

            Button.ButtonClickedEvent atk1Event = new Button.ButtonClickedEvent();
            atk1Event.AddListener(player.GetComponent<PlayerMovement>().attackButton);
            atkButton.onClick = atk1Event;

            Button.ButtonClickedEvent specialEvent = new Button.ButtonClickedEvent();
            specialEvent.AddListener(player.GetComponent<PlayerMovement>().specialButton);
            specialButton.onClick = specialEvent;

            CameraControl.Instance.player = player;
            playerHPBar.charComb = player.GetComponent<CharacterCombat>();
            PlayerMovement playMove = player.GetComponent<PlayerMovement>();
            playMove.bPlayer = true;

            playerList.Add(player);
            CharacterCombat playerCombat = player.GetComponent<CharacterCombat>();
            playerCombatList.Add(playerCombat);
            
            SendTick();
        }
    }

    public void spawnEnemy(Vector3 position) {
        enemyCounter++;
        GameObject enemy = Instantiate<GameObject>(enemyTemplate);
        enemy.transform.position = position;
        enemyList.Add(enemy);
        enemy.transform.parent = characterHolder;

        SendTick();
    }

    public void addEnemy(GameObject enemy) {
        enemyList.Add(enemy);
        CharacterCombat enemyCombat = enemy.GetComponent<CharacterCombat>();
        enemyCombatList.Add(enemyCombat);
    }

    public void pause() {
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
    }

    public void unpause() {
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
    }

    public void destroyPlayer() {
        Destroy(this.player);
    }

    public void dropItem(Vector3 position) {
        GameObject item = Instantiate<GameObject>(HealthTemplate);
        item.transform.position = position;
    }

    public void checkGameOver() {
        bool gameOver = true;
        foreach (CharacterCombat enemyCombat in enemyCombatList) {
                gameOver = gameOver && (enemyCombat.stats.curHP <= 0);
        }

        if (gameOver)
        {
            StartCoroutine(VictoryScreen());
            TickManager.Instance.bRunning = false;
            return;
        }

        gameOver = true;
        foreach (CharacterCombat playerCombat in playerCombatList)
        {
            gameOver = gameOver && (playerCombat.stats.curHP <= 0);
        }

        if (gameOver) {
            TickManager.Instance.bRunning = false;
            DefeatScreen();
            return;
        }
    }

    public IEnumerator VictoryScreen() {
        foreach (GameObject player in playerList)
        {
            PlayerMovement curPlayer = player.GetComponent<PlayerMovement>();
            curPlayer.cheerAnimation();
        }
        yield return new WaitForSeconds(5);
        TickManager.Instance.endBattle();
        endGameText.text = "Victory!";
        endGameScreen.SetActive(true);
    }

    public void DefeatScreen()
    {
        TickManager.Instance.endBattle();
        endGameText.text = "Defeat";
        endGameScreen.SetActive(true);
    }

    public void BackToMenu() {
        SceneManager.LoadScene("MainScene");
        SoundManager.Instance.exitGame();
    }

    void Update()
    {
        if (player == null) { return; }
        if (TickManager.Instance.bRunning) {
            checkGameOver();
        }
    }

    //Tick System
    public List<byte[]> tickList = new List<byte[]>();
    PlayerTickData currentTick;

    public void RegisterTickRev()
    {
        TickManager.Instance.registerTickRecv(this);
    }

    public void SendTick()
    {
        if (player == null)
        {
            PlayerTickData data = new PlayerTickData();
            data.dataType = (int)TickDataType.SpawnPlayer;
            SpawnPlayerTickData spawnData = new SpawnPlayerTickData();
            spawnData.peerId = 0;
            spawnData.playerClass = (int)charInfo.charClass;
            spawnData.weapon = (int)Weapon.Blaster;
            data.data = spawnData;

            byte[] byteData = ObjectToByteArray<PlayerTickData>(data);
            tickList.Add(byteData);
            TickManager.Instance.ReceiveTick(byteData);
        }
        else {
            PlayerTickData data = new PlayerTickData();
            data.dataType = (int)TickDataType.SpawnEnemy;
            SpawnEnemyTickData spawnData = new SpawnEnemyTickData();
            Vector3 randomPos = player.transform.position + new Vector3(9, -2 + Random.value * 4);
            spawnData.xPos = randomPos.x;
            spawnData.yPos = randomPos.y;
            spawnData.zPos = randomPos.z;
            data.data = spawnData;

            byte[] byteData = ObjectToByteArray<PlayerTickData>(data);
            tickList.Add(byteData);
            TickManager.Instance.ReceiveTick(byteData);
        }
    }

    public void UpdateTick(List<byte[]> tickDataList)
    {
        PlayerTickData receivedData = null;

        foreach (var tickByte in tickDataList)
        {
            PlayerTickData data = ByteArrayToObject<PlayerTickData>(tickByte);
            RunTick(data);
        }
        SendTick();
    }

    public void RunTick(PlayerTickData tickData = null)
    {
        if (tickData != null)
        {
            if (tickData.dataType == (int)TickDataType.SpawnPlayer)
            {
                SpawnPlayerTickData spawnData = tickData.data as SpawnPlayerTickData;
                CharacterInfo charInfo = new CharacterInfo((ClassType)spawnData.playerClass, (Weapon)spawnData.weapon, Boots.Armor);
                spawnPlayer(charInfo);
            }
            else if (tickData.dataType == (int)TickDataType.SpawnEnemy)
            {
                if (TickManager.Instance.Tick % 50 == 0)
                {
                    SpawnEnemyTickData spawnData = tickData.data as SpawnEnemyTickData;
                    Vector3 position = new Vector3(spawnData.xPos, spawnData.yPos, spawnData.zPos);
                }
            }
            else if (tickData.dataType == (int)TickDataType.HealthPack)
            {
                HealthPackData hpData = tickData.data as HealthPackData;
                dropItem(new Vector3(hpData.xPos, hpData.yPos, 0));
            }
        }
    }

    // Convert an object to a byte array
    private byte[] ObjectToByteArray<T>(T obj)
    {
        if (obj == null)
            return null;

        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, obj);

        return ms.ToArray();
    }

    // Convert a byte array to an Object
    private T ByteArrayToObject<T>(byte[] arrBytes)
    {
        MemoryStream memStream = new MemoryStream();
        BinaryFormatter binForm = new BinaryFormatter();
        memStream.Write(arrBytes, 0, arrBytes.Length);
        memStream.Seek(0, SeekOrigin.Begin);
        T obj = (T)binForm.Deserialize(memStream);

        return obj;
    }
}
