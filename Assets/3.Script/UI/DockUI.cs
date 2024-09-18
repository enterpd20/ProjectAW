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

            // Ȯ��: RarityBackGround�� CharacterButtonPrefab�� ����� �����Ǿ� �ִ��� Ȯ��
            if (RarityBackGround == null || RarityBackGround.Length < 4)
            {
                Debug.LogError("RarityBackGround array is not properly assigned.");
                continue;
            }

            characterButton.SetCharacterData(character, RarityBackGround);
        }
    }
}
