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
    //public List<Stage> clearedStage;  // 스테이지 작성해야함 240919

    public int selectedCharacterIndex = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Player.Instance.LoadPlayerData();   // 게임 시작 시 저장된 데이터를 불러움
        }
        else
        {
            Destroy(gameObject);
        }        
    }

    private void Start()
    {
    }

    public void SavePlayerData()
    {
        string path = Application.persistentDataPath + "/playerData.json";
        string json = JsonUtility.ToJson(this);
        File.WriteAllText(path, json);
        Debug.Log("Player data saved to file: " + path);
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

    public Character GetSelectedCharacter()
    {
        if(selectedCharacterIndex >= 0 && selectedCharacterIndex < ownedCharacter.Count)
        {
            return ownedCharacter[selectedCharacterIndex];
        }
        return null;
    }
}

[System.Serializable]
public class PlayerData
{
    public List<Character> ownedCharacter;
    public List<Gear> gears;
}
