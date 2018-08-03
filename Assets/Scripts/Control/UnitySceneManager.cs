using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitySceneManager : MonoBehaviour {
    static UnitySceneManager Instance;
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

    void OnEnable()
    {
        SceneManager.sceneLoaded += sendLoadedMessage;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= sendLoadedMessage;
    }

    public void sendLoadedMessage(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(scene.name);
        switch (scene.name)
        {
            case "MainScene":
                break;
            case "BattleScene":
                Websocket.Instance.sendLoadConfirm();
                break;
        }
    }

    public void goToBattle() {
        StartCoroutine(LoadLevelAsyncCoroutine("BattleScene"));
    }

    IEnumerator LoadLevelAsyncCoroutine(string sceneName)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);

        // this line prevents the scene from instant activation
        async.allowSceneActivation = false;
        if (Websocket.Instance.connected)
        {
            while (Websocket.Instance.load == false)
                yield return null;
        }
        // activate Scene now
        async.allowSceneActivation = true;

        yield return async;
    }
}
