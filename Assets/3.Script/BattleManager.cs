using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public float battleTimeLimit = 180f; // ���� �ð� ���� (3��)
    private float battleTimer;
    private bool isBattleOver = false;

    private List<BattleAI> allyCharacters;
    private List<BattleAI> enemyCharacters;


    // ���� ��� UI
    public GameObject resultPanel;  // ����� ǥ���� �г�
    public Text resultText;         // ��� �ؽ�Ʈ

    // �Ͻ����� ���� UI
    public GameObject pausePanel;   // �Ͻ����� UI �г�
    public Button pauseButton;      // �Ͻ����� ��ư
    public Button resumeButton;     // �簳 ��ư
    private bool isPaused = false;  // ���� �Ͻ����� ���¸� ����

    // ī�޶� ������ ����
    public Camera mainCamera;       // ���� ī�޶�
    public float dragSpeed = 1f;    // ���콺 �巡�� �ӵ�
    private Vector3 dragOrigin;     // �巡�� ���� ��ġ

    void Start()
    {
        resultPanel.transform.localScale = Vector3.zero;

        pausePanel.transform.localScale = Vector3.zero;
        pauseButton.onClick.AddListener(PauseGame);
        resumeButton.onClick.AddListener(ResumeGame);

        // CharacterSpawner�� ���� �Ϸ� �̺�Ʈ�� ����
        CharacterSpawner.SpawnComplete += OnCharacterSpawnComplete;
        EnemySpawner.EnemySpawnComplete += OnEnemySpawnComplete;
    }

    private void OnCharacterSpawnComplete()
    {
        // ������ �Ϸ�Ǹ� ���� �ʱ�ȭ
        InitializeBattle();
    }

    private void OnEnemySpawnComplete()
    {
        InitializeBattle();
    }

    void OnEnable()
    {
        // ���� Ȱ��ȭ�� ������ Ÿ�̸� �ʱ�ȭ
        ResetBattleTimer();
    }

    void Update()
    {
        if (isBattleOver || isPaused) return;

        // Ÿ�̸� ����
        battleTimer -= Time.unscaledDeltaTime;
        if (battleTimer <= 0)
        {
            CheckBattleResult();
            return;
        }

        // �¸�/�й�/���º� ���� üũ
        CheckBattleResult();

        // ī�޶� ������
        HandleCameraMovement();
    }

    private void InitializeBattle()
    {
        ResetBattleTimer();

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

        // ����� �α� �߰�: ������ �Ʊ�/���� ĳ���� �� �� �̸� Ȯ��
        Debug.Log($"[InitializeBattle] ������ �Ʊ� ĳ���� ��: {allyCharacters.Count}");
        foreach (BattleAI ally in allyCharacters)
        {
            Debug.Log($"[InitializeBattle] �Ʊ� ĳ����: {ally.gameObject.name}");
        }

        Debug.Log($"[InitializeBattle] ������ ���� ĳ���� ��: {enemyCharacters.Count}");
        foreach (BattleAI enemy in enemyCharacters)
        {
            Debug.Log($"[InitializeBattle] ���� ĳ����: {enemy.gameObject.name}");
        }
    }

    private void ResetBattleTimer()
    {
        // ���� Ÿ�̸� �ʱ�ȭ
        battleTimer = battleTimeLimit;
        isBattleOver = false; // ���� ���� ���� �ʱ�ȭ
        Time.timeScale = 1f;  // ������ ���۵� �� Ÿ�� �������� 1�� ����
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

        // ����� �α�: ���� ����ִ� �Ʊ�/������ ���� ���
        Debug.Log($"[CheckBattleResult] ����ִ� �Ʊ� ��: {aliveAllies}, ����ִ� ���� ��: {aliveEnemies}");

        // 1. �Ʊ� ĳ���Ͱ� ��� ����ϰ� ������ ����ִٸ� �й�
        if (aliveAllies == 0 && aliveEnemies > 0)
        {
            Debug.Log("Battle Over: Defeat");
            isBattleOver = true;
            ShowBattleResult("Defeat", Color.red);
            // �߰����� �й� ó�� ����
        }
        // 2. ���� ĳ���Ͱ� ��� ����ϰ� �Ʊ��� ����ִٸ� �¸�
        else if (aliveEnemies == 0 && aliveAllies > 0)
        {
            Debug.Log("Battle Over: Victory");
            isBattleOver = true;
            ShowBattleResult("Victory", Color.yellow);
            // �߰����� �¸� ó�� ����
        }
        // 3. ���� ��� ����ִ� ĳ���Ͱ� ���ų� ���� ��� ���º�
        else if ((aliveAllies == 0 && aliveEnemies == 0) || aliveAllies == aliveEnemies)
        {
            Debug.Log("Battle Over: Draw");
            isBattleOver = true;
            ShowBattleResult("Draw", Color.gray);
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
        yield return new WaitForSecondsRealtime(1f); // 1�� ���

        resultText.text = resultMessage;
        resultText.color = textColor;

        // ��� �г��� ũ�⸦ �����Ͽ� ��Ÿ���� ��
        resultPanel.transform.localScale = Vector3.one;

        Time.timeScale = 0f;
    }

    private void PauseGame()
    {
        if (isPaused) return;  // �̹� �Ͻ����� ���¶�� �������� ����

        // �Ͻ����� ���·� ��ȯ
        isPaused = true;
        pausePanel.transform.localScale = Vector3.one; // UI �г��� ��Ÿ��
        Time.timeScale = 0f;  // ������ ����
    }

    private void ResumeGame()
    {
        if (!isPaused) return; // �Ͻ����� ���°� �ƴ϶�� �������� ����

        // ���� �簳
        isPaused = false;
        pausePanel.transform.localScale = Vector3.zero; // UI �г��� ����
        Time.timeScale = 1f;  // ������ �簳��
    }

    private void HandleCameraMovement()
    {
        // ���콺 Ŭ�� �� �巡�� ���� ��ġ ����
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
        }

        // ���콺 �巡�� ���� �� ī�޶� �̵�
        if (Input.GetMouseButton(0))
        {
            Vector3 difference = Input.mousePosition - dragOrigin;
            Vector3 move = new Vector3(-difference.x * dragSpeed * Time.unscaledDeltaTime, 0, 0);

            // ī�޶��� �̵��� ����
            float targetX = Mathf.Clamp(mainCamera.transform.position.x + move.x, -16f, 16f);
            mainCamera.transform.position = new Vector3(targetX, mainCamera.transform.position.y, mainCamera.transform.position.z);

            // �巡�� ���� ��ġ ������Ʈ
            dragOrigin = Input.mousePosition;
        }
    }
}
