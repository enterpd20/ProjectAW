using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DockDetailUI_EquipStat : MonoBehaviour
{    
    [SerializeField] private GameObject[] statBGs;   // UI_StatBG 0~3 배열로 할당
    
    public void SetGearInfo(Gear gear)
    {
        DisplayGearName(gear.name);
        DisplayStats(gear.stats);
    }

    private void DisplayGearName(string gearName)
    {
        Text equipNameText = transform.Find("UI_EquipName").GetComponent<Text>();
        if(equipNameText != null)
        {
            equipNameText.text = gearName;
        }
    }

    private void DisplayStats(Gear.GearStats stats)
    {
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

        for(int i = 0; i < statList.Count; i++)
        {
            if(i < statList.Count)
            {
                //Text statTypeText = statBGs[i].transform.Find("Text_StatType").GetComponent<Text>();
                //Text statValueText = statBGs[i].transform.Find("Text_StatValue").GetComponent<Text>();

                Text statTypeText = statBGs[i].transform.Find("Text_StatType").GetComponent<Text>();
                Text statValueText = statBGs[i].transform.Find("Text_StatValue").GetComponent<Text>();

                statTypeText.text = statList[i].Item1;
                statValueText.text = statList[i].Item2.ToString();
            }
            else
            {
                statBGs[i].transform.Find("Text_StatType").GetComponent<Text>().text = "";
                statBGs[i].transform.Find("Text_StatValue").GetComponent<Text>().text = "";
            }
        }
    }   
}
