using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T>: MonoBehaviour where T:MonoBehaviour {
    static T _Instance;
    public static T Instance {
        get
        {
            if (_Instance == null)
            {
                _Instance = GameObject.FindObjectOfType<T>().GetComponent<T>();
            }
            return _Instance;
        }
    }
}