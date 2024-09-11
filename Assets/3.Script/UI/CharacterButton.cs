using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour
{
    public Image backgroundImage;
    public Image characterImage;

    public DockDetailUI dockDetailUI;

    public void SetCharacterData(Character character, Sprite[] rarityBackground)
    {
        // 희귀도에 따라 배경 설정
        backgroundImage.sprite = GetRarityBackground(character.rarity, rarityBackground);

        // 캐릭터 이미지 설정
        characterImage.sprite = Resources.Load<Sprite>($"CharacterImages/{character.imageName}");

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
            default: return rarityBackground[4];
        }
    }
    private void OnCharacterButtonClicked(Character character)
    {
        dockDetailUI.CharacterDetail(character);
    }
}
