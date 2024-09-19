using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterButton : MonoBehaviour
{
    public Image backgroundImage;       // 캐릭터의 레어도 배경
    public Image characterImage;        // 캐릭터 일러스트

    private Character characterData;

    //public DockDetailUI_Character dockDetailUI_character;

    public void SetCharacterData(Character character, Sprite[] rarityBackground)
    {
        if (character == null)
        {
            Debug.LogError("Character is null");
            return;
        }

        if (rarityBackground == null || rarityBackground.Length < 4)
        {
            Debug.LogError("Rarity backgrounds array is null or has insufficient elements");
            return;
        }

        // 확인: backgroundImage와 characterImage가 null인지 확인
        if (backgroundImage == null)
        {
            Debug.LogError("BackgroundImage is not assigned");
            return;
        }

        if (characterImage == null)
        {
            Debug.LogError("CharacterImage is not assigned");
            return;
        }

        characterData = character;

        // 희귀도에 따라 배경 설정
        backgroundImage.sprite = GetRarityBackground(character.rarity, rarityBackground);

        // 캐릭터 이미지 설정
        characterImage.sprite = Resources.Load<Sprite>($"Images_Character/{character.imageName}");

        // 버튼 클릭 시 캐릭터 상세정보로 이동
        GetComponent<Button>().onClick.AddListener(() => OnCharacterButtonClicked(character));
    }

    Sprite GetRarityBackground(string rarity, Sprite[] rarityBackground)
    {
        switch(rarity)
        {
            case "SSR": return rarityBackground[0];
            case "SR": return rarityBackground[1];
            case "R": return rarityBackground[2];
            case "N": return rarityBackground[3];
            //default: return rarityBackground[4];
            default: return rarityBackground.Length > 4 ? rarityBackground[4] : null;
        }
    }
    private void OnCharacterButtonClicked(Character character)
    {
        if (characterData == null)
        {
            Debug.LogError("CharacterData is null");
            return;
        }

        Player.Instance.selectedCharacterIndex = Player.Instance.ownedCharacter.IndexOf(characterData);

        SceneManager.LoadScene("02_DockDetail");

        // DockDetailUI 인스턴스를 통해 CharacterDetail 호출
        // dockDetailUI_character.CharacterDetail(character);        
    }
}
