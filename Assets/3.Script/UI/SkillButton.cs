using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    public GameObject skillButtonPrefab; // 스킬 버튼 프리팹
    public Transform skillButtonParent;  // 스킬 버튼들을 담을 부모 오브젝트

    private List<Character> spawnedAllies;

    private void Start()
    {
        CharacterSpawner.SpawnComplete += InitializeSkillButtons;
    }

    private void InitializeSkillButtons()
    {
        spawnedAllies = FindObjectOfType<CharacterSpawner>().spawnedAllies;

        // 스폰된 아군 캐릭터의 수에 맞게 스킬 버튼 생성
        foreach (Character ally in spawnedAllies)
        {
            GameObject skillButtonObject = Instantiate(skillButtonPrefab, skillButtonParent);

            // 스킬 버튼 이미지 설정
            Image skillButtonImage = skillButtonObject.transform.GetComponentInChildren<Button>().GetComponent<Image>();                
            skillButtonImage.sprite = Resources.Load<Sprite>($"Images_CharacterHead/{ally.imageName}");

            // 슬라이더 설정
            Slider cooldownSlider = skillButtonObject.GetComponentInChildren<Slider>();
            cooldownSlider.value = 1; // 초기 쿨타임 값

            Button skillButton = skillButtonObject.GetComponentInChildren<Button>();

            // 장착된 장비의 RLD 값 디버깅
            //foreach (string gearName in ally.eqiuppedGears)
            //{
            //    Gear gear = GearDataLoader.GetGearByName(gearName);
            //    if (gear != null)
            //    {
            //        Debug.Log($"[InitializeSkillButtons] {ally.name}의 장비 {gearName}의 RLD 값: {gear.stats.RLD}");
            //    }
            //}

            // 함종에 따라 스킬 설정
            if (ally.shipType == "DD")
            {
                skillButton.onClick.AddListener(() => UseDDSkill(ally, skillButton, cooldownSlider, skillButtonImage));
            }
            else if (ally.shipType == "CLCA")
            {
                skillButton.onClick.AddListener(() => UseCLCASkill(ally, skillButton, cooldownSlider, skillButtonImage));
            }
            else if (ally.shipType == "BB")
            {
                skillButton.onClick.AddListener(() => UseBBSkill(ally, skillButton, cooldownSlider, skillButtonImage));
            }
        }
    }

    // 스킬 쿨타임 코루틴
    private IEnumerator SkillCooldown(Button skillButton, Slider cooldownSlider, Image skillButtonImage, float cooldownTime)
    {
        Debug.Log("SkillCooldown 시작"); // 디버그 로그로 코루틴 시작 확인

        // 스킬 버튼을 비활성화
        skillButton.interactable = false;
        //skillButtonImage.color = new Color(0.5f, 0.5f, 0.5f, 1.0f); // 버튼 이미지 어둡게

        // 쿨타임 동안 슬라이더 값을 증가시킴
        float elapsedTime = 0;
        cooldownSlider.value = 0;

        while (elapsedTime < cooldownTime)
        {
            elapsedTime += Time.deltaTime;
            cooldownSlider.value = Mathf.Clamp01(elapsedTime / cooldownTime); // 슬라이더 값 갱신
            yield return null;
        }

        // 스킬 버튼을 다시 활성화
        skillButton.interactable = true;
        skillButtonImage.color = new Color(1, 1, 1, 1); // 버튼 이미지 밝게
        cooldownSlider.value = 1; // 슬라이더 값 최대치로 설정

        Debug.Log("SkillCooldown 완료"); // 디버그 로그로 코루틴 완료 확인
    }

    // DD 스킬: 어뢰 3발 발사
    private void UseDDSkill(Character ally, Button skillButton, Slider cooldownSlider, Image skillButtonImage)
    {
        // Torpedo 장비의 DMG 값 가져오기
        Gear torpedoGear = null;
        foreach (string gearName in ally.eqiuppedGears)
        {
            Gear gear = GearDataLoader.GetGearByName(gearName);
            if (gear != null && gear.gearType == "Torpedo")
            {
                torpedoGear = gear;
                break;
            }
        }

        if (torpedoGear != null && cooldownSlider.value >= 1.0f) // 쿨타임이 다 찼을 때만 스킬 사용 가능
        {
            float torpedoDamage = torpedoGear.stats.DMG;
            float reloadTime = torpedoGear.stats.RLD;

            // 어뢰 발사 로직 구현
            Debug.Log($"{ally.name}이(가) 어뢰 3발을 발사합니다! DMG: {torpedoDamage}");

            // 스킬 쿨타임 설정
            Debug.Log("Starting SkillCooldown Coroutine"); // 코루틴 시작 전 로그 추가
            StartCoroutine(SkillCooldown(skillButton, cooldownSlider, skillButtonImage, reloadTime));
        }
    }

    // CLCA 스킬: 재장전 속도 증가
    private void UseCLCASkill(Character ally, Button skillButton, Slider cooldownSlider, Image skillButtonImage)
    {
        if (cooldownSlider.value >= 1.0f) // 쿨타임이 다 찼을 때만 스킬 사용 가능
        {
            if (ally.eqiuppedGears.Count > 0)
            {
                Gear firstGear = GearDataLoader.GetGearByName(ally.eqiuppedGears[0]);
                if (firstGear != null)
                {
                    float originalReloadTime = firstGear.stats.RLD;
                    float reducedReloadTime = originalReloadTime / 2;
                    Debug.Log($"{ally.name}이(가) 10초 동안 재장전 속도가 증가합니다! RLD: {reducedReloadTime}");
                    // 재장전 시간 감소 로직 구현

                    StartCoroutine(SkillCooldown(skillButton, cooldownSlider, skillButtonImage, 10.0f)); // 10초 동안 스킬 쿨타임
                }
            }
        }
    }

    // BB 스킬: 체력 회복
    private void UseBBSkill(Character ally, Button skillButton, Slider cooldownSlider, Image skillButtonImage)
    {
        if (cooldownSlider.value >= 1.0f) // 쿨타임이 다 찼을 때만 스킬 사용 가능
        {
            float healAmount = ally.stats.HP * 0.3f;
            Debug.Log($"{ally.name}이(가) 체력을 회복합니다! 회복량: {healAmount}");
            // 체력 회복 로직 구현

            StartCoroutine(SkillCooldown(skillButton, cooldownSlider, skillButtonImage, 15.0f)); // 15초 동안 스킬 쿨타임
        }
    }

    
}
