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

            Button skillButton = skillButtonObject.GetComponentInChildren<Button>();

            // ������ ����� RLD �� �����
            //foreach (string gearName in ally.eqiuppedGears)
            //{
            //    Gear gear = GearDataLoader.GetGearByName(gearName);
            //    if (gear != null)
            //    {
            //        Debug.Log($"[InitializeSkillButtons] {ally.name}�� ��� {gearName}�� RLD ��: {gear.stats.RLD}");
            //    }
            //}

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
        //skillButtonImage.color = new Color(0.5f, 0.5f, 0.5f, 1.0f); // ��ư �̹��� ��Ӱ�

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

    // DD ��ų: ��� 3�� �߻�
    private void UseDDSkill(SkillData skillData)
    {
        // Torpedo ����� DMG �� ��������
        //Gear torpedoGear = null;
        //foreach (string gearName in ally.eqiuppedGears)
        //{
        //    Gear gear = GearDataLoader.GetGearByName(gearName);
        //    if (gear != null && gear.gearType == "Torpedo")
        //    {
        //        torpedoGear = gear;
        //        break;
        //    }
        //}

        //if (torpedoGear != null && cooldownSlider.value >= 1.0f) // ��Ÿ���� �� á�� ���� ��ų ��� ����
        //{
        //    float torpedoDamage = torpedoGear.stats.DMG;
        //    float reloadTime = torpedoGear.stats.RLD;
        //
        //    // ��� �߻� ���� ����
        //    Debug.Log($"{ally.name}��(��) ��� 3���� �߻��մϴ�! DMG: {torpedoDamage}");
        //
        //    // ��ų ��Ÿ�� ����
        //    Debug.Log("Starting SkillCooldown Coroutine"); // �ڷ�ƾ ���� �� �α� �߰�
        //    StartCoroutine(SkillCooldown(skillButton, cooldownSlider, skillButtonImage, reloadTime));
        //}

        // �̹� ��ų�� Ȱ��ȭ ������ Ȯ��
        if (skillData.cooldownSlider.value < 1.0f) return;

        // ĳ������ BattleAI ��ũ��Ʈ�� �̸����� ��������
        //GameObject allyObject = GameObject.Find(ally.name);
        //if (allyObject == null)
        //{
        //    Debug.LogWarning($"No GameObject found with name: {ally.name}");
        //    return;
        //}
        //
        //BattleAI battleAI = allyObject.GetComponent<BattleAI>();
        //if (battleAI == null)
        //{
        //    Debug.LogWarning($"BattleAI component not found on {ally.name}");
        //    return;
        //}
        //
        //// ��ų ȿ�� ����: SPD 10% ����
        //float originalSPD = ally.stats.SPD;
        //ally.stats.SPD *= 1.1f;
        //battleAI.DamageIgnore(30);
        //
        //// 10�� ���� ������ ȿ���� ���� Ÿ�̸� ����
        //float skillDuration = 10f;
        //StartCoroutine(SkillCooldown(ally, battleAI, skillButton, cooldownSlider, skillButtonImage, skillDuration, originalSPD));

        // ��ų ȿ�� ����: SPD 10% ����
        skillData.ally.stats.SPD *= 1.1f;
        skillData.battleAI.DamageIgnore(30);

        // 10�� ���� ������ ȿ���� ���� Ÿ�̸� ����
        float skillDuration = 10f;
        StartCoroutine(SkillCooldown(skillData, skillDuration));
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
                    float reducedReloadTime = originalReloadTime / 2;
                    Debug.Log($"{skillData.ally.name}��(��) 10�� ���� ������ �ӵ��� �����մϴ�! RLD: {reducedReloadTime}");
                    // ������ �ð� ���� ���� ����

                    StartCoroutine(SkillCooldown(skillData, 10.0f)); // 10�� ���� ��ų ��Ÿ��
                }
            }
        }
    }

    // BB ��ų: ü�� ȸ��
    private void UseBBSkill(SkillData skillData)
    {
        if (skillData.cooldownSlider.value >= 1.0f) // ��Ÿ���� �� á�� ���� ��ų ��� ����
        {
            float healAmount = skillData.ally.stats.HP * 0.3f;
            Debug.Log($"{skillData.ally.name}��(��) ü���� ȸ���մϴ�! ȸ����: {healAmount}");
            // ü�� ȸ�� ���� ����

            StartCoroutine(SkillCooldown(skillData, 15.0f)); // 15�� ���� ��ų ��Ÿ��
        }
    }

    
}
