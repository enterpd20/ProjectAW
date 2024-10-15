using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform[] enemy_mainFleetSpawnPoints;      // BB, CV�� ���� ���� �迭
    public Transform[] enemy_vanguardFleetSpawnPoints;  // DD, CLCA�� ���� ���� �迭

    public delegate void OnEnemySpawnComplete();
    public static event OnEnemySpawnComplete EnemySpawnComplete; // ���� ���� �Ϸ� �̺�Ʈ

    private void Start()
    {
        SpawnEnemies();

        // ���� ������ �Ϸ�Ǿ����� �˸�
        EnemySpawnComplete?.Invoke();
    }

    private void SpawnEnemies()
    {
        int enemy_mainFleetSpawnIndex = 0;        // BB, CV�� ���� ���� �ε���
        int enemy_vanguardFleetSpawnIndex = 0;    // DD, CLCA�� ���� ���� �ε���

        // Player�� enemyCharacter ����Ʈ�� ����ؼ� �� ĳ���͸� ����
        List<Character> enemyCharacters = Player.Instance.enemyCharacter;

        // Player�� selectedCharacterIdices �迭�� ����ؼ� ������ ĳ���� ��������
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

            // ĳ���� �������� �����ϰ� ��ġ�ϱ�
            if (spawnPoint != null)
            {
                // ������ �̸��� ���� �������� �ε�
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

                    // TeamManager ����
                    TeamManager teamManager = characterObject.GetComponent<TeamManager>();
                    if (teamManager == null)
                    {
                        teamManager = characterObject.AddComponent<TeamManager>(); // TeamManager ������Ʈ�� ���� ��� �߰�
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
