using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterButton : MonoBehaviour
{
    public Image backgroundImage;       // ĳ������ ��� ���
    public Image characterImage;        // ĳ���� �Ϸ���Ʈ
    public Text characterNameText;

    private Character characterData;

    //public DockDetailUI_Character dockDetailUI_character;

    public void SetCharacterData(Character character, Sprite[] rarityBackground)
    {
        characterData = character;

        // ��͵��� ���� ��� ����
        backgroundImage.sprite = GetRarityBackground(character.rarity/*, rarityBackground*/);

        // ĳ���� �̹��� ����
        characterImage.sprite = Resources.Load<Sprite>($"Images_Character/{character.imageName}");

        // ĳ���� �̸� ǥ��
        characterNameText.text = character.name;

        // ��ư Ŭ�� �� ĳ���� �������� �̵�
        GetComponent<Button>().onClick.AddListener(() => OnCharacterButtonClicked(character));
    }

    Sprite GetRarityBackground(string rarity/*, Sprite[] rarityBackground*/)
    {
        switch(rarity)
        {
            case "SSR": return  Resources.Load<Sprite>("RarityBG/BG_SSR");
            case "SR": return   Resources.Load<Sprite>("RarityBG/BG_SR");
            case "R": return    Resources.Load<Sprite>("RarityBG/BG_R");
            case "N": return    Resources.Load<Sprite>("RarityBG/BG_N");
            default: return Resources.Load<Sprite>("RarityBG/DefaultBackground");
        }
    }
    private void OnCharacterButtonClicked(Character character)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (characterData == null)
        {
            Debug.LogError("CharacterData is null");
            return;
        }
        
        //int characterIndex = Player.Instance.ownedCharacter.IndexOf(characterData);
        
        if(currentScene == "02_Dock")
        {
            Player.Instance.selectedCharacterIndex = Player.Instance.ownedCharacter.IndexOf(characterData);
            SceneManager.LoadScene("02_DockDetail");

            // DockDetailUI_Character�� ĳ���� �����͸� �ε��ϴ� �޼��� ȣ��
            var dockDetailUI = GameObject.FindObjectOfType<DockDetailUI_Character>();
            if (dockDetailUI != null)
            {
                dockDetailUI.LoadCharacterData();
            }
        }
        else if(currentScene == "03_SelectStage")
        {
            var selectStageUI = GameObject.FindObjectOfType<SelectStageUI>();
            if(selectStageUI != null)
            {
                selectStageUI.SetSelectedCharacter(characterData);
                selectStageUI.Close_CharacterSelectUI();
            }
        }
        else if (currentScene == "02_Build")
        {
            if (Player.Instance.ownedCharacter.Contains(characterData))
            {
                Player.Instance.ownedCharacter.Remove(characterData); // ĳ���� ����
                Player.Instance.SavePlayerData(); // ����� ������ ����
                Debug.Log($"{characterData.name}��(��) ownedCharacter���� ���ŵǾ����ϴ�.");

                // ��ư ��Ȱ��ȭ Ȥ�� UI ������Ʈ
                //gameObject.SetActive(false); // ��ư�� ��Ȱ��ȭ�Ͽ� �� �̻� ���õ��� �ʵ��� ��

                // ĳ���� ��ư�� ������ ���� (��Ȱ��ȭ�� �ƴ� ����)
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning($"{characterData.name}��(��) ownedCharacter ����Ʈ�� �����ϴ�.");
            }
        }
    }
}
