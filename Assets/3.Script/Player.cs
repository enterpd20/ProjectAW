using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Player : MonoBehaviour
{
    // C - User - KGA - AppData - LocalLow - DefaultCompany - ProjectAW

    public static Player Instance { get; private set; }

    public List<Character> ownedCharacter;
    public List<Gear> gears;
    //public List<Stage> clearedStage;  // �������� �ۼ��ؾ��� 240919

    public int selectedCharacterIndex = -1;             // Dock���� ����� ĳ���� �ε���
    public int[] selectedCharacterIndices = new int[6]; //SelectStage�� ĳ���� ���� ����� �ε���
    //public int[] selectedCharacterIndices; //SelectStage�� ĳ���� ���� ����� �ε���

    public CharacterStats finalCharacterStats;

    private void Awake()
    {
        Debug.Log("Player Awake called");

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadPlayerData();   // ���� ���� �� ����� �����͸� �ҷ���
            InitializePlayerData(); // ĳ���� ��ġ ��ư �迭 �ʱ�ȭ
            Debug.Log("Player Initialized and Data Loaded");
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
            gears = this.gears,
            selectedCharacterIndex = this.selectedCharacterIndex,
            selectedCharacterIndices = this.selectedCharacterIndices,
            finalCharacterStats = this.finalCharacterStats
            //returnToScene = this.returnToScene,
            //isFormationUIActive = this.isFormationUIActive
        };
        string json = JsonUtility.ToJson(this);
        File.WriteAllText(path, json);
        Debug.Log("Player data saved to file: " + path);
    }

    public void InitializePlayerData()
    {
        // �迭�� �ʱ�ȭ���� �ʾ��� ��� -1�� �ʱ�ȭ
        if(selectedCharacterIndices == null || selectedCharacterIndices.Length != -6)
        {
            selectedCharacterIndices = new int[6];
        }

        for(int i = 0; i < selectedCharacterIndices.Length; i++)
        {
            selectedCharacterIndices[i] = -1;
        }

        Debug.Log("selectedCharacterIndices initialized with length: " + selectedCharacterIndices.Length);
    }

    public void LoadPlayerData()
    {
        string path = Application.persistentDataPath + "/playerData.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);

            ownedCharacter = playerData.ownedCharacter;
            gears = playerData.gears;
            selectedCharacterIndex = playerData.selectedCharacterIndex;
            selectedCharacterIndices = playerData.selectedCharacterIndices;
            finalCharacterStats = playerData.finalCharacterStats;
            //returnToScene = playerData.returnToScene;
            //isFormationUIActive = playerData.isFormationUIActive;

            Debug.Log("Player data loaded from file: " + json);
        }
        else
        {
            Debug.LogWarning("No save data file found at: " + path);

            ownedCharacter = new List<Character>();
            gears = new List<Gear>();

            SavePlayerData();
            Debug.Log("New Player data created and saved.");
        }
    }

    public Character GetSelectedCharacter_DockUI()
    {
        if(selectedCharacterIndex >= 0 && selectedCharacterIndex < ownedCharacter.Count)
        {
            return ownedCharacter[selectedCharacterIndex];
        }
        return null;
    }

    public Character GetSelectedCharacter_SelectStage(int index)
    {
        if(index >= 0 && index < ownedCharacter.Count)
        {
            return ownedCharacter[index];
        }
        return null;
    }
}

[System.Serializable]
public class PlayerData
{
    public List<Character> ownedCharacter;
    public List<Gear> gears;
    public int selectedCharacterIndex;
    public int[] selectedCharacterIndices;

    public CharacterStats finalCharacterStats;
    //public string returnToScene;
    //public bool isFormationUIActive;
}
