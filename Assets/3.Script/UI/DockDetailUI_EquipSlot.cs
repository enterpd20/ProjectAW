using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DockDetailUI_EquipSlot : MonoBehaviour
{
    [SerializeField] private Text character_ShipType;   // ĳ���� ���� �޾ƿ���
    [SerializeField] private GameObject[] equipSlots;   // UI_EquipSlot 1~4 �迭�� �Ҵ�
    private DockDetailUI_Character characterUI;

    // JSON �����͸� ���� �ε�� ��� ���� ����Ʈ
    private List<Gear> loadedGears;

    private Character currentCharacter;
    private List<Gear> equippedGears;

    // UI ǥ���� ����
    private RectTransform Rect_GearUI;

    private void Awake()
    {
        Rect_GearUI = GetComponent<RectTransform>();
    }

    private void Start()
    {
        LoadGearData();
        SetEquipSlots();

        currentCharacter = Player.Instance.GetSelectedCharacter();
        equippedGears = currentCharacter.eqiuppedGears;

        SetCharaAndEquipInfo(currentCharacter, equippedGears);
    }

    private void LoadGearData()
    {
        // GearDataLoader ��ũ��Ʈ���� JSON ������ �ҷ��� List<Gear>�� ����
        loadedGears = GearDataLoader.LoadAllGears();
        if (loadedGears == null || loadedGears.Count == 0)
        {
            Debug.LogError("Failed to load gear data.");
        }
    }

    public void SetCharaAndEquipInfo(Character character, List<Gear> gears)
    {
        currentCharacter = character;
        equippedGears = gears;
        SetEquipSlots();

        for(int i = 0; i < equippedGears.Count; i++)
        {
            DisplayGearInfo(i, equippedGears[i]);
        }
    }

    public void SetEquipSlots()     // �� ��� ���Կ� ���� ������ ��� ���� ����
    {
        string shipType = character_ShipType.text; // ĳ���� ���� �ľ�

        for(int i = 0; i < equipSlots.Length; i++)
        {
            //Text equipTypeText = equipSlots[i].transform.Find("Text_EquipType").GetComponent<Text>();
            Text equipTypeText = equipSlots[i].GetComponentInChildren<Text>();
            if(equipTypeText != null)
            { 
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
            else
            {
                Debug.LogWarning($"Cannot find 'Text_EquipType' in equipSlot[{i}]");
            }
        }
    }

    public void DisplayGearInfo(int slotIndex, Gear gear)
    {
        // ��� �̸��� UI�� ǥ��
        Text equipNameText = equipSlots[slotIndex].transform.Find("UI_EquipName").GetComponent<Text>();
        if (equipNameText != null)
        { 
            equipNameText.text = gear.name;
        }
        else
        {
            Debug.LogWarning($"Cannot find 'UI_EquipName' in equipSlot[{slotIndex}]");
        }

        // ��� �̹����� UI�� ǥ��
        Image equipImage  = equipSlots[slotIndex].transform.Find("UI_EquipEmpty").GetComponent<Image>();
        Sprite gearSprite = Resources.Load<Sprite>($"Images_Gear/{gear.imageName}");
        if(gearSprite != null)
        {
            equipImage.sprite = gearSprite;
        }
        else
        {
            Debug.LogWarning($"Failed to load sprite for gear: {gear.imageName}");
        }


        // ���� ������ �� ���� �����ϴ� ����Ʈ
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

        // ������ ���� ǥ��
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

    // ��� �����͸� ������� ������ ��� ����
    public void AssignGearToSlot(int slotIndex, string gearType)
    {
        Gear matchingGear = loadedGears.Find(g => g.gearType == gearType); // gearType�� �´� ��� ã��
        if(matchingGear != null)
        {
            DisplayGearInfo(slotIndex, matchingGear);   // ��� ���Կ� ����
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
