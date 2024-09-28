using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public GameObject[] characterPrefabs;  // 캐릭터 프리팹 배열
    public Transform[] spawnPoints;      // 캐릭터가 스폰되는 위치 배열

    private void Start()
    {
        SpawnCharacters();
    }

    private void SpawnCharacters()
    {
        int mainFleetSpawnIndex = 0;        // BB, CV용 스폰 지점 인덱스
        int vanguardFleetSpawnIndex = 0;    // DD, CLCA용 스폰 지점 인덱스
        
        // Player의 selectedCharacterIdices 배열을 사용해서 스폰할 캐릭터 가져오기
        for (int i = 0; i < Player.Instance.selectedCharacterIndices.Length; i++)
        {
            int characterIndex = Player.Instance.selectedCharacterIndices[i];

            // 유효한 캐릭터 인덱스인 경우 스폰
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

                // 캐릭터 프리팹을 생성하고 배치하기
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
