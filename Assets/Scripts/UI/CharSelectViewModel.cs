using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharSelectViewModel : MonoBehaviour {
    public Text weapon;
    public Text boot;
    public Button weaponUp;
    public Button weaponDown;
    public Button bootsUp;
    public Button bootsDown;

    public CharacterInfo character;

    Weapon[] weaponListByClass;
    int weaponIndex = 0;

    public void init() {
        switch (character.charClass)
        {
            case ClassType.Swordsman:
                weaponListByClass = CharacterInfo.swordWeapons;
                break;
            case ClassType.Marksman:
                weaponListByClass = CharacterInfo.gunWeapons;
                break;
            case ClassType.Mage:
                weaponListByClass = CharacterInfo.mageWeapons;
                break;
            default:
                weaponListByClass = CharacterInfo.swordWeapons;
                break;
        }

        if (weaponListByClass.Contains(character.weapon))
        {
            weaponIndex = Array.IndexOf(weaponListByClass, character.weapon);
        }

        character.weapon = weaponListByClass[weaponIndex];
        weapon.text = character.weapon + "";
    }
    public void changeWeapon(bool up)
    {
        if (up)
        {
            weaponIndex++;
            if (weaponIndex >= weaponListByClass.Length)
                weaponIndex = 0;
        }
        else
        {
            weaponIndex--;
            if (weaponIndex < 0)
                weaponIndex = weaponListByClass.Length - 1;
        }
        character.weapon = weaponListByClass[weaponIndex];
        weapon.text = character.weapon + "";
    }

    public void changeBoots(bool up)
    {
        int bootsIndex = (int)character.boots;
        if (up)
        {
            bootsIndex++;
            if (bootsIndex >= Enum.GetNames(typeof(Boots)).Length)
                bootsIndex = 0;
        }
        else
        {
            bootsIndex--;
            if (bootsIndex < 0)
                bootsIndex = Enum.GetNames(typeof(Boots)).Length - 1;
        }
        character.boots = (Boots)bootsIndex;
        boot.text = character.boots + "";
    }
}
