using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DepotUI : MonoBehaviour
{
    public GameObject equipmentButtonPrefab; // ��� ���� ��ư ������
    public Transform equipmentGridParent; // Grid Layout Group�� �θ� ������Ʈ
    public GameObject equipmentDetailPanel; // ��� �� ������ ǥ���ϴ� �г�
    public Image equipmentImage; // ��� �̹����� ǥ���� UI �̹���
    public Text equipmentNameText; // ��� �̸��� ǥ���� �ؽ�Ʈ UI

    private List<string> equippedGears; // ���õ� ĳ���Ͱ� ������ ��� ����Ʈ

    void Start()
    {
        // �÷��̾��� ���� ĳ���� ����Ʈ�� ������
        List<Character> ownedCharacters = Player.Instance.ownedCharacter;
        equippedGears = new List<string>();

        // ��� ĳ������ ���� ��� ����Ʈ�� �߰�
        foreach (Character character in ownedCharacters)
        {
            equippedGears.AddRange(character.eqiuppedGears);
        }

        // ��� ��ư ����
        CreateEquipmentButtons();
    }

    // ��� ���� ��ư ����
    void CreateEquipmentButtons()
    {
        foreach (string gearName in equippedGears)
        {
            GameObject equipmentButton = Instantiate(equipmentButtonPrefab, equipmentGridParent);

            // ��� ������ �ε�
            Gear gear = GearDataLoader.GetGearByName(gearName);

            if (gear != null)
            {
                // ������ ���� �̹��� ������Ʈ�� ã�Ƽ� �̹��� ����
                Image gearImage = equipmentButton.transform.GetComponent<Image>();
                if (gearImage != null)
                {
                    gearImage.sprite = Resources.Load<Sprite>($"Images_Gear/{gear.imageName}");
                }
            }
        }
    }

    // ��� ��ư Ŭ�� �� ȣ��Ǵ� �޼���
    public void OnEquipmentButtonClicked(string gearName)
    {
        // ��� ������ �ε�
        Gear selectedGear = GearDataLoader.GetGearByName(gearName);

        if (selectedGear != null)
        {
            // ��� �� ���� �г��� Ȱ��ȭ
            equipmentDetailPanel.transform.localScale = new Vector3(2, 2, 2);
            //equipmentDetailPanel.SetActive(true);            

            // ��� �̹��� ����
            equipmentImage.sprite = Resources.Load<Sprite>($"Images_Gear/{selectedGear.imageName}");

            // ��� �̸� ����
            equipmentNameText.text = selectedGear.name;

            // ��� ���� ����
            //SetEquipmentStats(selectedGear);
        }
    }

    public void CloseEquipmentButtonPanel()
    {
        equipmentDetailPanel.transform.localScale = new Vector3(0, 0, 0);
    }
}
