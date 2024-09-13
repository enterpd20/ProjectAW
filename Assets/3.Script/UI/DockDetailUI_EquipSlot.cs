using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DockDetailUI_EquipSlot : MonoBehaviour
{
    [SerializeField] private Text character_ShipType;   // 캐릭터 함종 받아오기
    [SerializeField] private GameObject[] equipSlots;   // UI_EquipSlot 1~4 배열로 할당

    // JSON 데이터를 통해 로드된 장비 정보 리스트
    private List<Gear> loadedGears;

    private void Start()
    {
        LoadGearData();
        SetEquipSlots();
    }

    private void LoadGearData()
    {
        // GearDataLoader 스크립트에서 JSON 파일을 불러와 List<Gear>에 저장
        loadedGears = GearDataLoader.LoadAllGears();    
    }

    public void SetEquipSlots()     // 각 장비 슬롯에 장착 가능한 장비 유형 설정
    {
        string shipType = character_ShipType.text; // 캐릭터 함종 파악
        
        // 각 슬롯에 장착 가능한 장비 유형을 설정
        for(int i = 0; i < equipSlots.Length; i++)
        {
            Text equipTypeText = equipSlots[i].transform.Find("Text_EquipType").GetComponent<Text>();

            switch(i)
            {
                case 0:
                    if (shipType == "BB")
                        equipTypeText.text = "Main Gun";
                    else if(shipType == "CLCA")
                        equipTypeText.text = "Main Gun";
                    else if (shipType == "DD")
                        equipTypeText.text = "Main Gun";
                    else if (shipType == "CV")
                        equipTypeText.text = "Torpedo Bomber";
                    break;
                case 1:
                    if (shipType == "BB")
                        equipTypeText.text = "Secondary Gun";
                    else if (shipType == "CLCA")
                        equipTypeText.text = "Secondary Weapon";
                    else if (shipType == "DD")
                        equipTypeText.text = "Torpedo";
                    else if (shipType == "CV")
                        equipTypeText.text = "Dive Bomber";
                    break;
                case 2:
                    equipTypeText.text = "AntiAir";
                    break;
                case 3:
                    equipTypeText.text = "Auxiliary";
                    break;
            }
        }
    }

    public void DisplayGearInfo(int slotIndex, Gear gear)
    {
        // 장비 이름을 UI에 표시
        Text equipNameText = equipSlots[slotIndex].transform.Find("UI_EquipName").GetComponent<Text>();
        equipNameText.text = gear.name;

        // 장비 이미지를 UI에 표시
        Image equipImage  = equipSlots[slotIndex].transform.Find("UI_EquipEmpty").GetComponent<Image>();
        Sprite gearSprite = Resources.Load<Sprite>($"Images_Gear/{gear.imageName}");

        // 스탯 종류와 그 값을 저장하는 리스트
        List <(string, float)> stats = new List<(string, float)>
        {
            ("HP", gear.stats.HP),
            ("AVI", gear.stats.AVI),
            ("SPD", gear.stats.SPD),
            ("AA", gear.stats.AA),
            ("FP", gear.stats.FP),
            ("TRP", gear.stats.TRP),
            ("DMG", gear.stats.DMG),
            ("RLD", gear.stats.RLD),
        };

        // 스탯의 정보 표시
        for(int i = 0; i < equipSlots[slotIndex].transform.Find("UI_EquipStat").childCount; i++)
        {
            Transform statBG = equipSlots[slotIndex].transform.Find("UI_EquipStat").Find("UI_StatBG " + i);

            if(i < stats.Count)
            {
                Text statTypeText = statBG.Find("Text_StatType").GetComponent<Text>();
                Text statValueText = statBG.Find("Text_StatValue").GetComponent<Text>();

                statTypeText.text = stats[i].Item1;
                statValueText.text = stats[i].Item2.ToString();
            }
            else
            {
                statBG.Find("Text_StatType").GetComponent<Text>().text = "";
                statBG.Find("Text_StatValue").GetComponent<Text>().text = "";
            }
        }
    }

    // 장비 데이터를 기반으로 적절한 장비를 장착
    public void AssignGearToSlot(int slotIndex, string gearType)
    {
        Gear matchingGear = loadedGears.Find(g => g.gearType == gearType); // gearType에 맞는 장비 찾기
        if(matchingGear != null)
        {
            DisplayGearInfo(slotIndex, matchingGear);   // 장비 슬롯에 장착
        }
    }
}
