using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DockDetailUI_EquipSlot : MonoBehaviour
{
    [SerializeField] private Text equipTypeText;  // ��� Ÿ��
    [SerializeField] private Text equipNameText;  // ��� �̸�
    [SerializeField] private Image equipImage;    // ��� �̹���
    [SerializeField] private GameObject[] statBGs;
    [SerializeField] private Text slotNameText;    // ���� �̸�

    public void SetEquipmentData(Gear gear)
    {
        // ��� Ÿ�԰� �̸� ����
        equipTypeText.text = gear.gearType;
        equipNameText.text = gear.name;

        // ��� �̹��� �ҷ�����(Resources/Images_Gear ����)
        Sprite gearSprite = Resources.Load<Sprite>("Images_Gear/" + gear.imageName);
        equipImage.sprite = gearSprite;
        
        // ������ �������� ����(�ִ� 4������)
        UpdateStats(gear.stats);
    }

    private void UpdateStats(GearStats stats)
    {
        // �ʱ�ȭ
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
        
        // ���� statBG�� ������ ��ĭ���� ����
        for(; statIndex < statBGs.Length; statIndex++)
        {
            SetStatText(statBGs[statIndex], "", 0);
        }
    }

    // ���� ������ UI�� �Ҵ��ϴ� �޼���
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
            // ������ ���� ���, �ؽ�Ʈ�� ����� ��ĭ���� ���̰Բ�
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
