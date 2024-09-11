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
        // ��͵��� ���� ��� ����
        backgroundImage.sprite = GetRarityBackground(character.rarity, rarityBackground);

        // ĳ���� �̹��� ����
        characterImage.sprite = Resources.Load<Sprite>($"CharacterImages/{character.imageName}");

        // ��ư Ŭ�� �� ĳ���� �������� �̵�
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
