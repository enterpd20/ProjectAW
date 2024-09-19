using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DockDetailUI_Character : MonoBehaviour
{    
    public Text character_ShipType;                       // 일러스트 위에 표시할 캐릭터의 함종
    [SerializeField] private Image character_Image;       // 캐릭터 상세정보에 띄울 캐릭터 일러스트
    [SerializeField] private Text character_Name;         // 일러스트 위에 표시할 캐릭터의 이름
    //public Text Character_ShipName;     

    // 캐릭터 일러스트 오른쪽의 스탯창에 표시될 스탯 = 캐릭터 기본 스탯 + 현재 캐릭터가 장착한 장비 스탯
    [SerializeField] private Text finalCharacterStats_HP;
    [SerializeField] private Text finalCharacterStats_FP;
    [SerializeField] private Text finalCharacterStats_TRP;
    [SerializeField] private Text finalCharacterStats_AVI;
    [SerializeField] private Text finalCharacterStats_AA;
    [SerializeField] private Text finalCharacterStats_SPD;

    [SerializeField] private RectTransform Rect_StatusUI;

    //private Character currentCharacter;
    private List<Gear> equippedGears;

    //public void CharacterDetail(Character character)
    //{
    //    character_Image.sprite = Resources.Load<Sprite>($"Images_Character/{character.imageName}");
    //    character_Name.text = character.name;
    //    //Character_ShipName.text = character.shipname;
    //    character_ShipType.text = character.shipType;
    //
    //    //currentCharacter = character;
    //}

    private void Start()
    {
        LoadCharacterData();
    }

    private void LoadCharacterData()
    {
        Character character = Player.Instance.GetSelectedCharacter();

        if(character != null)
        {
            // UI에 캐릭터 정보 표시
            character_Image.sprite = Resources.Load<Sprite>($"Images_Character/{character.imageName}");
            character_Name.text = character.name;
            character_ShipType.text = character.shipType;

            // 장비 정보 가져오기
            List<Gear> equippedGears = GetEquippedGears(character.shipType);

            // 캐릭터 스탯 업데이트
            UpdateCharacterStats(character, equippedGears);
        }
        else
        {
            Debug.LogWarning("No character selected.");
        }
    }

    private List<Gear> GetEquippedGears(string shipType)
    {
        List<Gear> equippedGears = new List<Gear>();
        DockDetailUI_EquipSlot equipSlotManager = FindObjectOfType<DockDetailUI_EquipSlot>();

        if (equipSlotManager == null)
        {
            Debug.LogError("DockDetailUI_EquipSlot is not found!");
            return equippedGears;  // 빈 리스트 반환
        }

        // 각 슬롯에 장착된 장비 유형을 가져옴
        for (int i = 0; i < 4; i++)
        {
            Text equipTypeText = equipSlotManager.GetComponentInChildren<Text>();
            string gearType = equipTypeText.text;

            Gear gear = GearDataLoader.LoadAllGears().Find(g => g.gearType == gearType);
            if (gear != null)
            {
                equippedGears.Add(gear);
            }
        }

        return equippedGears;
    }

    public void UpdateCharacterStats(Character character, List<Gear> equippedGears)
    {
        //this.equippedGears = equippedGears;

        CharacterStats finalStats = new CharacterStats
        {
            HP = character.stats.HP,
            FP = character.stats.FP,
            TRP = character.stats.TRP,
            AVI = character.stats.AVI,
            AA = character.stats.AA,
            SPD = character.stats.SPD
        };

        foreach(var gear in equippedGears)
        {
            finalStats.HP  += gear.stats.HP;
            finalStats.FP  += gear.stats.FP;
            finalStats.TRP += gear.stats.TRP;
            finalStats.AVI += gear.stats.AVI;
            finalStats.AA  += gear.stats.AA;
            finalStats.AA  += gear.stats.AA;
            finalStats.SPD += gear.stats.SPD;
        }

        finalCharacterStats_HP.text =  finalStats.HP.ToString();
        finalCharacterStats_FP.text =  finalStats.FP.ToString();
        finalCharacterStats_TRP.text = finalStats.TRP.ToString();
        finalCharacterStats_AVI.text = finalStats.AVI.ToString();
        finalCharacterStats_AA.text =  finalStats.AA.ToString();
        finalCharacterStats_SPD.text = finalStats.SPD.ToString();
    }

    public void Gear_MoveChracterIllust()   // Gear 버튼 누르면 일러스트가 중앙으로 이동하는 메서드
    {
        StartCoroutine(MoveCharacterImage_co
            (new Vector3(105, character_Image.rectTransform.anchoredPosition.y, 0), 1.0f));
    }

    public void Info_MoveChracterIllust()   // Info 버튼 누르면 일러스트가 다시 왼쪽으로 이동하는 메서드
    {
        StartCoroutine(MoveCharacterImage_co
            (new Vector3(-365, character_Image.rectTransform.anchoredPosition.y, 0), 1.0f));
    }

    private IEnumerator MoveCharacterImage_co(Vector3 targetPosition, float duration)
    {
        // 시작 위치 저장
        Vector3 startPosition = character_Image.rectTransform.anchoredPosition;
        float timeElapsed = 0;

        while(timeElapsed < duration)
        {
            // 현재 시간에 비례해서 위치 업데이트
            character_Image.rectTransform.anchoredPosition = 
                Vector3.Lerp(startPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // 이동이 끝난 후 최종 위치 설정
        character_Image.rectTransform.anchoredPosition = targetPosition;
    }

    public void Appear_StatusUI()
    {
        Rect_StatusUI.localScale = Vector3.one;
    }
    
    public void Disappear_StatusUI()
    {
        Rect_StatusUI.localScale = Vector3.zero;
    }
}