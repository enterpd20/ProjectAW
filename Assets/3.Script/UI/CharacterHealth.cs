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
            healthSlider.interactable = false; // 체력바 드래그 비활성화

            //Debug.Log($"{gameObject.name} health: {battleAI.currentHealth} / {battleAI.maxHealth}, Slider value: {healthSlider.value}");
        }
        else
        {
            Debug.LogWarning("BattleAI 스크립트를 찾을 수 없습니다.");
        }
    }

    private void Update()
    {
        if (battleAI != null)
        {
            // 현재 체력을 Slider에 반영
            healthSlider.value = battleAI.currentHealth;

            //Debug.Log($"{gameObject.name} - Current Health: {battleAI.currentHealth}, Slider Value: {healthSlider.value}");
        }
    }
}
