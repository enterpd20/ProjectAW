using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    public class SkillData
    {
        public Character ally;
        public BattleAI battleAI;
        public Button skillButton;
        public Slider cooldownSlider;
        public Image skillButtonImage;        
        public float originalSPD;
    }

    public GameObject skillButtonPrefab; // 스킬 버튼 프리팹
    public Transform skillButtonParent;  // 스킬 버튼들을 담을 부모 오브젝트

    private List<Character> spawnedAllies;
    private List<SkillData> skillDataList = new List<SkillData>();

    private void Start()
    {
        CharacterSpawner.SpawnComplete += InitializeSkillButtons;
    }

    private void InitializeSkillButtons()
    {
        spawnedAllies = FindObjectOfType<CharacterSpawner>().spawnedAllies;

        CharacterSpawner characterSpawner = FindObjectOfType<CharacterSpawner>();
        spawnedAllies = characterSpawner.spawnedAllies;
        List<GameObject> spawnedAllyObjects = characterSpawner.spawnedAllyObjects; // 스폰된 캐릭터 오브젝트 리스트 가져오기

        // 스폰된 아군 캐릭터의 수에 맞게 스킬 버튼 생성
        for (int i = 0; i < spawnedAllies.Count; i++)
        {
            Character ally = spawnedAllies[i];
            GameObject allyObject = spawnedAllyObjects[i]; // 캐릭터 오브젝트 가져오기
            BattleAI battleAI = allyObject.GetComponent<BattleAI>();

            GameObject skillButtonObject = Instantiate(skillButtonPrefab, skillButtonParent);

            // 스킬 버튼 이미지 설정
            Image skillButtonImage = skillButtonObject.transform.GetComponentInChildren<Button>().GetComponent<Image>();                
            skillButtonImage.sprite = Resources.Load<Sprite>($"Images_CharacterHead/{ally.imageName}");

            // 슬라이더 설정
            Slider cooldownSlider = skillButtonObject.GetComponentInChildren<Slider>();
            cooldownSlider.value = 1; // 초기 쿨타임 값
            cooldownSlider.interactable = false; // 슬라이더 조작을 방지하기 위해 비활성화

            Button skillButton = skillButtonObject.GetComponentInChildren<Button>();

            // SkillData 객체를 한 번만 생성하여 리스트에 추가
            SkillData skillData = new SkillData
            {
                ally = ally,
                battleAI = battleAI,
                skillButton = skillButton,
                cooldownSlider = cooldownSlider,
                skillButtonImage = skillButtonImage,
                originalSPD = ally.stats.SPD
            };
            skillDataList.Add(skillData);

            // 함종에 따라 스킬 설정
            if (ally.shipType == "DD")
            {
                skillButton.onClick.AddListener(() => UseDDSkill(skillData));
            }
            else if (ally.shipType == "CLCA")
            {
                skillButton.onClick.AddListener(() => UseCLCASkill(skillData));
            }
            else if (ally.shipType == "BB")
            {
                skillButton.onClick.AddListener(() => UseBBSkill(skillData));
            }
        }
    }

    // 스킬 쿨타임 코루틴
    private IEnumerator SkillCooldown(SkillData skillData, float cooldownTime)
    {
        Debug.Log("SkillCooldown 시작"); // 디버그 로그로 코루틴 시작 확인

        // 스킬 버튼을 비활성화
        skillData.skillButton.interactable = false;

        // 쿨타임 동안 슬라이더 값을 증가시킴
        float elapsedTime = 0;
        skillData.cooldownSlider.value = 0;

        while (elapsedTime < cooldownTime)
        {
            elapsedTime += Time.deltaTime;
            skillData.cooldownSlider.value = Mathf.Clamp01(elapsedTime / cooldownTime); // 슬라이더 값 갱신
            yield return null;
        }

        // 스킬 효과 종료 (함종이 DD일 경우에만 적용)
        if (skillData.ally.shipType == "DD")
        {
            skillData.ally.stats.SPD = skillData.originalSPD;
            skillData.battleAI.DamageIgnore(0);
        }

        // 스킬 버튼을 다시 활성화
        skillData.skillButton.interactable = true;
        skillData.skillButtonImage.color = new Color(1, 1, 1, 1); // 버튼 이미지 밝게
        skillData.cooldownSlider.value = 1; // 슬라이더 값 최대치로 설정

        Debug.Log("SkillCooldown 완료"); // 디버그 로그로 코루틴 완료 확인
    }

    // DD 스킬: 이동속도 50% 증가, 회피율 30%
    private void UseDDSkill(SkillData skillData)
    {
        // 이미 스킬이 활성화 중인지 확인
        if (skillData.cooldownSlider.value < 1.0f) return;

        // 스킬 효과 적용: SPD 30% 증가, 회피율 30% 증가
        float originalSPD = skillData.ally.stats.SPD;
        skillData.ally.stats.SPD *= 1.5f;
        skillData.battleAI.DamageIgnore(30);

        Debug.Log($"{skillData.ally.name}이(가) 10초 동안 이동속도가 증가하고, 30% 확률로 대미지를 무시합니다!");

        // 10초 후 스킬 효과 종료 및 원래 속도와 회피율 복구
        StartCoroutine(ResetDDSkill(skillData, originalSPD, 10f));

        // 10초 동안 유지될 효과를 위한 타이머 설정
        StartCoroutine(SkillCooldown(skillData, 10f));
    }

    // 10초 후 속도와 회피율 복구 코루틴
    private IEnumerator ResetDDSkill(SkillData skillData, float originalSPD, float duration)
    {
        yield return new WaitForSeconds(duration);

        // 원래 속도와 회피율로 복구
        skillData.ally.stats.SPD = originalSPD;
        skillData.battleAI.DamageIgnore(0);

        Debug.Log($"{skillData.ally.name}의 이동속도와 회피율이 원래 값으로 복구되었습니다.");
    }

    // CLCA 스킬: 재장전 속도 증가
    private void UseCLCASkill(SkillData skillData)
    {
        if (skillData.cooldownSlider.value >= 1.0f) // 쿨타임이 다 찼을 때만 스킬 사용 가능
        {
            if (skillData.ally.eqiuppedGears.Count > 0)
            {
                Gear firstGear = GearDataLoader.GetGearByName(skillData.ally.eqiuppedGears[0]);
                if (firstGear != null)
                {
                    float originalReloadTime = firstGear.stats.RLD;
                    float reducedReloadTime = originalReloadTime * 0.5f;
                    Debug.Log($"{skillData.ally.name}이(가) 10초 동안 재장전 속도가 증가합니다! RLD: {reducedReloadTime}");

                    // 장비의 재장전 속도 감소
                    firstGear.stats.RLD = reducedReloadTime;

                    // 10초 동안 재장전 시간 증가 후, 다시 원래 값으로 복구
                    StartCoroutine(ResetCLCASkill(skillData, firstGear, originalReloadTime, 10f));

                    StartCoroutine(SkillCooldown(skillData, 10.0f)); // 10초 동안 스킬 쿨타임
                }
            }
        }
    }

    // 재장전 속도 복구 코루틴
    private IEnumerator ResetCLCASkill(SkillData skillData, Gear gear, float originalReloadTime, float duration)
    {
        yield return new WaitForSeconds(duration);

        // 재장전 시간 원래 값으로 복구
        gear.stats.RLD = originalReloadTime;
        Debug.Log($"{skillData.ally.name}의 재장전 속도가 원래 값으로 복구되었습니다.");
    }

    // BB 스킬: 체력 회복
    private void UseBBSkill(SkillData skillData)
    {
        if (skillData.cooldownSlider.value >= 1.0f) // 쿨타임이 다 찼을 때만 스킬 사용 가능
        {
            // 회복량은 캐릭터 최대 체력의 50%
            float healAmount = skillData.ally.stats.HP * 0.5f;

            // BattleAI 스크립트의 currentHealth에 회복량 적용
            skillData.battleAI.currentHealth += healAmount;

            // 현재 체력이 음수일 경우 0으로 설정 (체력 깎이는 현상 방지)
            skillData.battleAI.currentHealth = Mathf.Max(skillData.battleAI.currentHealth, 0);

            // 최대 체력을 넘지 않도록 현재 체력 값 제한
            skillData.battleAI.currentHealth = Mathf.Min(skillData.battleAI.currentHealth, skillData.ally.stats.HP);

            Debug.Log($"{skillData.ally.name}이(가) 체력을 회복합니다! 회복량: {healAmount}, 회복 후 체력: {skillData.battleAI.currentHealth}");

            StartCoroutine(SkillCooldown(skillData, 15.0f)); // 15초 동안 스킬 쿨타임
        }
    }

    public void DisableSkullButtons()
    {
        foreach (SkillData skillData in skillDataList)
        {
            gameObject.SetActive(false);
            skillData.cooldownSlider.gameObject.SetActive(false);

        }
    }
}
