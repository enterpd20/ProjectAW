using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GearStats
{
    public float HP;
    public float AVI;
    public float SPD;
    public float AA;
    public float FP;
    public float TRP;
    public float DMG;
    public float RLD;
}

[System.Serializable]
public class Gear
{
    public string name;
    public string gearType;
    public string rarity;
    public string faction;
    public string imageName;
    public GearStats stats;
}

[System.Serializable]
public class GearList
{
    public List<Gear> Gears;
}

