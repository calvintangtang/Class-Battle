using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterStats {
    public int curHP;
    public int maxHP;
    public int curMP;
    public int maxMP = 20;
    public int attackPower;

    public void init() {
        curHP = maxHP;
        curMP = maxMP;
    }

    public void takeDamage(int dmg) {
        curHP -= dmg;
        if (curHP <= 0) { curHP = 0; }
    }
}
