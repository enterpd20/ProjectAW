using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DockDetailUI_EquipSlot : MonoBehaviour
{
    [SerializeField] private Text[] equipTypeText;      // UI_EquipType�� Text_EquipType �迭�� �Ҵ�
    [SerializeField] private Text[] equipNameText;      // Text_EquipName�� �ؽ�Ʈ�� �迭�� �Ҵ�
    [SerializeField] private Image[] equipImage;        // UI_EquipEmpty�� �̹����� �迭�� �Ҵ�

    [SerializeField] private Text character_ShipType;   // ĳ���� ���� �޾ƿ���     
    [SerializeField] private GameObject[] equipSlot;    // UI_EquipSlot 1~4 �迭�� �Ҵ�  

    private DockDetailUI_Character dockDetailUICharacter;

    // JSON �����͸� ���� �ε�� ��� ���� ����Ʈ
    private List<Gear> loadedGears;

    private Character currentCharacter;
    private List<Gear> equippedGears;   // ����� �����͸� ��� �ִ� ����Ʈ

    // UI ǥ���� ����
    private RectTransform Rect_GearUI;

    private void Awake()
    {
        Rect_GearUI = GetComponent<RectTransform>();

        dockDetailUICharacter = FindObjectOfType<DockDetailUI_Character>();

        currentCharacter = Player.Instance.GetSelectedCharacter_DockUI();  // ���� ���õ� ĳ����
        equippedGears = new List<Gear>();             // equippedGears ����Ʈ �ʱ�ȭ, ���� ���õ� ĳ���Ͱ� �������� ���       

        foreach (var gearName in currentCharacter.eqiuppedGears)
        {
            Gear matchingGear = GearDataLoader.GetGearByName(gearName);
            if (matchingGear != null)
            {
                equippedGears.Add(matchingGear);    // ��ȯ�� Gear ��ü�� equippedGears�� �߰�
            }
            else
            {
                Debug.LogWarning($"No matching gear found for name {gearName}");
            }
        }

        SetEquipSlots();
    }

    public void SetEquipSlots()     // �� ��� ���Կ� ���� ������ ��� ���� ����
    {
        string shipType = Player.Instance.GetSelectedCharacter_DockUI().shipType; // ĳ���� ���� �ľ�
        Debug.Log($"Character ship type: {shipType}");

        for (int i = 0; i < equipSlot.Length; i++)
        {
            if (equipTypeText[i] != null)
            {
                equipTypeText[i].text = GetGearType(shipType, i);
                Debug.Log($"Slot {i}: Set equip type text to {equipTypeText[i].text}");
            }

            // ĳ���Ͱ� ������ ��� ������� ��� �̸��� �̹����� ����
            if (i < currentCharacter.eqiuppedGears.Count)
            {
                string equippedGearName = currentCharacter.eqiuppedGears[i];  // ���� i�� ������ ��� �̸� ��������
                Gear matchingGear = GearDataLoader.GetGearByName(equippedGearName); // ��� �̸����� Gear ��ü ã��

                if (matchingGear != null)
                {
                    // UI ������Ʈ: ��� �̸��� �̹��� ����
                    if (equipNameText[i] != null)
                    {
                        equipNameText[i].text = matchingGear.name;   // ������ ����� �̸��� UI�� ǥ��
                    }
                    if (equipImage[i] != null)
                    {
                        equipImage[i].sprite = GetGearSprite(matchingGear);  // ������ ����� �̹����� UI�� ǥ��
                        equipImage[i].enabled = true;
                    }
                }
            }
            else
            {
                // ��� ���� ��� �� �������� ó��
                if (equipNameText[i] != null)
                {
                    equipNameText[i].text = "Empty";
                }
                if (equipImage[i] != null)
                {
                    equipImage[i].enabled = false;  // ��� ���� ��� �� �̹����� ����
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
        // ���� ���� ǥ��
        if(slotIndex < equipSlot.Length)
        {
            
            DockDetailUI_EquipStat equipStat = equipSlot[slotIndex].GetComponent<DockDetailUI_EquipStat>();
            if(equipStat != null)
            {
                equipStat.SetGearInfo(gear);    // �ش� ���Կ� ������ ��� ������ UI�� ǥ��
            }
        }
    }

    // ��� �����͸� ������� ������ ��� ����
    public void AssignGearToSlot(int slotIndex, string gearType)
    {
        Gear matchingGear = GearDataLoader.GetGearByName(gearType); // gearType�� �´� ��� ã��
        if(matchingGear != null)
        {
            DisplayGearInfo(slotIndex, matchingGear);   // ��� ���Կ� ����
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
