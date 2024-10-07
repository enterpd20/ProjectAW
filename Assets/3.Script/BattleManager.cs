using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public float battleTimeLimit = 180f; // 전투 시간 제한 (3분)
    private float battleTimer;
    private bool isBattleOver = false;

    private List<BattleAI> allyCharacters;
    private List<BattleAI> enemyCharacters;


    // 전투 결과 UI
    public GameObject resultPanel;  // 결과를 표시할 패널
    public Text resultText;         // 결과 텍스트

    // 일시정지 관련 UI
    public GameObject pausePanel;   // 일시정지 UI 패널
    public Button pauseButton;      // 일시정지 버튼
    public Button resumeButton;     // 재개 버튼
    private bool isPaused = false;  // 현재 일시정지 상태를 추적

    // 카메라 움직임 관련
    public Camera mainCamera;       // 메인 카메라
    public float dragSpeed = 1f;    // 마우스 드래그 속도
    private Vector3 dragOrigin;     // 드래그 시작 위치

    void Start()
    {
        resultPanel.transform.localScale = Vector3.zero;

        pausePanel.transform.localScale = Vector3.zero;
        pauseButton.onClick.AddListener(PauseGame);
        resumeButton.onClick.AddListener(ResumeGame);

        // CharacterSpawner의 스폰 완료 이벤트에 구독
        CharacterSpawner.SpawnComplete += OnCharacterSpawnComplete;
        EnemySpawner.EnemySpawnComplete += OnEnemySpawnComplete;
    }

    private void OnCharacterSpawnComplete()
    {
        // 스폰이 완료되면 전투 초기화
        InitializeBattle();
    }

    private void OnEnemySpawnComplete()
    {
        InitializeBattle();
    }

    void OnEnable()
    {
        // 씬이 활성화될 때마다 타이머 초기화
        ResetBattleTimer();
    }

    void Update()
    {
        if (isBattleOver || isPaused) return;

        // 타이머 감소
        battleTimer -= Time.unscaledDeltaTime;
        if (battleTimer <= 0)
        {
            CheckBattleResult();
            return;
        }

        // 승리/패배/무승부 조건 체크
        CheckBattleResult();

        // 카메라 움직임
        HandleCameraMovement();
    }

    private void InitializeBattle()
    {
        ResetBattleTimer();

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

        // 디버그 로그 추가: 스폰된 아군/적군 캐릭터 수 및 이름 확인
        Debug.Log($"[InitializeBattle] 스폰된 아군 캐릭터 수: {allyCharacters.Count}");
        foreach (BattleAI ally in allyCharacters)
        {
            Debug.Log($"[InitializeBattle] 아군 캐릭터: {ally.gameObject.name}");
        }

        Debug.Log($"[InitializeBattle] 스폰된 적군 캐릭터 수: {enemyCharacters.Count}");
        foreach (BattleAI enemy in enemyCharacters)
        {
            Debug.Log($"[InitializeBattle] 적군 캐릭터: {enemy.gameObject.name}");
        }
    }

    private void ResetBattleTimer()
    {
        // 전투 타이머 초기화
        battleTimer = battleTimeLimit;
        isBattleOver = false; // 전투 종료 상태 초기화
        Time.timeScale = 1f;  // 게임이 시작될 때 타임 스케일을 1로 설정
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

        // 디버그 로그: 현재 살아있는 아군/적군의 수를 출력
        Debug.Log($"[CheckBattleResult] 살아있는 아군 수: {aliveAllies}, 살아있는 적군 수: {aliveEnemies}");

        // 1. 아군 캐릭터가 모두 사망하고 적군이 살아있다면 패배
        if (aliveAllies == 0 && aliveEnemies > 0)
        {
            Debug.Log("Battle Over: Defeat");
            isBattleOver = true;
            ShowBattleResult("Defeat", Color.red);
            // 추가적인 패배 처리 로직
        }
        // 2. 적군 캐릭터가 모두 사망하고 아군이 살아있다면 승리
        else if (aliveEnemies == 0 && aliveAllies > 0)
        {
            Debug.Log("Battle Over: Victory");
            isBattleOver = true;
            ShowBattleResult("Victory", Color.yellow);
            // 추가적인 승리 처리 로직
        }
        // 3. 양측 모두 살아있는 캐릭터가 없거나 같은 경우 무승부
        else if ((aliveAllies == 0 && aliveEnemies == 0) || aliveAllies == aliveEnemies)
        {
            Debug.Log("Battle Over: Draw");
            isBattleOver = true;
            ShowBattleResult("Draw", Color.gray);
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
        yield return new WaitForSecondsRealtime(1f); // 1초 대기

        resultText.text = resultMessage;
        resultText.color = textColor;

        // 결과 패널의 크기를 설정하여 나타나게 함
        resultPanel.transform.localScale = Vector3.one;

        Time.timeScale = 0f;
    }

    private void PauseGame()
    {
        if (isPaused) return;  // 이미 일시정지 상태라면 실행하지 않음

        // 일시정지 상태로 전환
        isPaused = true;
        pausePanel.transform.localScale = Vector3.one; // UI 패널을 나타냄
        Time.timeScale = 0f;  // 게임을 멈춤
    }

    private void ResumeGame()
    {
        if (!isPaused) return; // 일시정지 상태가 아니라면 실행하지 않음

        // 게임 재개
        isPaused = false;
        pausePanel.transform.localScale = Vector3.zero; // UI 패널을 숨김
        Time.timeScale = 1f;  // 게임을 재개함
    }

    private void HandleCameraMovement()
    {
        // 마우스 클릭 시 드래그 시작 위치 저장
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
        }

        // 마우스 드래그 중일 때 카메라 이동
        if (Input.GetMouseButton(0))
        {
            Vector3 difference = Input.mousePosition - dragOrigin;
            Vector3 move = new Vector3(-difference.x * dragSpeed * Time.unscaledDeltaTime, 0, 0);

            // 카메라의 이동을 제한
            float targetX = Mathf.Clamp(mainCamera.transform.position.x + move.x, -16f, 16f);
            mainCamera.transform.position = new Vector3(targetX, mainCamera.transform.position.y, mainCamera.transform.position.z);

            // 드래그 시작 위치 업데이트
            dragOrigin = Input.mousePosition;
        }
    }
}
