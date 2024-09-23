using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DockDetailUI_EquipStat : MonoBehaviour
{    
    [SerializeField] private StatBG[] statBGs;      // UI_StatBG 0~3 배열로 할당
    [SerializeField] private Text equipNameText;    // 장비의 이름을 받아와 스탯을 찾고 배정

    [System.Serializable]
    public class StatBG
    {
        public Text statTypeText;
        public Text statValueText;
    }

    private void Start()
    {
        string equipName = equipNameText.text;
        Gear matchingGear = GearDataLoader.GetGearByName(equipName);
        if (matchingGear != null)
        {
            SetGearInfo(matchingGear);
        }
        else
        {
            Debug.LogWarning($"Gear with name {equipName} not found");
        }        
    }

    public void SetGearInfo(Gear gear)
    {
        //DisplayGearName(gear.name);
        DisplayStats(gear);
    }

    //private void DisplayGearName(string gearName)
    //{
    //    Text equipNameText = transform.Find("UI_EquipName").GetComponent<Text>();
    //    if(equipNameText != null)
    //    {
    //        equipNameText.text = gearName;
    //    }
    //}

    private void DisplayStats(Gear gear)
    {
        Gear.GearStats stats = gear.stats;

        // 항공기인 경우와 나머지를 구분
        bool isAircraft = (gear.gearType == "Torpedo Bomber" || gear.gearType == "Dive Bomber");

        // 장비 스탯 정리
        List<(string, float)> statList = new List<(string, float)>
        {
            ("HP", stats.HP),
            ("AVI", stats.AVI),
            ("SPD", stats.SPD),
            ("AA", stats.AA),
            ("FP", stats.FP),
            ("TRP", stats.TRP),
            ("DMG", stats.DMG),
            ("RLD", stats.RLD)
        };

        // 유효한 스탯만 필터링
        List<(string, float)> validStats = statList.FindAll(stat => stat.Item2 != 0);

        // 항공기의 경우, SPD를 제외한 4개의 스탯만 표시
        if(isAircraft)
        {
            validStats.RemoveAll(stat => stat.Item1 == "SPD");                      // SPD 제외
            validStats = validStats.GetRange(0, Mathf.Min(4, validStats.Count));    // 4개의 스탯 표시
        }
        else
        {
            validStats = validStats.GetRange(0, Mathf.Min(4, validStats.Count));
        }

        // UI에 스탯을 표시
        for (int i = 0; i < statBGs.Length; i++)
        {
            if (i < validStats.Count)
            {
                //Text statTypeText = statBGs[i].transform.Find("Text_StatType").GetComponent<Text>();
                //Text statValueText = statBGs[i].transform.Find("Text_StatValue").GetComponent<Text>();

                statBGs[i].statTypeText.text = validStats[i].Item1;
                statBGs[i].statValueText.text = validStats[i].Item2.ToString();
            }
            else
            {
                //Text statTypeText = statBGs[i].transform.Find("Text_StatType").GetComponent<Text>();
                //Text statValueText = statBGs[i].transform.Find("Text_StatValue").GetComponent<Text>();

                statBGs[i].statTypeText.text = "";
                statBGs[i].statValueText.text = "";
            }
        }
    }
}
