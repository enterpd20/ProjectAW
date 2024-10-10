using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCharacterDisplay : MonoBehaviour
{
    public Image characterImage; // 중앙에 표시할 캐릭터 이미지
    private List<Character> ownedCharacters;

    // 위아래로 움직이기 위한 변수
    public float floatSpeed = 2f; // 위아래 움직임 속도
    public float floatAmount = 10f; // 위아래로 움직이는 거리
    private Vector3 initialPosition;

    private float offset; // 움직임을 위해 초기 시간 값을 저장할 변수

    void Start()
    {
        offset = Time.time; // 초기화 시 현재 Time.time 값을 저장

        // 플레이어의 소유 캐릭터 리스트 가져오기
        ownedCharacters = Player.Instance.ownedCharacter;

        // 캐릭터가 존재하는 경우 무작위로 하나의 캐릭터 스프라이트를 표시
        if (ownedCharacters.Count > 0)
        {
            DisplayRandomCharacter();
            initialPosition = characterImage.rectTransform.localPosition; // 초기 위치 저장
        }
        else
        {
            // 캐릭터가 없을 경우 이미지 투명화
            SetImageTransparent(true);
            Debug.LogWarning("소유한 캐릭터가 없습니다.");
        }
    }

    void Update()
    {
        // 위아래 움직임 구현
        if (characterImage.sprite != null)
        {
            float newY = Mathf.Sin((Time.time - offset) * floatSpeed) * floatAmount;
            characterImage.rectTransform.localPosition = initialPosition + new Vector3(0, newY, 0);
        }
    }

    void DisplayRandomCharacter()
    {
        // 소유 캐릭터 중 무작위로 하나 선택
        int randomIndex = Random.Range(0, ownedCharacters.Count);
        Character randomCharacter = ownedCharacters[randomIndex];

        // 선택된 캐릭터의 이미지 로드
        Sprite characterSprite = Resources.Load<Sprite>($"Images_Character/{randomCharacter.imageName}");
        if (characterSprite != null)
        {
            characterImage.sprite = characterSprite; // 이미지 오브젝트에 스프라이트 설정
        }
        else
        {
            Debug.LogError($"캐릭터 스프라이트를 로드할 수 없습니다: {randomCharacter.imageName}");
        }
    }

    void SetImageTransparent(bool isTransparent)
    {
        Color color = characterImage.color;
        color.a = isTransparent ? 0 : 1; // 투명할 경우 알파값을 0으로, 그렇지 않으면 1로 설정
        characterImage.color = color;
    }

    void OnEnable()
    {
        Debug.Log("LobbyCharacterDisplay 활성화됨");

        if (characterImage.sprite != null)
        {
            initialPosition = characterImage.rectTransform.localPosition; // 초기 위치 재설정
        }
    }
}
