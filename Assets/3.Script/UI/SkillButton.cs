using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    public GameObject skillButtonPrefab; // ��ų ��ư ������
    public Transform skillButtonParent;  // ��ų ��ư���� ���� �θ� ������Ʈ

    private List<Character> spawnedAllies;

    private void Start()
    {
        CharacterSpawner.SpawnComplete += InitializeSkillButtons;
    }

    private void InitializeSkillButtons()
    {
        spawnedAllies = FindObjectOfType<CharacterSpawner>().spawnedAllies;

        // ������ �Ʊ� ĳ������ ���� �°� ��ų ��ư ����
        foreach (Character ally in spawnedAllies)
        {
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

            // ������ ���� ��ų ����
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

    // ��ų ��Ÿ�� �ڷ�ƾ
    private IEnumerator SkillCooldown(Button skillButton, Slider cooldownSlider, Image skillButtonImage, float cooldownTime)
    {
        Debug.Log("SkillCooldown ����"); // ����� �α׷� �ڷ�ƾ ���� Ȯ��

        // ��ų ��ư�� ��Ȱ��ȭ
        skillButton.interactable = false;
        //skillButtonImage.color = new Color(0.5f, 0.5f, 0.5f, 1.0f); // ��ư �̹��� ��Ӱ�

        // ��Ÿ�� ���� �����̴� ���� ������Ŵ
        float elapsedTime = 0;
        cooldownSlider.value = 0;

        while (elapsedTime < cooldownTime)
        {
            elapsedTime += Time.deltaTime;
            cooldownSlider.value = Mathf.Clamp01(elapsedTime / cooldownTime); // �����̴� �� ����
            yield return null;
        }

        // ��ų ��ư�� �ٽ� Ȱ��ȭ
        skillButton.interactable = true;
        skillButtonImage.color = new Color(1, 1, 1, 1); // ��ư �̹��� ���
        cooldownSlider.value = 1; // �����̴� �� �ִ�ġ�� ����

        Debug.Log("SkillCooldown �Ϸ�"); // ����� �α׷� �ڷ�ƾ �Ϸ� Ȯ��
    }

    // DD ��ų: ��� 3�� �߻�
    private void UseDDSkill(Character ally, Button skillButton, Slider cooldownSlider, Image skillButtonImage)
    {
        // Torpedo ����� DMG �� ��������
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

        if (torpedoGear != null && cooldownSlider.value >= 1.0f) // ��Ÿ���� �� á�� ���� ��ų ��� ����
        {
            float torpedoDamage = torpedoGear.stats.DMG;
            float reloadTime = torpedoGear.stats.RLD;

            // ��� �߻� ���� ����
            Debug.Log($"{ally.name}��(��) ��� 3���� �߻��մϴ�! DMG: {torpedoDamage}");

            // ��ų ��Ÿ�� ����
            Debug.Log("Starting SkillCooldown Coroutine"); // �ڷ�ƾ ���� �� �α� �߰�
            StartCoroutine(SkillCooldown(skillButton, cooldownSlider, skillButtonImage, reloadTime));
        }
    }

    // CLCA ��ų: ������ �ӵ� ����
    private void UseCLCASkill(Character ally, Button skillButton, Slider cooldownSlider, Image skillButtonImage)
    {
        if (cooldownSlider.value >= 1.0f) // ��Ÿ���� �� á�� ���� ��ų ��� ����
        {
            if (ally.eqiuppedGears.Count > 0)
            {
                Gear firstGear = GearDataLoader.GetGearByName(ally.eqiuppedGears[0]);
                if (firstGear != null)
                {
                    float originalReloadTime = firstGear.stats.RLD;
                    float reducedReloadTime = originalReloadTime / 2;
                    Debug.Log($"{ally.name}��(��) 10�� ���� ������ �ӵ��� �����մϴ�! RLD: {reducedReloadTime}");
                    // ������ �ð� ���� ���� ����

                    StartCoroutine(SkillCooldown(skillButton, cooldownSlider, skillButtonImage, 10.0f)); // 10�� ���� ��ų ��Ÿ��
                }
            }
        }
    }

    // BB ��ų: ü�� ȸ��
    private void UseBBSkill(Character ally, Button skillButton, Slider cooldownSlider, Image skillButtonImage)
    {
        if (cooldownSlider.value >= 1.0f) // ��Ÿ���� �� á�� ���� ��ų ��� ����
        {
            float healAmount = ally.stats.HP * 0.3f;
            Debug.Log($"{ally.name}��(��) ü���� ȸ���մϴ�! ȸ����: {healAmount}");
            // ü�� ȸ�� ���� ����

            StartCoroutine(SkillCooldown(skillButton, cooldownSlider, skillButtonImage, 15.0f)); // 15�� ���� ��ų ��Ÿ��
        }
    }

    
}
