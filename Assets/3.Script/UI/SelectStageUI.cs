using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SelectStageUI : MonoBehaviour
{
    public GameObject characterFormationUI;     // UI ǥ�ø� ���� ����
    public GameObject characterSelectUI;        // UI ǥ�ø� ���� ����
    public GameObject confirmButton;            // UI ǥ�ø� ���� ����
    public GameObject warningMessage;           // UI ǥ�ø� ���� ����
    public Sprite emptySlotSprite;              // UI ǥ�ø� ���� ����

    public Image[] characterSelectButton;       // ��ġ�� ĳ���͸� ������ �� �ִ� ��ư��

    private int currentSelectedButtonIndex = -1;    // ĳ���Ͱ� ��ư�� ��ġ�Ǿ����� ���θ� ��Ÿ���� ����. -1 = ��ġ���� ����
    public Transform characterListContainer;    // ĳ���� ��� UI�� �θ� ��ü
    public GameObject characterListItemPrefab;  // ĳ���� ��� ������ ������


    private void Start()
    {
        if (Player.Instance.selectedCharacterIndices == null)
        {
            Debug.LogError("selectedCharacterIndices is not initialized properly.");
            Player.Instance.InitializePlayerData();  // �迭�� null�̸� �ʱ�ȭ
        }

        //Player.Instance.InitializeIndices(characterSelectButton.Length);
        Debug.Log("selectedCharacterIndices Length: " + Player.Instance.selectedCharacterIndices.Length);

        UpdateAllCharacterSelections();
        characterFormationUI.SetActive(true);   // ĳ���� �� UI�� Ȱ��ȭ�Ͽ� ���̰� ����
    }

    // ���õ� ��ư�� �ε����� ����� �ش� ��ư�� ĳ���� �̹����� ������Ʈ
    private void UpdateCharacterSelection(int buttonIndex)
    {
        //if (Player.Instance.selectedCharacterIndices == null)
        //{
        //    Debug.LogError("selectedCharacterIndices is not initialized properly.");
        //    Debug.LogError("selectedCharacterIndices == null");
        //    return;
        //}
        //if(Player.Instance.selectedCharacterIndices.Length != 6)
        //{
        //    Debug.LogError("selectedCharacterIndices.Length != 6");
        //    return;
        //}

        int characterIndex = Player.Instance.selectedCharacterIndices[buttonIndex];
        //Debug.Log($"Button {buttonIndex} - characterIndex: {characterIndex}");

        // ������ �ʱ�ȭ�Ͽ� ��������Ʈ�� ����
        characterSelectButton[buttonIndex].sprite = emptySlotSprite;


        if (characterIndex != -1)
        {
            Character selectedCharacter = Player.Instance.GetSelectedCharacter_SelectStage(characterIndex);
            if (selectedCharacter != null)
            {
                characterSelectButton[buttonIndex].sprite =
                    Resources.Load<Sprite>($"Images_Character/{selectedCharacter.imageName}");
            }
            else
            {
                Debug.LogError($"No character found for index {characterIndex}");
            } 
        }
        //else
        //{
        //    Debug.LogWarning($"No character assigned to button {buttonIndex}");
        //}
    }

    private void UpdateAllCharacterSelections()
    {
        for (int i = 0; i < characterSelectButton.Length; i++)
        {
            UpdateCharacterSelection(i);
        }
    }

    public void SetSelectedCharacter(Character character)
    {
        int characterIndex = Player.Instance.ownedCharacter.IndexOf(character);
        Debug.Log($"Character index for {character.name}: {characterIndex}");

        if (characterIndex != -1 && currentSelectedButtonIndex != -1)
        {
            //// ���� ��ư�� �ùٸ� ������ ��ġ�� �� �ֵ��� �˻�
            //if (IsValidShipTypeForButton(character.shipType, currentSelectedButtonIndex))
            //{
            //    Debug.Log($"Assigning {character.name} to button {currentSelectedButtonIndex}");
            //    Player.Instance.selectedCharacterIndices[currentSelectedButtonIndex] = characterIndex;
            //    Player.Instance.SavePlayerData();
            //    UpdateCharacterSelection(currentSelectedButtonIndex);
            //    Close_CharacterSelectUI();
            //}
            //else
            //{
            //    Debug.LogError($"Cannot assign {character.shipType} to button {currentSelectedButtonIndex}");
            //}

            // ���� ��ư�� �ùٸ� ������ ��ġ�� �� �ֵ��� �˻�
            if (IsValidShipTypeForButton(character.shipType, currentSelectedButtonIndex))
            {


                // �̹� ���� ĳ�������� Ȯ���ϰ� �ߺ� ����
                for (int i = 0; i < Player.Instance.selectedCharacterIndices.Length; i++)
                {
                    if (Player.Instance.selectedCharacterIndices[i] == characterIndex)
                    {
                        // �̹� ���� ĳ���͸� ���� ������ ĭ���� �̵�
                        Debug.Log($"Moving character {character.name} from slot {i} to slot {currentSelectedButtonIndex}");
                        Player.Instance.selectedCharacterIndices[i] = -1; // ���� ĭ ����
                        break;
                    }
                }

                // ĳ���͸� ���õ� ĭ�� ��ġ
                Debug.Log($"Assigning {character.name} to button {currentSelectedButtonIndex}");
                Player.Instance.selectedCharacterIndices[currentSelectedButtonIndex] = characterIndex;
                Player.Instance.SavePlayerData();
                UpdateAllCharacterSelections(); // ��ü ������Ʈ�� UI �ݿ�
                Close_CharacterSelectUI();
            }
            else
            {
                Debug.LogError($"Cannot assign {character.shipType} to button {currentSelectedButtonIndex}");
            }
        }
        else
        {
            Debug.LogError($"Character selection or button selection failed.");
        }
    }

    // Ư�� ��ư�� �����ϸ�, �� ��ư�� �ε����� �����ϰ� ĳ���� ���� UI�� Ȱ��ȭ
    public void SelectButton(int buttonIndex)   // ��ư ������Ʈ�� �ν����Ϳ��� OnClick�� �Ҵ��� �޼���
    {
        currentSelectedButtonIndex = buttonIndex;
        Debug.Log($"Selected button index: {currentSelectedButtonIndex}");
        Open_CharacterSelectUI();

        // ĳ���� ����� ���͸��Ͽ� UI ������Ʈ
        FilterCharacterListByButton(buttonIndex);
    }

    // ��ư �ε����� ���� ��ġ ������ ������ �˻��ϴ� �޼���
    private bool IsValidShipTypeForButton(string shipType, int buttonIndex)
    {
        if (buttonIndex < 3) // ù 3ĭ�� BB, CV��
        {
            return shipType == "BB" || shipType == "CV";
        }
        else // ������ 3ĭ�� DD, CLCA��
        {
            return shipType == "DD" || shipType == "CLCA";
        }
    }

    // ��ư �ε����� ���� ĳ���� ����� ���͸��ϴ� �޼���
    private void FilterCharacterListByButton(int buttonIndex)
    {
        // ���� ��� ����
        foreach (Transform child in characterListContainer)
        {
            Destroy(child.gameObject);
        }

        // ���͸��� ĳ���� �߰�
        foreach (Character character in Player.Instance.ownedCharacter)
        {
            if (IsValidShipTypeForButton(character.shipType, buttonIndex))
            {
                GameObject listItem = Instantiate(characterListItemPrefab, characterListContainer);

                // ��͵� ��� ���� (�ڽ� ������Ʈ�� ù ��° �ε���)
                Image rarityBackground = listItem.transform.GetChild(0).GetComponent<Image>();
                rarityBackground.sprite = GetRarityBackground(character.rarity);

                // ĳ���� ��������Ʈ ���� (�ڽ� ������Ʈ�� �� ��° �ε���)
                Image characterImage = listItem.transform.GetChild(1).GetComponent<Image>();
                characterImage.sprite = Resources.Load<Sprite>($"Images_Character/{character.imageName}");

                listItem.GetComponentInChildren<Text>().text = character.name; // ĳ���� �̸� ǥ��

                listItem.GetComponent<Button>().onClick.AddListener(() => SetSelectedCharacter(character));
            }
        }
    }

    public void Open_CharacterFormationUI()
    {
        characterFormationUI.transform.localScale = Vector3.one;
    }

    public void Close_CharacterFormationUI()
    {
        characterFormationUI.transform.localScale = Vector3.zero;
    }

    public void Open_CharacterSelectUI()
    {
        characterSelectUI.transform.localScale = Vector3.one;
    }

    public void Close_CharacterSelectUI()
    {
        characterSelectUI.transform.localScale = Vector3.zero;
    }

    public void Change_ToBattleScene()
    {
        if (IsAnyCharacterAssigned())
        {
            SceneManager.LoadScene("04_Battle");
        }
        else
        {
            StartCoroutine(ShowWarningMessage());
        }
    }

    private bool IsAnyCharacterAssigned()
    {
        foreach (int characterIndex in Player.Instance.selectedCharacterIndices)
        {
            if (characterIndex != -1)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator ShowWarningMessage()
    {
        warningMessage.transform.localScale = Vector3.one;
        yield return new WaitForSeconds(1.5f);
        warningMessage.transform.localScale = Vector3.zero;
    }

    // ��͵��� ���� ����� �������� �޼���
    private Sprite GetRarityBackground(string rarity)
    {
        switch (rarity)
        {
            case "SSR": return Resources.Load<Sprite>("RarityBG/BG_SSR");
            case "SR": return Resources.Load<Sprite>("RarityBG/BG_SR");
            case "R": return Resources.Load<Sprite>("RarityBG/BG_R");
            case "N": return Resources.Load<Sprite>("RarityBG/BG_N");
            default: return Resources.Load<Sprite>("RarityBG/DefaultBackground");
        }
    }
}
