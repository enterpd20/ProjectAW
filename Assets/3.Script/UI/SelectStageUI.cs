using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SelectStageUI : MonoBehaviour
{
    public GameObject characterFormationUI;     // UI 표시를 위한 변수
    public GameObject characterSelectUI;        // UI 표시를 위한 변수
    public GameObject confirmButton;
    public GameObject warningMessage;

    public Image[] characterSelectButton;       // 배치할 캐릭터를 선택할 수 있는 버튼들

    private int currentSelectedButtonIndex = -1;    // 캐릭터가 버튼에 배치되었는지 여부를 나타내는 변수. -1 = 배치되지 않음

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
        if (characterIndex != -1)
        {
            Character selectedCharacter = Player.Instance.GetSelectedCharacter_SelectStage(characterIndex);
            if(selectedCharacter != null)
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
        for(int i = 0; i < characterSelectButton.Length; i++)
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
            Debug.Log($"Assigning {character.name} to button {currentSelectedButtonIndex}");
            Player.Instance.selectedCharacterIndices[currentSelectedButtonIndex] = characterIndex;
            Player.Instance.SavePlayerData();
            UpdateCharacterSelection(currentSelectedButtonIndex);
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
        if(IsAnyCharacterAssigned())
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
        foreach(int characterIndex in Player.Instance.selectedCharacterIndices)
        {
            if(characterIndex != -1)
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
}
