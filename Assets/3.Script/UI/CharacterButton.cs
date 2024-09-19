using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterButton : MonoBehaviour
{
    public Image backgroundImage;       // ĳ������ ��� ���
    public Image characterImage;        // ĳ���� �Ϸ���Ʈ

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

        // Ȯ��: backgroundImage�� characterImage�� null���� Ȯ��
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

        // ��͵��� ���� ��� ����
        backgroundImage.sprite = GetRarityBackground(character.rarity, rarityBackground);

        // ĳ���� �̹��� ����
        characterImage.sprite = Resources.Load<Sprite>($"Images_Character/{character.imageName}");

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

        // DockDetailUI �ν��Ͻ��� ���� CharacterDetail ȣ��
        // dockDetailUI_character.CharacterDetail(character);        
    }
}
