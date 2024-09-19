using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DockUI : MonoBehaviour
{
    public GameObject CharacterButtonPrefab;   // 캐릭터 버튼 프리팹
    public Transform ButtonContainer;          // 스크롤뷰의 버튼들이 배치될 컨테이너
    public Sprite[] RarityBackGround;          // 레어도에 따른 배경 스프라이트들

    private List<Character> characterList;

    private void Start()
    {
        if (Player.Instance.ownedCharacter == null)
        {
            Debug.LogError("Player data is not loaded yet.");
            return;
        }

        //characterList = CharacterDataLoader.LoadCharaters();
        characterList = Player.Instance.ownedCharacter;

        //CreateCharacterButtons();
        StartCoroutine(LoadAndCreateButtons());
    }

    private IEnumerator LoadAndCreateButtons()  // ???
    {
        yield return new WaitUntil(() => Player.Instance.ownedCharacter != null && Player.Instance.ownedCharacter.Count > 0);
        characterList = Player.Instance.ownedCharacter;
        CreateCharacterButtons();
    }

    private void CreateCharacterButtons()
    {
        foreach(Character character in characterList)
        {
            GameObject newButton = Instantiate(CharacterButtonPrefab, ButtonContainer);
            CharacterButton characterButton = newButton.GetComponent<CharacterButton>();

            if (characterButton == null)
            {
                Debug.LogError("CharacterButton component is missing on the instantiated prefab.");
                continue;
            }

            // 확인: RarityBackGround와 CharacterButtonPrefab이 제대로 설정되어 있는지 확인
            if (RarityBackGround == null || RarityBackGround.Length < 4)
            {
                Debug.LogError("RarityBackGround array is not properly assigned.");
                continue;
            }

            characterButton.SetCharacterData(character, RarityBackGround);
        }
    }
}
