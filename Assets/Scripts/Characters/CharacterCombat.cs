using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterChannel
{
    Ally,
    Enemy
}

public class CharacterCombat : MonoBehaviour {
    public CharacterChannel channel;
    public CharacterPreset presetData;
    public CharacterStats stats;
    public PlayerMovement playerMovement;
    public EnemyMovement enemyMovement;
    public bool isDead;

    public Action getHurtAction;
    public Action onDeathAction;

    public AudioClip damage1;
    public AudioClip damage2;
    public AudioClip damage3;
    public AudioClip damage4;

    void Start () {
        stats = presetData.stats.DeepClone<CharacterStats>();
        stats.init();
        StartCoroutine(regenHP());
        StartCoroutine(regenMP());
        isDead = false;
        if (channel == CharacterChannel.Ally)
        {
            this.playerMovement = GetComponent<PlayerMovement>();
            this.enemyMovement = null;
        }
        else {
            this.enemyMovement = GetComponent<EnemyMovement>();
            this.playerMovement = null;
        }
	}

    public void receiveHit(AttackHitbox hitbox)
    {
        if (hitbox.curChar.channel != this.channel) {
            stats.takeDamage(hitbox.curChar.stats.attackPower);
            SoundManager.Instance.RandomizeSFX(damage1, damage2, damage3, damage4);
            if (stats.curHP > 0)
            {
                if (getHurtAction != null)
                {
                    getHurtAction();
                }
            }
            else
            {
                if (onDeathAction != null)
                {
                    onDeathAction();
                }
            }
        } 
    }

    public void receiveHit(int damage) {
        stats.takeDamage(damage);
        SoundManager.Instance.RandomizeSFX(damage1, damage2, damage3, damage4);
        if (stats.curHP > 0)
        {
            if (getHurtAction != null)
            {
                getHurtAction();
            }
        }
        else
        {
            if (onDeathAction != null)
            {
                onDeathAction();
            }
        }
    }

    public void receiveHealth(HealthPack pack) {
        if (stats.curHP < stats.maxHP) {
            stats.curHP += pack.healAmount;
            if (stats.curHP > stats.maxHP)
            {
                stats.curHP = stats.maxHP;
            }
            pack.destroyPack();
        }
    }

    public IEnumerator regenHP() {
        if (this.channel == CharacterChannel.Ally) {
            while (!isDead)
            {
                if (stats.curHP <= 0)
                {
                    isDead = true;
                }
                if (stats.curHP < stats.maxHP)
                {
                    stats.curHP += 1;
                    yield return new WaitForSeconds(2);
                }
                else
                {
                    yield return null;
                }
            }
        }
    }

    public IEnumerator regenMP()
    {
        if (this.channel == CharacterChannel.Ally) {
            while (!isDead)
            {
                if (stats.curMP < stats.maxMP)
                {
                    stats.curMP += 1;
                    yield return new WaitForSeconds(2);
                }
                else
                {
                    yield return null;
                }
            }
        }
    }
}
