using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DockUI : MonoBehaviour
{
    private GameObject CharacterButtonPrefab;   // ĳ���� ��ư ������
    private Transform ButtonContainer;          // ��ũ�Ѻ��� ��ư���� ��ġ�� �����̳�
    private Sprite[] RarityBackGround;          // ����� ���� ��� ��������Ʈ��

    private List<Character> characterList;

    private void Start()
    {
        //characterList = 
    }

    private void NewCharacterButtons()
    {
        foreach(Character character in characterList)
        {
            GameObject newButton = Instantiate(CharacterButtonPrefab, ButtonContainer);

        }
    }
}
