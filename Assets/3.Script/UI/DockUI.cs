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
        characterList = CharacterDataLoader.LoadCharaters();

        NewCharacterButtons();
    }

    private void NewCharacterButtons()
    {
        foreach(Character character in characterList)
        {
            GameObject newButton = Instantiate(CharacterButtonPrefab, ButtonContainer);
            CharacterButton characterButton = newButton.GetComponent<CharacterButton>();

            characterButton.SetCharacterData(character, RarityBackGround);
        }
    }
}
