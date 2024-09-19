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
