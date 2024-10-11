using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class CharacterGacha : MonoBehaviour/*, IPointerClickHandler*/
{
    public GameObject characterButtonPrefab; // ĳ���� ��ư ������
    public Transform GridLayoutParent; // Grid Layout Group�� �θ� ������Ʈ
    public GameObject resultPanel; // �̱� ��� �г�
    public Button closeResultButton; // ��� �г� �ݱ� ��ư
    public Button buildUIButton; // Build UI ��ư
    public GameObject scrapUI;

    private List<Character> allCharacters; // ��� ĳ���� ������

    // ��͵� ��� ��������Ʈ �迭 (Inspector���� ����)
    public Sprite[] rarityBackgrounds;

    void Start()
    {
        // Build UI ��ư�� Ŭ�� �̺�Ʈ �߰�
        buildUIButton.onClick.AddListener(BuildGachaResult);

        // ��� �г� �ݱ� ��ư�� Ŭ�� �̺�Ʈ �߰�
        closeResultButton.onClick.AddListener(HideResultPanel);

        // ĳ���� ������ �ε�
        allCharacters = CharacterDataLoader.LoadCharaters();

        // ��� �г��� ���� �� ����
        HideResultPanel();
    }

    // ��í ��� ���� �޼���
    void BuildGachaResult()
    {
        // �̱� ��� �ʱ�ȭ
        ClearPreviousResults();

        // 10���� ĳ���͸� ����
        List<Character> drawnCharacters = DrawCharacters(10);

        // Grid Layout Group�� ���� ĳ���� ��ư ����
        foreach (Character character in drawnCharacters)
        {
            GameObject characterButton = Instantiate(characterButtonPrefab, GridLayoutParent);
            // ��ư ��Ȱ��ȭ (�ش� �������� Ŭ�� ����� ����)
            characterButton.GetComponent<Button>().interactable = false;

            // ĳ���� ���� ǥ�� (��: �̹����� �̸�)
            CharacterButton characterUI = characterButton.GetComponent<CharacterButton>();
            if (characterUI != null)
            {
                characterUI.SetCharacterData(character, rarityBackgrounds); // ĳ���� �����͸� UI�� �ݿ�
            }

            // �÷��̾��� ���� ĳ���Ϳ� �߰�
            Player.Instance.ownedCharacter.Add(character);
        }

        // ���� ��� ����
        Player.Instance.SavePlayerData();

        // ��� �г��� ��Ÿ��
        ShowResultPanel();
    }

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    // �� �޼���� Ŭ�� �̺�Ʈ�� ����
    //}

    // ĳ���� �̱� �޼���
    List<Character> DrawCharacters(int count)
    {
        List<Character> drawnCharacters = new List<Character>();

        // �̱⿡�� ������ ĳ���� ���͸�
        List<Character> eligibleCharacters = allCharacters.Where(c => c.shipType != "CV" && c.faction != "META").ToList();

        for (int i = 0; i < count; i++)
        {
            Character drawnCharacter = GetRandomCharacter(eligibleCharacters);
            drawnCharacters.Add(drawnCharacter);
        }

        return drawnCharacters;
    }

    // ���� ĳ���� �̱� �޼���
    Character GetRandomCharacter(List<Character> characters)
    {
        float rand = Random.value; // 0 ~ 1 ������ ���� ��

        if (rand < 0.20f) // 20% Ȯ���� SSR ĳ����
        {
            List<Character> ssrCharacters = characters.Where(c => c.rarity == "SSR").ToList();
            return ssrCharacters[Random.Range(0, ssrCharacters.Count)];
        }
        else if (rand < 0.50f) // 30% Ȯ���� SR ĳ����
        {
            List<Character> srCharacters = characters.Where(c => c.rarity == "SR").ToList();
            return srCharacters[Random.Range(0, srCharacters.Count)];
        }
        else // 50% Ȯ���� R ĳ����
        {
            List<Character> rCharacters = characters.Where(c => c.rarity == "R").ToList();
            return rCharacters[Random.Range(0, rCharacters.Count)];
        }
    }

    // ���� ��� �ʱ�ȭ �޼���
    void ClearPreviousResults()
    {
        foreach (Transform child in GridLayoutParent)
        {
            Destroy(child.gameObject);
        }
    }

    // ��� �г��� ��Ÿ���� �޼���
    void ShowResultPanel()
    {
        resultPanel.transform.localScale = Vector3.one; // �г� ��Ÿ����
    }

    // ��� �г��� ����� �޼���
    void HideResultPanel()
    {
        resultPanel.transform.localScale = Vector3.zero; // �г� �����
    }

    public void ShowScrapUIPanel()
    {
        scrapUI.transform.localScale = Vector3.one;
    }

    public void HideScrapUIPanel()
    {
        scrapUI.transform.localScale = Vector3.zero;
    }
}
