using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    public Image characterImage;  // ĳ���� �̹���
    public Slider skillCooldownSlider;  // ��ų ��Ÿ�� ǥ�ø� ���� �����̴�
    public Button skillButton;  // ��ų ��ư
    public Character character;  // ��ų�� ����� ĳ���� ����
    private float cooldownTime;
    private float currentCooldown;
    private bool isCooldown = false;

    // ��ų ��ư�� �����ϰ� �ʱ�ȭ�ϴ� �޼���
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

        // ĳ������ ������ ���� ��ų ��Ÿ�� �� ȿ�� ����
        switch (character.shipType)
        {
            case "DD":
                cooldownTime = 10f; // ���÷� ��Ÿ���� 10�ʷ� ����, �� ĳ���Ͱ� ������ Torpedo ����� ������ �ð��� �޾ƿ�
                skillButton.onClick.AddListener(UseTorpedoSkill);
                break;
            case "CLCA":
                cooldownTime = 15f; // ���÷� ��Ÿ���� 15�ʷ� ����
                skillButton.onClick.AddListener(UseReloadBoostSkill);
                break;
            case "BB":
                cooldownTime = 20f; // ���÷� ��Ÿ���� 20�ʷ� ����
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

    // DD ��ų: ��� �߻�
    private void UseTorpedoSkill()
    {
        if (isCooldown) return;

        // ��ų ȿ�� ����: ��� �߻�
        Debug.Log($"{character.name} uses Torpedo Skill!");

        // ��ų ��Ÿ�� ����
        StartCooldown();
    }

    // CLCA ��ų: ������ �ӵ� ����
    private void UseReloadBoostSkill()
    {
        if (isCooldown) return;

        // ��ų ȿ�� ����: ������ �ӵ� ����
        Debug.Log($"{character.name} uses Reload Boost Skill!");

        // ��ų ��Ÿ�� ����
        StartCooldown();
    }

    // BB ��ų: ü�� ȸ��
    private void UseHealSkill()
    {
        if (isCooldown) return;

        // ��ų ȿ�� ����: ü�� ȸ��
        Debug.Log($"{character.name} uses Heal Skill!");

        // ��ų ��Ÿ�� ����
        StartCooldown();
    }

    // ��ų ��Ÿ�� ����
    private void StartCooldown()
    {
        isCooldown = true;
        currentCooldown = cooldownTime;
        skillButton.interactable = false;
        skillCooldownSlider.gameObject.SetActive(true);
    }
}
