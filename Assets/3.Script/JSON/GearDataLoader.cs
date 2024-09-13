using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class GearDataLoader
{
    public static List<Gear> LoadAllGears()
    {
        List<Gear> allGears = new List<Gear>();

        string[] gearFiles =
        {
            "GearInfo_DDgun",
            "GearInfo_CLCAgun",
            "GearInfo_CLCASubgun",
            "GearInfo_BBgun",
            "GearInfo_BBSubgun",
            "GearInfo_Torpedo",
            "GearInfo_DiveBomber",
            "GearInfo_TorpedoBomber",
            "GearInfo_AntiAir",
            "GearInfo_Auxiliary"
        };

        // Resources �������� ���� ������
        foreach(string file in gearFiles)
        {
            TextAsset jsonFile = Resources.Load<TextAsset>(file);
            if(jsonFile != null)
            {
                GearList gearList = JsonUtility.FromJson<GearList>(jsonFile.text);
                allGears.AddRange(gearList.Gears);
            }
            else
            {
                Debug.LogWarning($"Failed to load gear data from file: {file}");
            }
        }

        return allGears;
    }

    // �̸��� ������� ��� �˻�
    public static Gear GetGearByName(string gearName)
    {
        List<Gear> allGears = LoadAllGears();
        return allGears.Find(gear => gear.name == gearName);
    }
}
