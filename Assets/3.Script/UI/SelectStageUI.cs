using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SelectStageUI : MonoBehaviour
{
    public GameObject characterFormationUI;     // UI 표시를 위한 변수
    public GameObject characterSelectUI;        // UI 표시를 위한 변수
    public GameObject confirmButton;            // UI 표시를 위한 변수
    public GameObject warningMessage;           // UI 표시를 위한 변수
    public Sprite emptySlotSprite;              // UI 표시를 위한 변수

    public Image[] characterSelectButton;       // 배치할 캐릭터를 선택할 수 있는 버튼들

    private int currentSelectedButtonIndex = -1;    // 캐릭터가 버튼에 배치되었는지 여부를 나타내는 변수. -1 = 배치되지 않음
    public Transform characterListContainer;    // 캐릭터 목록 UI의 부모 객체
    public GameObject characterListItemPrefab;  // 캐릭터 목록 아이템 프리팹


    private void Start()
    {
        if (Player.Instance.selectedCharacterIndices == null)
        {
            Debug.LogError("selectedCharacterIndices is not initialized properly.");
            Player.Instance.InitializePlayerData();  // 배열이 null이면 초기화
        }

        //Player.Instance.InitializeIndices(characterSelectButton.Length);
        Debug.Log("selectedCharacterIndices Length: " + Player.Instance.selectedCharacterIndices.Length);

        UpdateAllCharacterSelections();
        characterFormationUI.SetActive(true);   // 캐릭터 편성 UI를 활성화하여 보이게 설정
    }

    // 선택된 버튼의 인덱스를 사용해 해당 버튼에 캐릭터 이미지를 업데이트
    private void UpdateCharacterSelection(int buttonIndex)
    {
        //if (Player.Instance.selectedCharacterIndices == null)
        //{
        //    Debug.LogError("selectedCharacterIndices is not initialized properly.");
        //    Debug.LogError("selectedCharacterIndices == null");
        //    return;
        //}
        //if(Player.Instance.selectedCharacterIndices.Length != 6)
        //{
        //    Debug.LogError("selectedCharacterIndices.Length != 6");
        //    return;
        //}

        int characterIndex = Player.Instance.selectedCharacterIndices[buttonIndex];
        //Debug.Log($"Button {buttonIndex} - characterIndex: {characterIndex}");

        // 슬롯을 초기화하여 스프라이트를 제거
        characterSelectButton[buttonIndex].sprite = emptySlotSprite;


        if (characterIndex != -1)
        {
            Character selectedCharacter = Player.Instance.GetSelectedCharacter_SelectStage(characterIndex);
            if (selectedCharacter != null)
            {
                characterSelectButton[buttonIndex].sprite =
                    Resources.Load<Sprite>($"Images_Character/{selectedCharacter.imageName}");
            }
            else
            {
                Debug.LogError($"No character found for index {characterIndex}");
            } 
        }
        //else
        //{
        //    Debug.LogWarning($"No character assigned to button {buttonIndex}");
        //}
    }

    private void UpdateAllCharacterSelections()
    {
        for (int i = 0; i < characterSelectButton.Length; i++)
        {
            UpdateCharacterSelection(i);
        }
    }

    public void SetSelectedCharacter(Character character)
    {
        int characterIndex = Player.Instance.ownedCharacter.IndexOf(character);
        Debug.Log($"Character index for {character.name}: {characterIndex}");

        if (characterIndex != -1 && currentSelectedButtonIndex != -1)
        {
            //// 현재 버튼에 올바른 함종만 배치할 수 있도록 검사
            //if (IsValidShipTypeForButton(character.shipType, currentSelectedButtonIndex))
            //{
            //    Debug.Log($"Assigning {character.name} to button {currentSelectedButtonIndex}");
            //    Player.Instance.selectedCharacterIndices[currentSelectedButtonIndex] = characterIndex;
            //    Player.Instance.SavePlayerData();
            //    UpdateCharacterSelection(currentSelectedButtonIndex);
            //    Close_CharacterSelectUI();
            //}
            //else
            //{
            //    Debug.LogError($"Cannot assign {character.shipType} to button {currentSelectedButtonIndex}");
            //}

            // 현재 버튼에 올바른 함종만 배치할 수 있도록 검사
            if (IsValidShipTypeForButton(character.shipType, currentSelectedButtonIndex))
            {


                // 이미 편성된 캐릭터인지 확인하고 중복 방지
                for (int i = 0; i < Player.Instance.selectedCharacterIndices.Length; i++)
                {
                    if (Player.Instance.selectedCharacterIndices[i] == characterIndex)
                    {
                        // 이미 편성된 캐릭터를 현재 선택한 칸으로 이동
                        Debug.Log($"Moving character {character.name} from slot {i} to slot {currentSelectedButtonIndex}");
                        Player.Instance.selectedCharacterIndices[i] = -1; // 기존 칸 비우기
                        break;
                    }
                }

                // 캐릭터를 선택된 칸에 배치
                Debug.Log($"Assigning {character.name} to button {currentSelectedButtonIndex}");
                Player.Instance.selectedCharacterIndices[currentSelectedButtonIndex] = characterIndex;
                Player.Instance.SavePlayerData();
                UpdateAllCharacterSelections(); // 전체 업데이트로 UI 반영
                Close_CharacterSelectUI();
            }
            else
            {
                Debug.LogError($"Cannot assign {character.shipType} to button {currentSelectedButtonIndex}");
            }
        }
        else
        {
            Debug.LogError($"Character selection or button selection failed.");
        }
    }

    // 특정 버튼을 선택하면, 그 버튼의 인덱스를 저장하고 캐릭터 선택 UI를 활성화
    public void SelectButton(int buttonIndex)   // 버튼 오브젝트의 인스펙터에서 OnClick에 할당할 메서드
    {
        currentSelectedButtonIndex = buttonIndex;
        Debug.Log($"Selected button index: {currentSelectedButtonIndex}");
        Open_CharacterSelectUI();

        // 캐릭터 목록을 필터링하여 UI 업데이트
        FilterCharacterListByButton(buttonIndex);
    }

    // 버튼 인덱스에 따라 배치 가능한 함종을 검사하는 메서드
    private bool IsValidShipTypeForButton(string shipType, int buttonIndex)
    {
        if (buttonIndex < 3) // 첫 3칸은 BB, CV만
        {
            return shipType == "BB" || shipType == "CV";
        }
        else // 나머지 3칸은 DD, CLCA만
        {
            return shipType == "DD" || shipType == "CLCA";
        }
    }

    // 버튼 인덱스에 따라 캐릭터 목록을 필터링하는 메서드
    private void FilterCharacterListByButton(int buttonIndex)
    {
        // 기존 목록 삭제
        foreach (Transform child in characterListContainer)
        {
            Destroy(child.gameObject);
        }

        // 필터링된 캐릭터 추가
        foreach (Character character in Player.Instance.ownedCharacter)
        {
            if (IsValidShipTypeForButton(character.shipType, buttonIndex))
            {
                GameObject listItem = Instantiate(characterListItemPrefab, characterListContainer);

                // 희귀도 배경 설정 (자식 오브젝트의 첫 번째 인덱스)
                Image rarityBackground = listItem.transform.GetChild(0).GetComponent<Image>();
                rarityBackground.sprite = GetRarityBackground(character.rarity);

                // 캐릭터 스프라이트 설정 (자식 오브젝트의 두 번째 인덱스)
                Image characterImage = listItem.transform.GetChild(1).GetComponent<Image>();
                characterImage.sprite = Resources.Load<Sprite>($"Images_Character/{character.imageName}");

                listItem.GetComponentInChildren<Text>().text = character.name; // 캐릭터 이름 표시

                listItem.GetComponent<Button>().onClick.AddListener(() => SetSelectedCharacter(character));
            }
        }
    }

    public void Open_CharacterFormationUI()
    {
        characterFormationUI.transform.localScale = Vector3.one;
    }

    public void Close_CharacterFormationUI()
    {
        characterFormationUI.transform.localScale = Vector3.zero;
    }

    public void Open_CharacterSelectUI()
    {
        characterSelectUI.transform.localScale = Vector3.one;
    }

    public void Close_CharacterSelectUI()
    {
        characterSelectUI.transform.localScale = Vector3.zero;
    }

    public void Change_ToBattleScene()
    {
        if (IsAnyCharacterAssigned())
        {
            SceneManager.LoadScene("04_Battle");
        }
        else
        {
            StartCoroutine(ShowWarningMessage());
        }
    }

    private bool IsAnyCharacterAssigned()
    {
        foreach (int characterIndex in Player.Instance.selectedCharacterIndices)
        {
            if (characterIndex != -1)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator ShowWarningMessage()
    {
        warningMessage.transform.localScale = Vector3.one;
        yield return new WaitForSeconds(1.5f);
        warningMessage.transform.localScale = Vector3.zero;
    }

    // 희귀도에 따른 배경을 가져오는 메서드
    private Sprite GetRarityBackground(string rarity)
    {
        switch (rarity)
        {
            case "SSR": return Resources.Load<Sprite>("RarityBG/BG_SSR");
            case "SR": return Resources.Load<Sprite>("RarityBG/BG_SR");
            case "R": return Resources.Load<Sprite>("RarityBG/BG_R");
            case "N": return Resources.Load<Sprite>("RarityBG/BG_N");
            default: return Resources.Load<Sprite>("RarityBG/DefaultBackground");
        }
    }
}
