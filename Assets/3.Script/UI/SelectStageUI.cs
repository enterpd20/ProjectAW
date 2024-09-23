using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectStageUI : MonoBehaviour
{
    public Image[] characterSelectButton;

    public GameObject characterFormationUI;

    private void Start()
    {
        // SelectStage로 돌아올 때, 캐릭터 편성 UI 상태 복원
        if(Player.Instance.selectedCharacterIndex != -1)
        {
            UpdateCharacterSelection();
        }

        characterFormationUI.SetActive(true);
    }

    private void UpdateCharacterSelection()
    {
        Character selectedCharacter = Player.Instance.GetSelectedCharacter();
        if(selectedCharacter != null)
        {
            for(int i = 0; i < characterSelectButton.Length; i++)
            {
            characterSelectButton[i].sprite = Resources.Load<Sprite>($"Images_Character/{selectedCharacter.imageName}");
            }
        }
    }

    public void ShowFormationUI()
    {
        characterFormationUI.SetActive(true);
    }

    public void OnCharacterSelectedButtonPress()
    {
        SceneChangingManager.Instance.ChangeScene("02_Dock");
    }

    public void Open_SelectCharacterUI()
    {
        characterFormationUI.transform.localScale = Vector3.one;
    }

    public void Close_SelectCharacterUI()
    {
        characterFormationUI.transform.localScale = Vector3.zero;
    }
}
