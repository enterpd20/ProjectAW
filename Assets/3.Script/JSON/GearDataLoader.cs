using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GearDataLoader : MonoBehaviour
{
    public GearList gearList;

    private string[] gearFiles =
    {
        "GearInfo_DDgun.json",
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

    private void Start()
    {
        LoadGearData();
    }

    private void LoadGearData()
    {
        // gearList √ ±‚»≠
        gearList = new GearList { Gears = new List<Gear>() };

       foreach (string fileName in gearFiles)
        {
            string path = Path.Combine(Application.streamingAssetsPath, fileName);
            if(File.Exists(path))
            {
                string json = File.ReadAllText(path);
                GearList loadedGearList = JsonUtility.FromJson<GearList>(json);
                gearList.Gears.AddRange(loadedGearList.Gears);
            }
            else
            {
                Debug.Log($"JSON file not found: {fileName}");
            }
        }
    }

    public Gear GetGearByName(string gearName)
    {
        return gearList.Gears.Find(gear => gear.name == gearName);
    }
}
