using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour {
    public CharacterCombat curChar;

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.GetComponent<CharacterCombat>() != null)
        {
            CharacterCombat target = coll.gameObject.GetComponent<CharacterCombat>();
            target.receiveHit(this);
            GetComponent<Collider2D>().enabled = false;
        }
    }

    private void OnEnable()
    {
        GetComponent<Collider2D>().enabled = true;
    }
}
