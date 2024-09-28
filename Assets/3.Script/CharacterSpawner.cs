using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public GameObject[] characterPrefabs;  // ĳ���� ������ �迭
    public Transform[] spawnPoints;      // ĳ���Ͱ� �����Ǵ� ��ġ �迭

    private void Start()
    {
        SpawnCharacters();
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
                    if(mainFleetSpawnIndex < 3)
                    {
                        spawnPoint = spawnPoints[mainFleetSpawnIndex];
                        mainFleetSpawnIndex++;
                    }
                }
                else if(characterData.shipType == "DD" || characterData.shipType == "CLCA")
                {
                    if(vanguardFleetSpawnIndex < 6)
                    {
                        spawnPoint = spawnPoints[vanguardFleetSpawnIndex];
                        vanguardFleetSpawnIndex++;
                    }
                }

                // ĳ���� �������� �����ϰ� ��ġ�ϱ�
                if(spawnPoint != null)
                {
                    GameObject characterPrefab = characterPrefabs[characterIndex];
                    GameObject characterObject = Instantiate(characterPrefab, spawnPoint.position, spawnPoint.rotation);
                    characterObject.name = characterData.name;

                    BattleAI battleAI = characterObject.GetComponent<BattleAI>();
                    if (battleAI != null)
                    {
                        battleAI.InitializeCharacterStats(characterData.stats);
                    }

                    TeamManager teamManager = characterObject.GetComponent<TeamManager>();
                    if(teamManager != null)
                    {
                        teamManager.team = TeamManager.Team.Ally;
                    }
                }
                else
                {
                    Debug.LogWarning($"No valid spawn point available for character: {characterData.name}");
                }
            }
        }
    }
}
