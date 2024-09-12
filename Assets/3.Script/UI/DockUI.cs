using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DockUI : MonoBehaviour
{
    public GameObject CharacterButtonPrefab;   // ĳ���� ��ư ������
    public Transform ButtonContainer;          // ��ũ�Ѻ��� ��ư���� ��ġ�� �����̳�
    public Sprite[] RarityBackGround;          // ����� ���� ��� ��������Ʈ��

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
