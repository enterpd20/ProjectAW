using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DockDetailUI_EquipSlot : MonoBehaviour
{
    [SerializeField] private Text[] equipTypeText;      // UI_EquipType의 Text_EquipType 배열로 할당
    [SerializeField] private Text[] equipNameText;      // Text_EquipName의 텍스트를 배열로 할당
    [SerializeField] private Image[] equipImage;        // UI_EquipEmpty의 이미지를 배열로 할당

    [SerializeField] private Text character_ShipType;   // 캐릭터 함종 받아오기     
    [SerializeField] private GameObject[] equipSlot;    // UI_EquipSlot 1~4 배열로 할당  

    private DockDetailUI_Character dockDetailUICharacter;

    // JSON 데이터를 통해 로드된 장비 정보 리스트
    private List<Gear> loadedGears;

    private Character currentCharacter;
    private List<Gear> equippedGears;   // 장비의 데이터를 담고 있는 리스트

    // UI 표현을 위함
    private RectTransform Rect_GearUI;

    private void Awake()
    {
        Rect_GearUI = GetComponent<RectTransform>();

        dockDetailUICharacter = FindObjectOfType<DockDetailUI_Character>();

        currentCharacter = Player.Instance.GetSelectedCharacter_DockUI();  // 현재 선택된 캐릭터
        equippedGears = new List<Gear>();             // equippedGears 리스트 초기화, 현재 선택된 캐릭터가 장착중인 장비       

        foreach (var gearName in currentCharacter.eqiuppedGears)
        {
            Gear matchingGear = GearDataLoader.GetGearByName(gearName);
            if (matchingGear != null)
            {
                equippedGears.Add(matchingGear);    // 변환된 Gear 객체를 equippedGears에 추가
            }
            else
            {
                Debug.LogWarning($"No matching gear found for name {gearName}");
            }
        }

        SetEquipSlots();
    }

    public void SetEquipSlots()     // 각 장비 슬롯에 장착 가능한 장비 유형 설정
    {
        string shipType = Player.Instance.GetSelectedCharacter_DockUI().shipType; // 캐릭터 함종 파악
        Debug.Log($"Character ship type: {shipType}");

        for (int i = 0; i < equipSlot.Length; i++)
        {
            if (equipTypeText[i] != null)
            {
                equipTypeText[i].text = GetGearType(shipType, i);
                Debug.Log($"Slot {i}: Set equip type text to {equipTypeText[i].text}");
            }

            // 캐릭터가 장착한 장비를 기반으로 장비 이름과 이미지를 설정
            if (i < currentCharacter.eqiuppedGears.Count)
            {
                string equippedGearName = currentCharacter.eqiuppedGears[i];  // 슬롯 i에 장착된 장비 이름 가져오기
                Gear matchingGear = GearDataLoader.GetGearByName(equippedGearName); // 장비 이름으로 Gear 객체 찾기

                if (matchingGear != null)
                {
                    // UI 업데이트: 장비 이름과 이미지 설정
                    if (equipNameText[i] != null)
                    {
                        equipNameText[i].text = matchingGear.name;   // 장착한 장비의 이름을 UI에 표시
                    }
                    if (equipImage[i] != null)
                    {
                        equipImage[i].sprite = GetGearSprite(matchingGear);  // 장착한 장비의 이미지를 UI에 표시
                        equipImage[i].enabled = true;
                    }
                }
            }
            else
            {
                // 장비가 없는 경우 빈 슬롯으로 처리
                if (equipNameText[i] != null)
                {
                    equipNameText[i].text = "Empty";
                }
                if (equipImage[i] != null)
                {
                    equipImage[i].enabled = false;  // 장비가 없을 경우 빈 이미지로 설정
                }
            }
        }
    }

    private Sprite GetGearSprite(Gear gear)
    {
        return Resources.Load<Sprite>($"Images_Gear/{gear.imageName}");
    }

    private string GetGearType(string shipType, int index)
    {
        Debug.Log($"GetGearType called with shipType: {shipType}, index: {index}");

        switch (index)
        {
            case 0: return (shipType == "CV") ? "Torpedo Bomber" : "Main Gun";
            case 1: return (shipType == "CV") ? "Dive Bomber" : "Secondary Weapon";
            case 2: return "AntiAir";
            case 3: return "Auxiliary";
            default: return "";
        }
    }

    public void DisplayGearInfo(int slotIndex, Gear gear)
    {
        // 스탯 정보 표시
        if(slotIndex < equipSlot.Length)
        {
            
            DockDetailUI_EquipStat equipStat = equipSlot[slotIndex].GetComponent<DockDetailUI_EquipStat>();
            if(equipStat != null)
            {
                equipStat.SetGearInfo(gear);    // 해당 슬롯에 장착된 장비 정보를 UI에 표시
            }
        }
    }

    // 장비 데이터를 기반으로 적절한 장비를 장착
    public void AssignGearToSlot(int slotIndex, string gearType)
    {
        Gear matchingGear = GearDataLoader.GetGearByName(gearType); // gearType에 맞는 장비 찾기
        if(matchingGear != null)
        {
            DisplayGearInfo(slotIndex, matchingGear);   // 장비 슬롯에 장착
            //Debug.Log($"Assigned gear {matchingGear.name} to slot {slotIndex}");
        }
        else
        {
            Debug.LogWarning($"No matching gear found for type {gearType} in slot {slotIndex}");
        }
    }

    public void Appear_GearUI()
    {
        Rect_GearUI.localScale = Vector3.one;
    }

    public void Disappear_GearUI()
    {
        Rect_GearUI.localScale = Vector3.zero;
    }
}
