using UnityEngine;
using UnityEngine.UI;

public class SelectStageUI : MonoBehaviour
{
    public GameObject characterFormationUI;     // UI 표시를 위한 변수
    public GameObject characterSelectUI;        // UI 표시를 위한 변수

    public Image[] characterSelectButton;       // 배치할 캐릭터를 선택할 수 있는 버튼들

    private int[] selectedButtonIndex;
    private int currentSelectedButtonIndex = -1;    // 캐릭터가 버튼에 배치되었는지 여부를 나타내는 변수. -1 = 배치되지 않음

    private void Start()
    {
        // SelectStage로 돌아올 때, 캐릭터 편성 UI 상태 복원
        if (Player.Instance.selectedCharacterIndex != -1)   // 저장된 선택 캐릭터가 있는지 확인
        {
            UpdateCharacterSelection(Player.Instance.selectedCharacterIndex);   // 선택된 캐릭터를 버튼에 표시
        }

        characterFormationUI.SetActive(true);   // 캐릭터 편성 UI를 활성화하여 보이게 설정
    }

    // 선택된 버튼의 인덱스를 사용해 해당 버튼에 캐릭터 이미지를 업데이트
    private void UpdateCharacterSelection(int buttonIndex)
    {
        Character selectedCharacter = Player.Instance.GetSelectedCharacter();   // 현재 선택된 캐릭터 불러옴

        // 선택된 캐릭터가 존재하고 유효한 버튼 인덱스인지 확인
        if (selectedCharacter != null && buttonIndex >= 0 && buttonIndex < characterSelectButton.Length)
        {
            // 해당 버튼에 선택된 캐릭터의 이미지 설정
            characterSelectButton[buttonIndex].sprite =
                Resources.Load<Sprite>($"Images_Character/{selectedCharacter.imageName}");
        }
    }

    public void SetSelectedCharacter(Character character)
    {
        // 선택된 캐릭터의 인덱스를 저장
        Player.Instance.selectedCharacterIndex = Player.Instance.ownedCharacter.IndexOf(character);

        // 선택된 캐릭터 데이터 저장
        Player.Instance.SavePlayerData();   

        // 현재 선택된 버튼에 선택된 캐릭터 이미지를 업데이트
        UpdateCharacterSelection(currentSelectedButtonIndex);
    }

    // 특정 버튼을 선택하면, 그 버튼의 인덱스를 저장하고 캐릭터 선택 UI를 활성화
    public void SelectButton(int buttonIndex)   
    {
        currentSelectedButtonIndex = buttonIndex;
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
}
