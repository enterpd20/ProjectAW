using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterStats
{
    public float HP;
    public float FP;
    public float TRP;
    public float AVI;
    public float AA;
    public float SPD;
}

[System.Serializable]
public class Character
{
    public string name;
    public string shipType;
    public string rarity;
    public string faction;
    public string imageName;
    public string prefabName;
    public bool hasUniqueModule;
    public CharacterStats stats;

    public List<string> eqiuppedGears;
    //public List<Gear> availableGears;

}

[System.Serializable]
public class CharacterList
{
    public List<Character> Characters;
}