using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterButton : MonoBehaviour
{
    public Image backgroundImage;       // 캐릭터의 레어도 배경
    public Image characterImage;        // 캐릭터 일러스트
    public Text characterNameText;

    private Character characterData;

    //public DockDetailUI_Character dockDetailUI_character;

    public void SetCharacterData(Character character, Sprite[] rarityBackground)
    {
        //if (character == null)
        //{
        //    Debug.LogError("Character is null");
        //    return;
        //}
        //
        //if (rarityBackground == null || rarityBackground.Length < 4)
        //{
        //    Debug.LogError("Rarity backgrounds array is null or has insufficient elements");
        //    return;
        //}
        //
        //// 확인: backgroundImage와 characterImage가 null인지 확인
        //if (backgroundImage == null)
        //{
        //    Debug.LogError("BackgroundImage is not assigned");
        //    return;
        //}
        //
        //if (characterImage == null)
        //{
        //    Debug.LogError("CharacterImage is not assigned");
        //    return;
        //}

        characterData = character;

        // 희귀도에 따라 배경 설정
        backgroundImage.sprite = GetRarityBackground(character.rarity/*, rarityBackground*/);

        // 캐릭터 이미지 설정
        characterImage.sprite = Resources.Load<Sprite>($"Images_Character/{character.imageName}");

        // 캐릭터 이름 표시
        characterNameText.text = character.name;

        // 버튼 클릭 시 캐릭터 상세정보로 이동
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
    }
}
