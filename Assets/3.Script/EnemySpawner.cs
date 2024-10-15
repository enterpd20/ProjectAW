using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform[] enemy_mainFleetSpawnPoints;      // BB, CV용 스폰 지점 배열
    public Transform[] enemy_vanguardFleetSpawnPoints;  // DD, CLCA용 스폰 지점 배열

    public delegate void OnEnemySpawnComplete();
    public static event OnEnemySpawnComplete EnemySpawnComplete; // 적군 스폰 완료 이벤트

    private void Start()
    {
        SpawnEnemies();

        // 적군 스폰이 완료되었음을 알림
        EnemySpawnComplete?.Invoke();
    }

    private void SpawnEnemies()
    {
        int enemy_mainFleetSpawnIndex = 0;        // BB, CV용 스폰 지점 인덱스
        int enemy_vanguardFleetSpawnIndex = 0;    // DD, CLCA용 스폰 지점 인덱스

        // Player의 enemyCharacter 리스트를 사용해서 적 캐릭터를 스폰
        List<Character> enemyCharacters = Player.Instance.enemyCharacter;

        // Player의 selectedCharacterIdices 배열을 사용해서 스폰할 캐릭터 가져오기
        for (int i = 0; i < enemyCharacters.Count; i++)
        {
            Character characterData = enemyCharacters[i];

            Transform spawnPoint = null;
            if (characterData.shipType == "BB" || characterData.shipType == "CV")
            {
                if (enemy_mainFleetSpawnIndex < enemy_mainFleetSpawnPoints.Length)
                {
                    spawnPoint = enemy_mainFleetSpawnPoints[enemy_mainFleetSpawnIndex];
                    enemy_mainFleetSpawnIndex++;
                }
            }
            else if (characterData.shipType == "DD" || characterData.shipType == "CLCA")
            {
                if (enemy_vanguardFleetSpawnIndex < enemy_vanguardFleetSpawnPoints.Length)
                {
                    spawnPoint = enemy_vanguardFleetSpawnPoints[enemy_vanguardFleetSpawnIndex];
                    enemy_vanguardFleetSpawnIndex++;
                }
            }

            // 캐릭터 프리팹을 생성하고 배치하기
            if (spawnPoint != null)
            {
                // 프리팹 이름을 통해 프리팹을 로드
                GameObject characterPrefab = Resources.Load<GameObject>($"Prefabs_Character/{characterData.prefabName}");

                if (characterPrefab != null)
                {
                    GameObject characterObject = Instantiate(characterPrefab, spawnPoint.position, spawnPoint.rotation);
                    characterObject.name = characterData.name;

                    BattleAI battleAI = characterObject.GetComponent<BattleAI>();
                    if (battleAI != null)
                    {
                        battleAI.InitializeCharacterStats(characterData.stats);
                    }

                    // TeamManager 설정
                    TeamManager teamManager = characterObject.GetComponent<TeamManager>();
                    if (teamManager == null)
                    {
                        teamManager = characterObject.AddComponent<TeamManager>(); // TeamManager 컴포넌트가 없을 경우 추가
                    }
                    teamManager.team = TeamManager.Team.Enemy;
                }
                else
                {
                    Debug.LogError($"Prefab with name {characterData.prefabName} not found in Resources/Prefabs.");
                }
            }
            else
            {
                Debug.LogWarning($"No valid spawn point available for character: {characterData.name}");
            }
        }
    }
}
