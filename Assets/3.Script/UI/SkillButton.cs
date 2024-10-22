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

    public GameObject skillButtonPrefab; // ��ų ��ư ������
    public Transform skillButtonParent;  // ��ų ��ư���� ���� �θ� ������Ʈ

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
        List<GameObject> spawnedAllyObjects = characterSpawner.spawnedAllyObjects; // ������ ĳ���� ������Ʈ ����Ʈ ��������

        // ������ �Ʊ� ĳ������ ���� �°� ��ų ��ư ����
        for (int i = 0; i < spawnedAllies.Count; i++)
        {
            Character ally = spawnedAllies[i];
            GameObject allyObject = spawnedAllyObjects[i]; // ĳ���� ������Ʈ ��������
            BattleAI battleAI = allyObject.GetComponent<BattleAI>();

            GameObject skillButtonObject = Instantiate(skillButtonPrefab, skillButtonParent);

            // ��ų ��ư �̹��� ����
            Image skillButtonImage = skillButtonObject.transform.GetComponentInChildren<Button>().GetComponent<Image>();                
            skillButtonImage.sprite = Resources.Load<Sprite>($"Images_CharacterHead/{ally.imageName}");

            // �����̴� ����
            Slider cooldownSlider = skillButtonObject.GetComponentInChildren<Slider>();
            cooldownSlider.value = 1; // �ʱ� ��Ÿ�� ��
            cooldownSlider.interactable = false; // �����̴� ������ �����ϱ� ���� ��Ȱ��ȭ

            Button skillButton = skillButtonObject.GetComponentInChildren<Button>();

            // SkillData ��ü�� �� ���� �����Ͽ� ����Ʈ�� �߰�
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

            // ������ ���� ��ų ����
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

    // ��ų ��Ÿ�� �ڷ�ƾ
    private IEnumerator SkillCooldown(SkillData skillData, float cooldownTime)
    {
        Debug.Log("SkillCooldown ����"); // ����� �α׷� �ڷ�ƾ ���� Ȯ��

        // ��ų ��ư�� ��Ȱ��ȭ
        skillData.skillButton.interactable = false;

        // ��Ÿ�� ���� �����̴� ���� ������Ŵ
        float elapsedTime = 0;
        skillData.cooldownSlider.value = 0;

        while (elapsedTime < cooldownTime)
        {
            elapsedTime += Time.deltaTime;
            skillData.cooldownSlider.value = Mathf.Clamp01(elapsedTime / cooldownTime); // �����̴� �� ����
            yield return null;
        }

        // ��ų ȿ�� ���� (������ DD�� ��쿡�� ����)
        if (skillData.ally.shipType == "DD")
        {
            skillData.ally.stats.SPD = skillData.originalSPD;
            skillData.battleAI.DamageIgnore(0);
        }

        // ��ų ��ư�� �ٽ� Ȱ��ȭ
        skillData.skillButton.interactable = true;
        skillData.skillButtonImage.color = new Color(1, 1, 1, 1); // ��ư �̹��� ���
        skillData.cooldownSlider.value = 1; // �����̴� �� �ִ�ġ�� ����

        Debug.Log("SkillCooldown �Ϸ�"); // ����� �α׷� �ڷ�ƾ �Ϸ� Ȯ��
    }

    // DD ��ų: �̵��ӵ� 50% ����, ȸ���� 30%
    private void UseDDSkill(SkillData skillData)
    {
        // �̹� ��ų�� Ȱ��ȭ ������ Ȯ��
        if (skillData.cooldownSlider.value < 1.0f) return;

        // ��ų ȿ�� ����: SPD 30% ����, ȸ���� 30% ����
        float originalSPD = skillData.ally.stats.SPD;
        skillData.ally.stats.SPD *= 1.5f;
        skillData.battleAI.DamageIgnore(30);

        Debug.Log($"{skillData.ally.name}��(��) 10�� ���� �̵��ӵ��� �����ϰ�, 30% Ȯ���� ������� �����մϴ�!");

        // 10�� �� ��ų ȿ�� ���� �� ���� �ӵ��� ȸ���� ����
        StartCoroutine(ResetDDSkill(skillData, originalSPD, 10f));

        // 10�� ���� ������ ȿ���� ���� Ÿ�̸� ����
        StartCoroutine(SkillCooldown(skillData, 10f));
    }

    // 10�� �� �ӵ��� ȸ���� ���� �ڷ�ƾ
    private IEnumerator ResetDDSkill(SkillData skillData, float originalSPD, float duration)
    {
        yield return new WaitForSeconds(duration);

        // ���� �ӵ��� ȸ������ ����
        skillData.ally.stats.SPD = originalSPD;
        skillData.battleAI.DamageIgnore(0);

        Debug.Log($"{skillData.ally.name}�� �̵��ӵ��� ȸ������ ���� ������ �����Ǿ����ϴ�.");
    }

    // CLCA ��ų: ������ �ӵ� ����
    private void UseCLCASkill(SkillData skillData)
    {
        if (skillData.cooldownSlider.value >= 1.0f) // ��Ÿ���� �� á�� ���� ��ų ��� ����
        {
            if (skillData.ally.eqiuppedGears.Count > 0)
            {
                Gear firstGear = GearDataLoader.GetGearByName(skillData.ally.eqiuppedGears[0]);
                if (firstGear != null)
                {
                    float originalReloadTime = firstGear.stats.RLD;
                    float reducedReloadTime = originalReloadTime * 0.5f;
                    Debug.Log($"{skillData.ally.name}��(��) 10�� ���� ������ �ӵ��� �����մϴ�! RLD: {reducedReloadTime}");

                    // ����� ������ �ӵ� ����
                    firstGear.stats.RLD = reducedReloadTime;

                    // 10�� ���� ������ �ð� ���� ��, �ٽ� ���� ������ ����
                    StartCoroutine(ResetCLCASkill(skillData, firstGear, originalReloadTime, 10f));

                    StartCoroutine(SkillCooldown(skillData, 10.0f)); // 10�� ���� ��ų ��Ÿ��
                }
            }
        }
    }

    // ������ �ӵ� ���� �ڷ�ƾ
    private IEnumerator ResetCLCASkill(SkillData skillData, Gear gear, float originalReloadTime, float duration)
    {
        yield return new WaitForSeconds(duration);

        // ������ �ð� ���� ������ ����
        gear.stats.RLD = originalReloadTime;
        Debug.Log($"{skillData.ally.name}�� ������ �ӵ��� ���� ������ �����Ǿ����ϴ�.");
    }

    // BB ��ų: ü�� ȸ��
    private void UseBBSkill(SkillData skillData)
    {
        if (skillData.cooldownSlider.value >= 1.0f) // ��Ÿ���� �� á�� ���� ��ų ��� ����
        {
            // ȸ������ ĳ���� �ִ� ü���� 50%
            float healAmount = skillData.ally.stats.HP * 0.5f;

            // BattleAI ��ũ��Ʈ�� currentHealth�� ȸ���� ����
            skillData.battleAI.currentHealth += healAmount;

            // ���� ü���� ������ ��� 0���� ���� (ü�� ���̴� ���� ����)
            skillData.battleAI.currentHealth = Mathf.Max(skillData.battleAI.currentHealth, 0);

            // �ִ� ü���� ���� �ʵ��� ���� ü�� �� ����
            skillData.battleAI.currentHealth = Mathf.Min(skillData.battleAI.currentHealth, skillData.ally.stats.HP);

            Debug.Log($"{skillData.ally.name}��(��) ü���� ȸ���մϴ�! ȸ����: {healAmount}, ȸ�� �� ü��: {skillData.battleAI.currentHealth}");

            StartCoroutine(SkillCooldown(skillData, 15.0f)); // 15�� ���� ��ų ��Ÿ��
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
