using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    public Image characterImage;  // 캐릭터 이미지
    public Slider skillCooldownSlider;  // 스킬 쿨타임 표시를 위한 슬라이더
    public Button skillButton;  // 스킬 버튼
    public Character character;  // 스킬을 사용할 캐릭터 정보
    private float cooldownTime;
    private float currentCooldown;
    private bool isCooldown = false;

    // 스킬 버튼을 생성하고 초기화하는 메서드
    public static void CreateSkillButtons(List<Character> allyCharacters, Transform skillButtonGrid, GameObject skillButtonPrefab)
    {
        foreach (var ally in allyCharacters)
        {
            GameObject skillButtonObject = Instantiate(skillButtonPrefab, skillButtonGrid);
            SkillButton skillButtonScript = skillButtonObject.GetComponent<SkillButton>();
            skillButtonScript.Initialize(ally);
        }
    }

    public void Initialize(Character characterData)
    {
        character = characterData;
        characterImage.sprite = Resources.Load<Sprite>($"Images_Character/{character.imageName}");

        // 캐릭터의 함종에 따라 스킬 쿨타임 및 효과 설정
        switch (character.shipType)
        {
            case "DD":
                cooldownTime = 10f; // 예시로 쿨타임을 10초로 설정, 각 캐릭터가 장착한 Torpedo 장비의 재장전 시간을 받아옴
                skillButton.onClick.AddListener(UseTorpedoSkill);
                break;
            case "CLCA":
                cooldownTime = 15f; // 예시로 쿨타임을 15초로 설정
                skillButton.onClick.AddListener(UseReloadBoostSkill);
                break;
            case "BB":
                cooldownTime = 20f; // 예시로 쿨타임을 20초로 설정
                skillButton.onClick.AddListener(UseHealSkill);
                break;
        }
    }

    private void Update()
    {
        if (isCooldown)
        {
            currentCooldown -= Time.deltaTime;
            skillCooldownSlider.value = currentCooldown / cooldownTime;

            if (currentCooldown <= 0)
            {
                isCooldown = false;
                skillButton.interactable = true;
                skillCooldownSlider.gameObject.SetActive(false);
            }
        }
    }

    // DD 스킬: 어뢰 발사
    private void UseTorpedoSkill()
    {
        if (isCooldown) return;

        // 스킬 효과 구현: 어뢰 발사
        Debug.Log($"{character.name} uses Torpedo Skill!");

        // 스킬 쿨타임 설정
        StartCooldown();
    }

    // CLCA 스킬: 재장전 속도 증가
    private void UseReloadBoostSkill()
    {
        if (isCooldown) return;

        // 스킬 효과 구현: 재장전 속도 증가
        Debug.Log($"{character.name} uses Reload Boost Skill!");

        // 스킬 쿨타임 설정
        StartCooldown();
    }

    // BB 스킬: 체력 회복
    private void UseHealSkill()
    {
        if (isCooldown) return;

        // 스킬 효과 구현: 체력 회복
        Debug.Log($"{character.name} uses Heal Skill!");

        // 스킬 쿨타임 설정
        StartCooldown();
    }

    // 스킬 쿨타임 시작
    private void StartCooldown()
    {
        isCooldown = true;
        currentCooldown = cooldownTime;
        skillButton.interactable = false;
        skillCooldownSlider.gameObject.SetActive(true);
    }
}
