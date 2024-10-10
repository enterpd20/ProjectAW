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

            // DockDetailUI_Character에 캐릭터 데이터를 로드하는 메서드 호출
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
                Player.Instance.ownedCharacter.Remove(characterData); // 캐릭터 제거
                Player.Instance.SavePlayerData(); // 변경된 데이터 저장
                Debug.Log($"{characterData.name}이(가) ownedCharacter에서 제거되었습니다.");

                // 버튼 비활성화 혹은 UI 업데이트
                //gameObject.SetActive(false); // 버튼을 비활성화하여 더 이상 선택되지 않도록 함

                // 캐릭터 버튼을 완전히 삭제 (비활성화가 아닌 삭제)
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning($"{characterData.name}이(가) ownedCharacter 리스트에 없습니다.");
            }
        }
    }
}
