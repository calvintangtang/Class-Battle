using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {
    public Animator animator;
    public bool isDead;

    public float atk1Duration;
    public float atk1HitboxDuration;
    public GameObject atk1Hitbox;

    // Use this for initialization
    void Start () {
        animator = GameObject.FindObjectOfType<Animator>();
        this.GetComponent<CharacterCombat>().getHurtAction += onHurt;
        this.GetComponent<CharacterCombat>().onDeathAction += death;
    }
	
	// Update is called once per frame
	void LateUpdate () {
        if (isDead) { return; }
	}

    public void onHurt()
    {
        if (isDead)
        {
            return;
        }
        animator.SetTrigger("Hurt");
    }

    public void death()
    {
        animator.SetTrigger("Death");
        isDead = true;
        Destroy(GetComponent<Rigidbody2D>());
        GetComponent<CapsuleCollider2D>().enabled = false;
    }
}
