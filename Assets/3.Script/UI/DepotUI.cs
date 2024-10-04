using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DepotUI : MonoBehaviour
{
    public GameObject equipmentButtonPrefab; // 장비 선택 버튼 프리팹
    public Transform equipmentGridParent; // Grid Layout Group의 부모 오브젝트
    public GameObject equipmentDetailPanel; // 장비 상세 정보를 표시하는 패널
    public Image equipmentImage; // 장비 이미지를 표시할 UI 이미지
    public Text[] equipmentStatsTexts; // 장비의 스탯을 표시할 텍스트 배열 (필요한 만큼 인스펙터에서 설정)

    private List<Character> ownedCharacters; // 소유한 캐릭터 리스트
    private List<string> equippedGears; // 선택된 캐릭터가 장착한 장비 리스트

    void Start()
    {
        // 플레이어의 소유 캐릭터를 가져옴
        ownedCharacters = Player.Instance.ownedCharacter;

        // 첫 번째 캐릭터의 장착된 장비를 가져옴 (예시로 첫 번째 캐릭터를 사용)
        equippedGears = ownedCharacters[0].eqiuppedGears;

        // 장비 버튼 생성
        CreateEquipmentButtons();
    }

    // 장비 선택 버튼 생성
    void CreateEquipmentButtons()
    {
        foreach (string gearName in equippedGears)
        {
            GameObject equipmentButton = Instantiate(equipmentButtonPrefab, equipmentGridParent);

            // 버튼의 텍스트를 장비 이름으로 설정
            equipmentButton.GetComponentInChildren<Text>().text = gearName;

            // 버튼 클릭 이벤트 추가
            equipmentButton.GetComponent<Button>().onClick.AddListener(() => OnEquipmentButtonClicked(gearName));
        }
    }

    // 장비 버튼 클릭 시 호출되는 메서드
    void OnEquipmentButtonClicked(string gearName)
    {
        // 장비 데이터 로드
        Gear selectedGear = GearDataLoader.GetGearByName(gearName);

        if (selectedGear != null)
        {
            // 장비 상세 정보 패널을 활성화
            equipmentDetailPanel.SetActive(true);

            // 장비 이미지 설정
            equipmentImage.sprite = Resources.Load<Sprite>($"Images_Gear/{selectedGear.imageName}");

            // 장비 스탯 설정
            SetEquipmentStats(selectedGear);
        }
    }

    // 장비 스탯을 UI에 설정하는 메서드
    void SetEquipmentStats(Gear gear)
    {
        // 스탯을 텍스트에 반영
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
