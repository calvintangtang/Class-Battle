using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatingBackground : MonoBehaviour {
    public Transform bg1;
    public Transform bg2;
    private bool bg1bool = true;
    public Transform camera;

    private float currentLocation = 20f;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (currentLocation < camera.position.x)
        {
            if (bg1bool)
            {
                bg1.localPosition = new Vector3(bg1.localPosition.x + 40, 0, 5);
            }
            else
            {
                bg2.localPosition = new Vector3(bg2.localPosition.x + 40, 0, 5);
            }
            currentLocation += 25f;
            bg1bool = !bg1bool;
        }
        else if (currentLocation > camera.position.x + 40)
        {
            if (bg1bool)
            {
                bg2.localPosition = new Vector3(bg2.localPosition.x - 40, 0, 5);
            }
            else
            {
                bg1.localPosition = new Vector3(bg1.localPosition.x - 40, 0, 5);
            }
            currentLocation -= 25f;
            bg1bool = !bg1bool;
        }
	}
}
