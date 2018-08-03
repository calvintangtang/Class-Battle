using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadExample : Singleton<SaveLoadExample> {

    public int level;
    public UnityEngine.UI.InputField input;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void SaveClick()
    {
        level = int.Parse(input.text);
        SaveLoad.Save();
    }
    public void LoadClick()
    {
        SaveLoad.Load();
    }
}
