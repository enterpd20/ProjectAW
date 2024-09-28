using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SelectStageUI : MonoBehaviour
{
    public GameObject characterFormationUI;     // UI ǥ�ø� ���� ����
    public GameObject characterSelectUI;        // UI ǥ�ø� ���� ����
    public GameObject confirmButton;
    public GameObject warningMessage;

    public Image[] characterSelectButton;       // ��ġ�� ĳ���͸� ������ �� �ִ� ��ư��

    private int currentSelectedButtonIndex = -1;    // ĳ���Ͱ� ��ư�� ��ġ�Ǿ����� ���θ� ��Ÿ���� ����. -1 = ��ġ���� ����

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
        if (characterIndex != -1)
        {
            Character selectedCharacter = Player.Instance.GetSelectedCharacter_SelectStage(characterIndex);
            if(selectedCharacter != null)
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
        for(int i = 0; i < characterSelectButton.Length; i++)
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
            Debug.Log($"Assigning {character.name} to button {currentSelectedButtonIndex}");
            Player.Instance.selectedCharacterIndices[currentSelectedButtonIndex] = characterIndex;
            Player.Instance.SavePlayerData();
            UpdateCharacterSelection(currentSelectedButtonIndex);
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
        if(IsAnyCharacterAssigned())
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
        foreach(int characterIndex in Player.Instance.selectedCharacterIndices)
        {
            if(characterIndex != -1)
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
}
