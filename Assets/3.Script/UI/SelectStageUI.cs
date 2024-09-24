using UnityEngine;
using UnityEngine.UI;

public class SelectStageUI : MonoBehaviour
{
    public GameObject characterFormationUI;     // UI ǥ�ø� ���� ����
    public GameObject characterSelectUI;        // UI ǥ�ø� ���� ����

    public Image[] characterSelectButton;       // ��ġ�� ĳ���͸� ������ �� �ִ� ��ư��

    private int[] selectedButtonIndex;
    private int currentSelectedButtonIndex = -1;    // ĳ���Ͱ� ��ư�� ��ġ�Ǿ����� ���θ� ��Ÿ���� ����. -1 = ��ġ���� ����

    private void Start()
    {
        // SelectStage�� ���ƿ� ��, ĳ���� �� UI ���� ����
        if (Player.Instance.selectedCharacterIndex != -1)   // ����� ���� ĳ���Ͱ� �ִ��� Ȯ��
        {
            UpdateCharacterSelection(Player.Instance.selectedCharacterIndex);   // ���õ� ĳ���͸� ��ư�� ǥ��
        }

        characterFormationUI.SetActive(true);   // ĳ���� �� UI�� Ȱ��ȭ�Ͽ� ���̰� ����
    }

    // ���õ� ��ư�� �ε����� ����� �ش� ��ư�� ĳ���� �̹����� ������Ʈ
    private void UpdateCharacterSelection(int buttonIndex)
    {
        Character selectedCharacter = Player.Instance.GetSelectedCharacter();   // ���� ���õ� ĳ���� �ҷ���

        // ���õ� ĳ���Ͱ� �����ϰ� ��ȿ�� ��ư �ε������� Ȯ��
        if (selectedCharacter != null && buttonIndex >= 0 && buttonIndex < characterSelectButton.Length)
        {
            // �ش� ��ư�� ���õ� ĳ������ �̹��� ����
            characterSelectButton[buttonIndex].sprite =
                Resources.Load<Sprite>($"Images_Character/{selectedCharacter.imageName}");
        }
    }

    public void SetSelectedCharacter(Character character)
    {
        // ���õ� ĳ������ �ε����� ����
        Player.Instance.selectedCharacterIndex = Player.Instance.ownedCharacter.IndexOf(character);

        // ���õ� ĳ���� ������ ����
        Player.Instance.SavePlayerData();   

        // ���� ���õ� ��ư�� ���õ� ĳ���� �̹����� ������Ʈ
        UpdateCharacterSelection(currentSelectedButtonIndex);
    }

    // Ư�� ��ư�� �����ϸ�, �� ��ư�� �ε����� �����ϰ� ĳ���� ���� UI�� Ȱ��ȭ
    public void SelectButton(int buttonIndex)   
    {
        currentSelectedButtonIndex = buttonIndex;
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
}
