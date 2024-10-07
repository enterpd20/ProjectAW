using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    // 스폰된 아군 캐릭터들을 저장할 리스트
    public List<Character> spawnedAlliedCharacters = new List<Character>();

    //public GameObject[] characterPrefabs;  // 캐릭터 프리팹 배열
    public Transform[] mainFleetSpawnPoints;      // BB, CV용 스폰 지점 배열
    public Transform[] vanguardFleetSpawnPoints;  // DD, CLCA용 스폰 지점 배열

    public delegate void OnSpawnComplete();
    public static event OnSpawnComplete SpawnComplete; // 스폰 완료 이벤트

    private void Start()
    {
        SpawnCharacters();

        // 캐릭터 스폰이 완료되었음을 알림
        SpawnComplete?.Invoke();
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

                // 캐릭터 프리팹을 생성하고 배치하기
                if(spawnPoint != null)
                {
                    // 프리팹 이름을 통해 프리팹을 로드
                    GameObject characterPrefab = Resources.Load<GameObject>($"Prefabs_Character/{characterData.prefabName}");

                    if (characterPrefab != null)
                    {
                        GameObject characterObject = Instantiate(characterPrefab, spawnPoint.position, spawnPoint.rotation);
                        characterObject.name = characterData.name;

                        // 스폰된 캐릭터 정보를 리스트에 저장
                        spawnedAlliedCharacters.Add(characterData);

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

    // 스폰된 아군 캐릭터 데이터를 가져오는 메서드
    public List<Character> GetSpawnedAlliedCharacters()
    {
        return spawnedAlliedCharacters;
    }
}
