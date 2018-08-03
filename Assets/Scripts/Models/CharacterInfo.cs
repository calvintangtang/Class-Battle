using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClassType {
    Swordsman = 0,
    Marksman,
    Mage
}
public enum Weapon {
    Longsword = 0,
    Shortsword,
    Daggers,
    Blaster,
    Pistols,
    Rifle,
    Spellbook,
    Staff,
    Wand
}
public enum Boots {
    Swiftness = 0,
    Armor,
    Stealth,
    Heal
}

[System.Serializable]
public class CharacterInfo {
    public ClassType charClass;
    public Weapon weapon;
    public Boots boots;

    public static Weapon[] swordWeapons = new Weapon[] { Weapon.Longsword, Weapon.Shortsword, Weapon.Daggers };
    public static Weapon[] gunWeapons = new Weapon[] { Weapon.Blaster, Weapon.Pistols, Weapon.Rifle };
    public static Weapon[] mageWeapons = new Weapon[] { Weapon.Spellbook, Weapon.Staff, Weapon.Wand };


    public CharacterInfo(ClassType type, Weapon _weapon, Boots _boots) {
        charClass = type;
        weapon = _weapon;
        boots = _boots;
    }
}
