using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HealthPackData {
    public float xPos;
    public float yPos;
}

public class HealthPack : MonoBehaviour {
    public int healAmount = 20;
    public AudioClip healSound;

    public HealthPack(Vector3 position) {
        this.transform.position = position;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.GetComponent<CharacterCombat>() != null)
        {
            CharacterCombat target = coll.gameObject.GetComponent<CharacterCombat>();
            target.receiveHealth(this);
            SoundManager.Instance.PlaySingle(healSound);
        }
    }

    public void destroyPack() {
        Destroy(gameObject);
    }
}
