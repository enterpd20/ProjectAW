using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CharacterDataLoader
{
    // JSON 파일에서 캐릭터 데이터를 불러오는 메서드
   public static List<Character> LoadCharaters()
    {
        // JSON 파일 경로 (Resource 폴더에 JSON 파일을 넣을 것)
        string filePath = Path.Combine(Application.streamingAssetsPath, "CharacterInfo.json");

        if(File.Exists(filePath))
        {
            // JSON 파일 읽기
            string jsonData = File.ReadAllText(filePath);

            // JSON 데이터를 JSONList_Character 스크립트를 거쳐 CharacterList 객체로 변환
            CharacterList characterList = JsonUtility.FromJson<CharacterList>(jsonData);

            // 리스트 반환
            return characterList.Characters;
        }
        else
        {
            Debug.LogError("There's no Character JSON File: " + filePath);
            return new List<Character>();
        }
    }
}
