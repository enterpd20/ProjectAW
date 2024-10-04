using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCharacterDisplay : MonoBehaviour
{
    public Image characterImage; // �߾ӿ� ǥ���� ĳ���� �̹���
    private List<Character> ownedCharacters;

    // ���Ʒ��� �����̱� ���� ����
    public float floatSpeed = 2f; // ���Ʒ� ������ �ӵ�
    public float floatAmount = 10f; // ���Ʒ��� �����̴� �Ÿ�
    private Vector3 initialPosition;

    void Start()
    {
        // �÷��̾��� ���� ĳ���� ����Ʈ ��������
        ownedCharacters = Player.Instance.ownedCharacter;

        // ĳ���Ͱ� �����ϴ� ��� �������� �ϳ��� ĳ���� ��������Ʈ�� ǥ��
        if (ownedCharacters.Count > 0)
        {
            DisplayRandomCharacter();
            initialPosition = characterImage.rectTransform.localPosition; // �ʱ� ��ġ ����
        }
        else
        {
            // ĳ���Ͱ� ���� ��� �̹��� ����ȭ
            SetImageTransparent(true);
            Debug.LogWarning("������ ĳ���Ͱ� �����ϴ�.");
        }
    }

    void Update()
    {
        // ���Ʒ� ������ ����
        if (characterImage.sprite != null)
        {
            float newY = Mathf.Sin(Time.time * floatSpeed) * floatAmount;
            characterImage.rectTransform.localPosition = initialPosition + new Vector3(0, newY, 0);
        }
    }

    void DisplayRandomCharacter()
    {
        // ���� ĳ���� �� �������� �ϳ� ����
        int randomIndex = Random.Range(0, ownedCharacters.Count);
        Character randomCharacter = ownedCharacters[randomIndex];

        // ���õ� ĳ������ �̹��� �ε�
        Sprite characterSprite = Resources.Load<Sprite>($"Images_Character/{randomCharacter.imageName}");
        if (characterSprite != null)
        {
            characterImage.sprite = characterSprite; // �̹��� ������Ʈ�� ��������Ʈ ����
        }
        else
        {
            Debug.LogError($"ĳ���� ��������Ʈ�� �ε��� �� �����ϴ�: {randomCharacter.imageName}");
        }
    }

    void SetImageTransparent(bool isTransparent)
    {
        Color color = characterImage.color;
        color.a = isTransparent ? 0 : 1; // ������ ��� ���İ��� 0����, �׷��� ������ 1�� ����
        characterImage.color = color;
    }
}
