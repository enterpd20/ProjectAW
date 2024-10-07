using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    // ������ �Ʊ� ĳ���͵��� ������ ����Ʈ
    public List<Character> spawnedAlliedCharacters = new List<Character>();

    //public GameObject[] characterPrefabs;  // ĳ���� ������ �迭
    public Transform[] mainFleetSpawnPoints;      // BB, CV�� ���� ���� �迭
    public Transform[] vanguardFleetSpawnPoints;  // DD, CLCA�� ���� ���� �迭

    public delegate void OnSpawnComplete();
    public static event OnSpawnComplete SpawnComplete; // ���� �Ϸ� �̺�Ʈ

    private void Start()
    {
        SpawnCharacters();

        // ĳ���� ������ �Ϸ�Ǿ����� �˸�
        SpawnComplete?.Invoke();
    }

    private void SpawnCharacters()
    {
        int mainFleetSpawnIndex = 0;        // BB, CV�� ���� ���� �ε���
        int vanguardFleetSpawnIndex = 0;    // DD, CLCA�� ���� ���� �ε���
        
        // Player�� selectedCharacterIdices �迭�� ����ؼ� ������ ĳ���� ��������
        for (int i = 0; i < Player.Instance.selectedCharacterIndices.Length; i++)
        {
            int characterIndex = Player.Instance.selectedCharacterIndices[i];

            // ��ȿ�� ĳ���� �ε����� ��� ����
            if (characterIndex != -1 && characterIndex < Player.Instance.ownedCharacter.Count)
            {
                Character characterData = Player.Instance.ownedCharacter[characterIndex];

                Transform spawnPoint = null;
                if(characterData.shipType == "BB" || characterData.shipType == "CV")
                {
                    if(mainFleetSpawnIndex < mainFleetSpawnPoints.Length)
                    {
                        spawnPoint = mainFleetSpawnPoints[mainFleetSpawnIndex];
                        mainFleetSpawnIndex++;
                    }
                }
                else if(characterData.shipType == "DD" || characterData.shipType == "CLCA")
                {
                    if(vanguardFleetSpawnIndex < vanguardFleetSpawnPoints.Length)
                    {
                        spawnPoint = vanguardFleetSpawnPoints[vanguardFleetSpawnIndex];
                        vanguardFleetSpawnIndex++;
                    }
                }

                // ĳ���� �������� �����ϰ� ��ġ�ϱ�
                if(spawnPoint != null)
                {
                    // ������ �̸��� ���� �������� �ε�
                    GameObject characterPrefab = Resources.Load<GameObject>($"Prefabs_Character/{characterData.prefabName}");

                    if (characterPrefab != null)
                    {
                        GameObject characterObject = Instantiate(characterPrefab, spawnPoint.position, spawnPoint.rotation);
                        characterObject.name = characterData.name;

                        // ������ ĳ���� ������ ����Ʈ�� ����
                        spawnedAlliedCharacters.Add(characterData);

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
                        teamManager.team = TeamManager.Team.Ally;
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

    // ������ �Ʊ� ĳ���� �����͸� �������� �޼���
    public List<Character> GetSpawnedAlliedCharacters()
    {
        return spawnedAlliedCharacters;
    }
}
