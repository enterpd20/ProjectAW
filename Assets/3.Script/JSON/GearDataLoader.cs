using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GearDataLoader : MonoBehaviour
{
    public GearList gearList;

    private void Start()
    {
        LoadGearData();
    }

    private void LoadGearData()
    {
        string path = 
            Path.Combine(Application.streamingAssetsPath, 
            "GearInfo_DDgun.json", "GearInfo_CLCAgun", "GearInfo_BBgun", "GearInfo_Torpedo",
            "GearInfo_DiveBomber", "GearInfo_TorpedoBomber", "GearInfo_AntiAir", "GearInfo_Auxiliary");
        if(File.Exists(path))
        {
            string json = File.ReadAllText(path);
            gearList = JsonUtility.FromJson<GearList>(json);
        }
        else
        {
            Debug.Log("JSON file not found.");
        }
    }

    public Gear GetGearByName(string gearName)
    {
        return gearList.Gears.Find(gear => gear.name == gearName);
    }
}
