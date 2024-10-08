using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class GearDataLoader
{
    private static List<Gear> gears = new List<Gear>(); // ��� ��� �����͸� ���� ����Ʈ

    public static void LoadAllGears()
    {
        //List<Gear> allGears = new List<Gear>();

        gears.Clear(); // ���� ��� �����͸� ����� ���� �ε�

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

        // StreamingAssets �������� ���� ������
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
                //Debug.Log($"Loaded JSON content from {file}: {jsonContent}");   // JSON ������ ���
                //Debug.Log($"Successfully loaded gears from {file}");
            }
            else
            {
                Debug.LogWarning($"Failed to load gear data from file: {file}");
            }
        }

        // �ε�� ��� ���� ���
        //Debug.Log($"Total gears loaded: {gears.Count}");

        //return allGears;
    }

    // �̸��� ������� ��� �˻�
    public static Gear GetGearByName(string gearName)
    {
        //List<Gear> allGears = LoadAllGears();

        // ��� ��� �����Ͱ� �̹� �ε�Ǿ����� Ȯ��
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
