using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHealth : MonoBehaviour
{
    public Slider healthSlider;
    private BattleAI battleAI;

    private void Start()
    {
        battleAI = GetComponentInParent<BattleAI>();

        if(battleAI != null)
        {
            healthSlider.maxValue = battleAI.maxHealth;
            healthSlider.value = battleAI.currentHealth;
            healthSlider.interactable = false; // ü�¹� �巡�� ��Ȱ��ȭ

            //Debug.Log($"{gameObject.name} health: {battleAI.currentHealth} / {battleAI.maxHealth}, Slider value: {healthSlider.value}");
        }
        else
        {
            Debug.LogWarning("BattleAI ��ũ��Ʈ�� ã�� �� �����ϴ�.");
        }
    }

    private void Update()
    {
        if (battleAI != null)
        {
            // ���� ü���� Slider�� �ݿ�
            healthSlider.value = battleAI.currentHealth;

            //Debug.Log($"{gameObject.name} - Current Health: {battleAI.currentHealth}, Slider Value: {healthSlider.value}");
        }
    }
}
