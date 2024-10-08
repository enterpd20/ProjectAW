using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class GearDataLoader
{
    private static List<Gear> gears = new List<Gear>(); // 모든 장비 데이터를 담을 리스트

    public static void LoadAllGears()
    {
        //List<Gear> allGears = new List<Gear>();

        gears.Clear(); // 기존 장비 데이터를 지우고 새로 로드

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

        // StreamingAssets 폴더에서 파일 가져옴
        foreach(string file in gearFiles)
        {
            string filePath = 
                Path.Combine(Application.streamingAssetsPath, file + ".json");

            if(File.Exists(filePath))
            {
                string jsonContent = File.ReadAllText(filePath);
                GearList gearList = JsonUtility.FromJson<GearList>(jsonContent);
                gears.AddRange(gearList.gears);

                //Debug.Log($"Loading file from path: {filePath}");
                //Debug.Log($"Loaded JSON content from {file}: {jsonContent}");   // JSON 내용을 출력
                //Debug.Log($"Successfully loaded gears from {file}");
            }
            else
            {
                Debug.LogWarning($"Failed to load gear data from file: {file}");
            }
        }

        // 로드된 장비 개수 출력
        //Debug.Log($"Total gears loaded: {gears.Count}");

        //return allGears;
    }

    // 이름을 기반으로 장비를 검색
    public static Gear GetGearByName(string gearName)
    {
        //List<Gear> allGears = LoadAllGears();

        // 모든 장비 데이터가 이미 로드되었는지 확인
        if (gears.Count == 0)
        {
            Debug.LogWarning("Gear data not loaded. Loading now...");
            LoadAllGears();
        }

        //Debug.Log($"Attempting to find gear: {gearName}");

        //Gear findGear = allGears.Find(gear => gear.name.Equals(gearName, System.StringComparison.OrdinalIgnoreCase));        
        Gear findGear = gears.Find(gear => gear.name.Equals(gearName, System.StringComparison.OrdinalIgnoreCase));

        if (findGear != null)
        {
            //Debug.Log($"Found gear: {findGear.name}");
        }
        else
        {
            Debug.LogWarning($"Gear not found: {gearName}");
        }

        return findGear;
    }
}
