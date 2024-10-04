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
    public Text[] equipmentStatsTexts; // ����� ������ ǥ���� �ؽ�Ʈ �迭 (�ʿ��� ��ŭ �ν����Ϳ��� ����)

    private List<Character> ownedCharacters; // ������ ĳ���� ����Ʈ
    private List<string> equippedGears; // ���õ� ĳ���Ͱ� ������ ��� ����Ʈ

    void Start()
    {
        // �÷��̾��� ���� ĳ���͸� ������
        ownedCharacters = Player.Instance.ownedCharacter;

        // ù ��° ĳ������ ������ ��� ������ (���÷� ù ��° ĳ���͸� ���)
        equippedGears = ownedCharacters[0].eqiuppedGears;

        // ��� ��ư ����
        CreateEquipmentButtons();
    }

    // ��� ���� ��ư ����
    void CreateEquipmentButtons()
    {
        foreach (string gearName in equippedGears)
        {
            GameObject equipmentButton = Instantiate(equipmentButtonPrefab, equipmentGridParent);

            // ��ư�� �ؽ�Ʈ�� ��� �̸����� ����
            equipmentButton.GetComponentInChildren<Text>().text = gearName;

            // ��ư Ŭ�� �̺�Ʈ �߰�
            equipmentButton.GetComponent<Button>().onClick.AddListener(() => OnEquipmentButtonClicked(gearName));
        }
    }

    // ��� ��ư Ŭ�� �� ȣ��Ǵ� �޼���
    void OnEquipmentButtonClicked(string gearName)
    {
        // ��� ������ �ε�
        Gear selectedGear = GearDataLoader.GetGearByName(gearName);

        if (selectedGear != null)
        {
            // ��� �� ���� �г��� Ȱ��ȭ
            equipmentDetailPanel.SetActive(true);

            // ��� �̹��� ����
            equipmentImage.sprite = Resources.Load<Sprite>($"Images_Gear/{selectedGear.imageName}");

            // ��� ���� ����
            SetEquipmentStats(selectedGear);
        }
    }

    // ��� ������ UI�� �����ϴ� �޼���
    void SetEquipmentStats(Gear gear)
    {
        // ������ �ؽ�Ʈ�� �ݿ�
        equipmentStatsTexts[0].text = $"HP: {gear.stats.HP}";
        equipmentStatsTexts[1].text = $"FP: {gear.stats.FP}";
        equipmentStatsTexts[2].text = $"TRP: {gear.stats.TRP}";
        equipmentStatsTexts[3].text = $"AVI: {gear.stats.AVI}";
        equipmentStatsTexts[4].text = $"AA: {gear.stats.AA}";
        equipmentStatsTexts[5].text = $"SPD: {gear.stats.SPD}";
        equipmentStatsTexts[6].text = $"DMG: {gear.stats.DMG}";
        equipmentStatsTexts[7].text = $"RLD: {gear.stats.RLD}";
    }
}
