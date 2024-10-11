using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class CharacterGacha : MonoBehaviour/*, IPointerClickHandler*/
{
    public GameObject characterButtonPrefab; // 캐릭터 버튼 프리팹
    public Transform GridLayoutParent; // Grid Layout Group의 부모 오브젝트
    public GameObject resultPanel; // 뽑기 결과 패널
    public Button closeResultButton; // 결과 패널 닫기 버튼
    public Button buildUIButton; // Build UI 버튼
    public GameObject scrapUI;

    private List<Character> allCharacters; // 모든 캐릭터 데이터

    // 희귀도 배경 스프라이트 배열 (Inspector에서 설정)
    public Sprite[] rarityBackgrounds;

    void Start()
    {
        // Build UI 버튼에 클릭 이벤트 추가
        buildUIButton.onClick.AddListener(BuildGachaResult);

        // 결과 패널 닫기 버튼에 클릭 이벤트 추가
        closeResultButton.onClick.AddListener(HideResultPanel);

        // 캐릭터 데이터 로드
        allCharacters = CharacterDataLoader.LoadCharaters();

        // 결과 패널을 시작 시 숨김
        HideResultPanel();
    }

    // 가챠 결과 생성 메서드
    void BuildGachaResult()
    {
        // 뽑기 결과 초기화
        ClearPreviousResults();

        // 10명의 캐릭터를 뽑음
        List<Character> drawnCharacters = DrawCharacters(10);

        // Grid Layout Group에 뽑은 캐릭터 버튼 생성
        foreach (Character character in drawnCharacters)
        {
            GameObject characterButton = Instantiate(characterButtonPrefab, GridLayoutParent);
            // 버튼 비활성화 (해당 씬에서는 클릭 기능을 막음)
            characterButton.GetComponent<Button>().interactable = false;

            // 캐릭터 정보 표시 (예: 이미지와 이름)
            CharacterButton characterUI = characterButton.GetComponent<CharacterButton>();
            if (characterUI != null)
            {
                characterUI.SetCharacterData(character, rarityBackgrounds); // 캐릭터 데이터를 UI에 반영
            }

            // 플레이어의 소유 캐릭터에 추가
            Player.Instance.ownedCharacter.Add(character);
        }

        // 뽑은 결과 저장
        Player.Instance.SavePlayerData();

        // 결과 패널을 나타냄
        ShowResultPanel();
    }

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    // 빈 메서드로 클릭 이벤트를 막음
    //}

    // 캐릭터 뽑기 메서드
    List<Character> DrawCharacters(int count)
    {
        List<Character> drawnCharacters = new List<Character>();

        // 뽑기에서 제외할 캐릭터 필터링
        List<Character> eligibleCharacters = allCharacters.Where(c => c.shipType != "CV" && c.faction != "META").ToList();

        for (int i = 0; i < count; i++)
        {
            Character drawnCharacter = GetRandomCharacter(eligibleCharacters);
            drawnCharacters.Add(drawnCharacter);
        }

        return drawnCharacters;
    }

    // 랜덤 캐릭터 뽑기 메서드
    Character GetRandomCharacter(List<Character> characters)
    {
        float rand = Random.value; // 0 ~ 1 사이의 랜덤 값

        if (rand < 0.20f) // 20% 확률로 SSR 캐릭터
        {
            List<Character> ssrCharacters = characters.Where(c => c.rarity == "SSR").ToList();
            return ssrCharacters[Random.Range(0, ssrCharacters.Count)];
        }
        else if (rand < 0.50f) // 30% 확률로 SR 캐릭터
        {
            List<Character> srCharacters = characters.Where(c => c.rarity == "SR").ToList();
            return srCharacters[Random.Range(0, srCharacters.Count)];
        }
        else // 50% 확률로 R 캐릭터
        {
            List<Character> rCharacters = characters.Where(c => c.rarity == "R").ToList();
            return rCharacters[Random.Range(0, rCharacters.Count)];
        }
    }

    // 이전 결과 초기화 메서드
    void ClearPreviousResults()
    {
        foreach (Transform child in GridLayoutParent)
        {
            Destroy(child.gameObject);
        }
    }

    // 결과 패널을 나타내는 메서드
    void ShowResultPanel()
    {
        resultPanel.transform.localScale = Vector3.one; // 패널 나타내기
    }

    // 결과 패널을 숨기는 메서드
    void HideResultPanel()
    {
        resultPanel.transform.localScale = Vector3.zero; // 패널 숨기기
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
