using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Player : MonoBehaviour
{
    // C - User - KGA - AppData - LocalLow - DefaultCompany - ProjectAW

    public static Player Instance { get; private set; }

    public List<Character> enemyCharacter;
    public List<Character> ownedCharacter;
    public List<Gear> gears;

    public int selectedCharacterIndex = -1;             // Dock에서 사용할 캐릭터 인덱스
    public int[] selectedCharacterIndices = new int[6]; //SelectStage의 캐릭터 편성에 사용할 인덱스

    public CharacterStats finalCharacterStats;

    private void Awake()
    {
        //Debug.Log("Player Awake called");

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            GearDataLoader.LoadAllGears(); // 모든 장비 데이터 로드

            LoadPlayerData();   // 게임 시작 시 저장된 데이터를 불러움
            InitializePlayerData(); // 캐릭터 배치 버튼 배열 초기화

            //Debug.Log($"Gears count in Player after loading: {gears.Count}");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SavePlayerData()
    {
        string path = Application.persistentDataPath + "/playerData.json";
        PlayerData data = new PlayerData
        {
            ownedCharacter = this.ownedCharacter,
            enemyCharacter = this.enemyCharacter,
            gears = this.gears,
            selectedCharacterIndex = this.selectedCharacterIndex,
            selectedCharacterIndices = this.selectedCharacterIndices,
            finalCharacterStats = this.finalCharacterStats
        };
        string json = JsonUtility.ToJson(this);
        File.WriteAllText(path, json);
        Debug.Log("Player data saved to file: " + path);
    }

    public void InitializePlayerData()
    {
        // 배열이 초기화되지 않았을 경우 -1로 초기화
        if (selectedCharacterIndices == null || selectedCharacterIndices.Length != -6)
        {
            selectedCharacterIndices = new int[6];
        }

        for (int i = 0; i < selectedCharacterIndices.Length; i++)
        {
            selectedCharacterIndices[i] = -1;
        }

        //Debug.Log("selectedCharacterIndices initialized with length: " + selectedCharacterIndices.Length);
    }

    public void LoadPlayerData()
    {
        string path = Application.persistentDataPath + "/playerData.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);

            ownedCharacter = playerData.ownedCharacter;
            enemyCharacter = playerData.enemyCharacter;
            gears = playerData.gears;
            selectedCharacterIndex = playerData.selectedCharacterIndex;
            selectedCharacterIndices = playerData.selectedCharacterIndices;
            finalCharacterStats = playerData.finalCharacterStats;

            //Debug.Log("Player data loaded from file: " + json);

            //Debug.Log($"Loaded gears count: {gears.Count}");
            //foreach (var gear in gears)
            //{
            //    Debug.Log($"Loaded gear: {gear.name}, Type: {gear.gearType}");
            //}
            //
            //if (finalCharacterStats != null)
            //{
            //    Debug.Log($"Loaded final stats: HP = {finalCharacterStats.HP}, FP = {finalCharacterStats.FP}, SPD = {finalCharacterStats.SPD}");
            //}
            //else
            //{
            //    Debug.LogWarning("Final character stats are null in the loaded data.");
            //}
        }
        else
        {
            Debug.LogWarning("No save data file found at: " + path);

            ownedCharacter = new List<Character>
            {
                new Character
                {
                    name = "Ayanami",
                    shipType = "DD",
                    rarity = "SR",
                    faction = "SakuraEmpire",
                    imageName = "Ayanami",
                    prefabName = "CharaPrefab_Ayanami",
                    hasUniqueModule = true,
                    stats = new CharacterStats
                    {
                        HP = 2007,
                        FP = 76,
                        TRP = 576,
                        AVI = 0,
                        AA = 149,
                        SPD = 48
                    },
                    eqiuppedGears = new List<string> { "Twin 127mm (Type 3)",
                                                       "610mm Quadruple Torpedo Mount",
                                                       "Twin 25mm AA (Type 96)",
                                                       "Type 93 Pure Oxygen Torpedo" }
                },
                new Character
                {
                    name = "Fusou",
                    shipType = "BB",
                    rarity = "R",
                    faction = "SakuraEmpire",
                    imageName = "Fusou",
                    prefabName = "CharaPrefab_Fusou",
                    hasUniqueModule = false,
                    stats = new CharacterStats
                    {
                        HP = 100,
                        FP = 0,
                        TRP = 0,
                        AVI = 200,
                        AA = 200,
                        SPD = 50
                    },
                    eqiuppedGears = new List<string> { "Twin 356mm (41st Year Type)",
                                                       "Single 152mm (41st Year Type)",
                                                       "Twin 127mm AA (Type 89)",
                                                       "Fire Control Radar" }
                }
            };
            enemyCharacter = new List<Character>();
            gears = new List<Gear>();

            SavePlayerData();
            Debug.Log("New Player data created and saved.");
        }
    }

    public Character GetSelectedCharacter_DockUI()
    {
        if (selectedCharacterIndex >= 0 && selectedCharacterIndex < ownedCharacter.Count)
        {
            return ownedCharacter[selectedCharacterIndex];
        }
        return null;
    }

    public Character GetSelectedCharacter_SelectStage(int index)
    {
        if (index >= 0 && index < ownedCharacter.Count)
        {
            return ownedCharacter[index];
        }
        return null;
    }

    public void SaveFinalCharacterStats(CharacterStats finalStats)
    {
        finalCharacterStats = finalStats;
        Debug.Log($"Saved final stats: HP = {finalStats.HP}, FP = {finalStats.FP}, SPD = {finalStats.SPD}");
        SavePlayerData();
    }
}

[System.Serializable]
public class PlayerData
{
    public List<Character> ownedCharacter;
    public List<Character> enemyCharacter;
    public List<Gear> gears;
    public int selectedCharacterIndex;
    public int[] selectedCharacterIndices;

    public CharacterStats finalCharacterStats;
}
