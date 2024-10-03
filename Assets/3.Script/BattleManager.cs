using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public float battleTimeLimit = 180f; // 전투 시간 제한 (3분)
    private float battleTimer;

    private List<BattleAI> allyCharacters;
    private List<BattleAI> enemyCharacters;

    private bool isBattleOver = false;

    // 전투 결과 UI
    public GameObject resultPanel;  // 결과를 표시할 패널
    public Text resultText;         // 결과 텍스트

    void Start()
    {
        InitializeBattle();

        resultPanel.transform.localScale = Vector3.zero;
    }

    void Update()
    {
        if (isBattleOver) return;

        // 타이머 감소
        battleTimer -= Time.deltaTime;
        if (battleTimer <= 0)
        {
            CheckBattleResult();
            return;
        }

        // 승리/패배/무승부 조건 체크
        CheckBattleResult();
    }

    private void InitializeBattle()
    {
        // 전투 타이머 초기화
        battleTimer = battleTimeLimit;

        // 아군 및 적군 캐릭터를 리스트에 저장
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

        // 아군/적군의 살아있는 캐릭터 수 확인
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

        // 1. 아군 캐릭터가 모두 사망하고 적군이 살아있다면 패배
        if (aliveAllies == 0 && aliveEnemies > 0)
        {
            Debug.Log("Battle Over: Defeat");
            isBattleOver = true;
            ShowBattleResult("Defeat (Time's Up)", Color.red);
            // 추가적인 패배 처리 로직
        }
        // 2. 적군 캐릭터가 모두 사망하고 아군이 살아있다면 승리
        else if (aliveEnemies == 0 && aliveAllies > 0)
        {
            Debug.Log("Battle Over: Victory");
            isBattleOver = true;
            ShowBattleResult("Victory (Time's Up)", Color.yellow);
            // 추가적인 승리 처리 로직
        }
        // 3. 양측 모두 살아있는 캐릭터가 없거나 같은 경우 무승부
        else if ((aliveAllies == 0 && aliveEnemies == 0) || aliveAllies == aliveEnemies)
        {
            Debug.Log("Battle Over: Draw");
            isBattleOver = true;
            ShowBattleResult("Draw (Time's Up)", Color.gray);
            // 추가적인 무승부 처리 로직
        }
        // 4. 시간이 다 되었을 경우, 그 시점에 살아있는 캐릭터 수로 결과 결정
        else if (battleTimer <= 0)
        {
            if (aliveAllies > aliveEnemies)
            {
                Debug.Log("Battle Over: Victory (Time's Up)");
                isBattleOver = true;
                ShowBattleResult("Victory (Time's Up)", Color.yellow);
                // 추가적인 승리 처리 로직
            }
            else if (aliveEnemies > aliveAllies)
            {
                Debug.Log("Battle Over: Defeat (Time's Up)");
                isBattleOver = true;
                ShowBattleResult("Defeat (Time's Up)", Color.red);
                // 추가적인 패배 처리 로직
            }
            else
            {
                Debug.Log("Battle Over: Draw (Time's Up)");
                isBattleOver = true;
                ShowBattleResult("Draw (Time's Up)", Color.gray);
                // 추가적인 무승부 처리 로직
            }
        }
    }

    private void ShowBattleResult(string resultMessage, Color textColor)
    {
        // 1초 후에 결과 패널 표시
        StartCoroutine(DisplayResult(resultMessage, textColor));
    }

    private IEnumerator DisplayResult(string resultMessage, Color textColor)
    {
        yield return new WaitForSeconds(1f); // 1초 대기

        resultText.text = resultMessage;
        resultText.color = textColor;

        // 결과 패널의 크기를 설정하여 나타나게 함
        resultPanel.transform.localScale = Vector3.one;

        Time.timeScale = 0f;
    }
}
