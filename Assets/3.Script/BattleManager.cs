using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public float battleTimeLimit = 180f; // ���� �ð� ���� (3��)
    private float battleTimer;

    private List<BattleAI> allyCharacters;
    private List<BattleAI> enemyCharacters;

    private bool isBattleOver = false;

    // ���� ��� UI
    public GameObject resultPanel;  // ����� ǥ���� �г�
    public Text resultText;         // ��� �ؽ�Ʈ

    void Start()
    {
        InitializeBattle();

        resultPanel.transform.localScale = Vector3.zero;
    }

    void Update()
    {
        if (isBattleOver) return;

        // Ÿ�̸� ����
        battleTimer -= Time.deltaTime;
        if (battleTimer <= 0)
        {
            CheckBattleResult();
            return;
        }

        // �¸�/�й�/���º� ���� üũ
        CheckBattleResult();
    }

    private void InitializeBattle()
    {
        // ���� Ÿ�̸� �ʱ�ȭ
        battleTimer = battleTimeLimit;

        // �Ʊ� �� ���� ĳ���͸� ����Ʈ�� ����
        allyCharacters = new List<BattleAI>();
        enemyCharacters = new List<BattleAI>();

        BattleAI[] allCharacters = FindObjectsOfType<BattleAI>();
        foreach (BattleAI character in allCharacters)
        {
            TeamManager teamManager = character.GetComponent<TeamManager>();
            if (teamManager != null)
            {
                if (teamManager.team == TeamManager.Team.Ally)
                {
                    allyCharacters.Add(character);
                }
                else if (teamManager.team == TeamManager.Team.Enemy)
                {
                    enemyCharacters.Add(character);
                }
            }
        }
    }

    private void CheckBattleResult()
    {
        int aliveAllies = 0;
        int aliveEnemies = 0;

        // �Ʊ�/������ ����ִ� ĳ���� �� Ȯ��
        foreach (BattleAI ally in allyCharacters)
        {
            if (ally.gameObject.activeSelf && ally.currentHealth > 0)
            {
                aliveAllies++;
            }
        }

        foreach (BattleAI enemy in enemyCharacters)
        {
            if (enemy.gameObject.activeSelf && enemy.currentHealth > 0)
            {
                aliveEnemies++;
            }
        }

        // 1. �Ʊ� ĳ���Ͱ� ��� ����ϰ� ������ ����ִٸ� �й�
        if (aliveAllies == 0 && aliveEnemies > 0)
        {
            Debug.Log("Battle Over: Defeat");
            isBattleOver = true;
            ShowBattleResult("Defeat (Time's Up)", Color.red);
            // �߰����� �й� ó�� ����
        }
        // 2. ���� ĳ���Ͱ� ��� ����ϰ� �Ʊ��� ����ִٸ� �¸�
        else if (aliveEnemies == 0 && aliveAllies > 0)
        {
            Debug.Log("Battle Over: Victory");
            isBattleOver = true;
            ShowBattleResult("Victory (Time's Up)", Color.yellow);
            // �߰����� �¸� ó�� ����
        }
        // 3. ���� ��� ����ִ� ĳ���Ͱ� ���ų� ���� ��� ���º�
        else if ((aliveAllies == 0 && aliveEnemies == 0) || aliveAllies == aliveEnemies)
        {
            Debug.Log("Battle Over: Draw");
            isBattleOver = true;
            ShowBattleResult("Draw (Time's Up)", Color.gray);
            // �߰����� ���º� ó�� ����
        }
        // 4. �ð��� �� �Ǿ��� ���, �� ������ ����ִ� ĳ���� ���� ��� ����
        else if (battleTimer <= 0)
        {
            if (aliveAllies > aliveEnemies)
            {
                Debug.Log("Battle Over: Victory (Time's Up)");
                isBattleOver = true;
                ShowBattleResult("Victory (Time's Up)", Color.yellow);
                // �߰����� �¸� ó�� ����
            }
            else if (aliveEnemies > aliveAllies)
            {
                Debug.Log("Battle Over: Defeat (Time's Up)");
                isBattleOver = true;
                ShowBattleResult("Defeat (Time's Up)", Color.red);
                // �߰����� �й� ó�� ����
            }
            else
            {
                Debug.Log("Battle Over: Draw (Time's Up)");
                isBattleOver = true;
                ShowBattleResult("Draw (Time's Up)", Color.gray);
                // �߰����� ���º� ó�� ����
            }
        }
    }

    private void ShowBattleResult(string resultMessage, Color textColor)
    {
        // 1�� �Ŀ� ��� �г� ǥ��
        StartCoroutine(DisplayResult(resultMessage, textColor));
    }

    private IEnumerator DisplayResult(string resultMessage, Color textColor)
    {
        yield return new WaitForSeconds(1f); // 1�� ���

        resultText.text = resultMessage;
        resultText.color = textColor;

        // ��� �г��� ũ�⸦ �����Ͽ� ��Ÿ���� ��
        resultPanel.transform.localScale = Vector3.one;

        Time.timeScale = 0f;
    }
}
