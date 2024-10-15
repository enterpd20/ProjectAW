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
    public Text equipmentNameText; // 장비 이름을 표시할 텍스트 UI

    private List<string> equippedGears; // 선택된 캐릭터가 장착한 장비 리스트

    void Start()
    {
        // 플레이어의 소유 캐릭터 리스트를 가져옴
        List<Character> ownedCharacters = Player.Instance.ownedCharacter;
        equippedGears = new List<string>();

        // 모든 캐릭터의 장착 장비를 리스트에 추가
        foreach (Character character in ownedCharacters)
        {
            equippedGears.AddRange(character.eqiuppedGears);
        }

        // 장비 버튼 생성
        CreateEquipmentButtons();
    }

    // 장비 선택 버튼 생성
    void CreateEquipmentButtons()
    {
        foreach (string gearName in equippedGears)
        {
            GameObject equipmentButton = Instantiate(equipmentButtonPrefab, equipmentGridParent);

            // 장비 데이터 로드
            Gear gear = GearDataLoader.GetGearByName(gearName);

            if (gear != null)
            {
                // 프리팹 내의 이미지 컴포넌트를 찾아서 이미지 설정
                Image gearImage = equipmentButton.transform.GetComponent<Image>();
                if (gearImage != null)
                {
                    gearImage.sprite = Resources.Load<Sprite>($"Images_Gear/{gear.imageName}");
                }
            }
        }
    }

    // 장비 버튼 클릭 시 호출되는 메서드
    public void OnEquipmentButtonClicked(string gearName)
    {
        // 장비 데이터 로드
        Gear selectedGear = GearDataLoader.GetGearByName(gearName);

        if (selectedGear != null)
        {
            // 장비 상세 정보 패널을 활성화
            equipmentDetailPanel.transform.localScale = new Vector3(2, 2, 2);
            //equipmentDetailPanel.SetActive(true);            

            // 장비 이미지 설정
            equipmentImage.sprite = Resources.Load<Sprite>($"Images_Gear/{selectedGear.imageName}");

            // 장비 이름 설정
            equipmentNameText.text = selectedGear.name;

            // 장비 스탯 설정
            //SetEquipmentStats(selectedGear);
        }
    }

    public void CloseEquipmentButtonPanel()
    {
        equipmentDetailPanel.transform.localScale = new Vector3(0, 0, 0);
    }
}
