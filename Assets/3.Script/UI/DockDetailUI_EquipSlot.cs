using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DockDetailUI_EquipSlot : MonoBehaviour
{
    [SerializeField] private Text equipTypeText;  // 장비 타입
    [SerializeField] private Text equipNameText;  // 장비 이름
    [SerializeField] private Image equipImage;    // 장비 이미지
    [SerializeField] private GameObject[] statBGs;
    [SerializeField] private Text slotNameText;    // 슬롯 이름

    public void SetEquipmentData(Gear gear)
    {
        // 장비 타입과 이름 설정
        equipTypeText.text = gear.gearType;
        equipNameText.text = gear.name;

        // 장비 이미지 불러오기(Resources/Images_Gear 폴더)
        Sprite gearSprite = Resources.Load<Sprite>("Images_Gear/" + gear.imageName);
        equipImage.sprite = gearSprite;
        
        // 스탯을 동적으로 설정(최대 4개까지)
        UpdateStats(gear.stats);
    }

    private void UpdateStats(GearStats stats)
    {
        // 초기화
        //foreach(GameObject statBG in statBGs)
        //{
        //    statBG.SetActive(false);
        //}

        int statIndex = 0;

        // HP
        if (stats.HP > 0 && statIndex < statBGs.Length)
        {
            SetStatText(statBGs[statIndex], "HP", stats.HP);
            statIndex++;
        }
        // AVI
        if (stats.AVI > 0 && statIndex < statBGs.Length)
        {
            SetStatText(statBGs[statIndex], "AVI", stats.AVI);
            statIndex++;
        }
        // SPD
        if (stats.SPD > 0 && statIndex < statBGs.Length)
        {
            SetStatText(statBGs[statIndex], "SPD", stats.SPD);
            statIndex++;
        }
        // AA
        if (stats.AA > 0 && statIndex < statBGs.Length)
        {
            SetStatText(statBGs[statIndex], "AA", stats.AA);
            statIndex++;
        }
        // FP
        if (stats.FP > 0 && statIndex < statBGs.Length)
        {
            SetStatText(statBGs[statIndex], "FP", stats.FP);
            statIndex++;
        }
        // TRP
        if (stats.TRP > 0 && statIndex < statBGs.Length)
        {
            SetStatText(statBGs[statIndex], "TRP", stats.TRP);
            statIndex++;
        }
        // DMG
        if (stats.DMG > 0 && statIndex < statBGs.Length)
        {
            SetStatText(statBGs[statIndex], "DMG", stats.DMG);
            statIndex++;
        }
        // RLD
        if (stats.RLD > 0 && statIndex < statBGs.Length)
        {
            SetStatText(statBGs[statIndex], "RLD", stats.RLD);
            statIndex++;
        }
        
        // 남은 statBG가 있으면 빈칸으로 설정
        for(; statIndex < statBGs.Length; statIndex++)
        {
            SetStatText(statBGs[statIndex], "", 0);
        }
    }

    // 스탯 정보를 UI에 할당하는 메서드
    private void SetStatText(GameObject statBG, string statType, float statValue)
    {
        Text statTyoeText = statBG.transform.Find("Text_StatType").GetComponent<Text>();
        Text statValueText = statBG.transform.Find("Text_StatValue").GetComponent<Text>();

        if(!string.IsNullOrEmpty(statType))
        {
            statTyoeText.text = statType;
            statValueText.text = statValue.ToString();
        }
        else
        {
            // 스탯이 없을 경우, 텍스트를 비워서 빈칸으로 보이게끔
            statTyoeText.text = "";
            statValueText.text = "";
        }
    }

    public void SetSlotType(Character character, int slotIndex)
    {
        string shipType = character.shipType;
        string slotType = "";

        switch(slotIndex)
        {
            case 1:
                slotType = shipType == "BB" ? "BB Gun" :
                           shipType == "CA/CL" ? "CL/CA Gun" :
                           shipType == "DD" ? "DD Gun" :
                           shipType == "CV" ? "Torpedo Bomber" : "";
                break;
            case 2:
                slotType = shipType == "BB" ? "BB Sub Gun" :
                           shipType == "CA/CL" ? "CL/CA Sub Gun" :
                           shipType == "DD" ? "DD Gun" :
                           shipType == "CV" ? "Torpedo Bomber" : "";
                break;
            case 3:
                slotType = "AntiAir";
                break;
            case 4:
                slotType = "Auxiliary";
                break;
        }

        slotNameText.text = slotType;
    }
}
