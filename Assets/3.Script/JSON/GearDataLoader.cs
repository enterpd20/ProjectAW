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

        // Resources 폴더에서 파일 가져옴
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

    // 이름을 기반으로 장비를 검색
    public static Gear GetGearByName(string gearName)
    {
        List<Gear> allGears = LoadAllGears();
        return allGears.Find(gear => gear.name == gearName);
    }
}
