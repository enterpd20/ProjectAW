using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DockDetailUI_Character : MonoBehaviour
{    
    [SerializeField] private Image character_Image;       // 캐릭터 상세정보에 띄울 캐릭터 일러스트
    [SerializeField] private Text character_Name;         // 일러스트 위에 표시할 캐릭터의 이름
    [SerializeField] private Text character_ShipType;     // 일러스트 위에 표시할 캐릭터의 함종
    //public Text Character_ShipName;     

    // 캐릭터 일러스트 오른쪽의 스탯창에 표시될 스탯 = 캐릭터 기본 스탯 + 현재 캐릭터가 장착한 장비 스탯
    [SerializeField] private Text finalCharacterStats_HP;
    [SerializeField] private Text finalCharacterStats_FP;
    [SerializeField] private Text finalCharacterStats_TRP;
    [SerializeField] private Text finalCharacterStats_AVI;
    [SerializeField] private Text finalCharacterStats_AA;
    [SerializeField] private Text finalCharacterStats_SPD;
    
    public void CharacterDetail(Character character)
    {
        character_Image.sprite = Resources.Load<Sprite>($"Images_Character/{character.imageName}");
        character_Name.text = character.name;
        //Character_ShipName.text = character.shipname;
        character_ShipType.text = character.shipType;
    }

    public void UpdateCharacterStats(Character character, List<Gear> equippedGears)
    {
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
}