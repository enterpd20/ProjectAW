using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CharacterDataLoader
{
    // JSON ���Ͽ��� ĳ���� �����͸� �ҷ����� �޼���
   public static List<Character> LoadCharaters()
    {
        // JSON ���� ��� (Resource ������ JSON ������ ���� ��)
        string filePath = Path.Combine(Application.streamingAssetsPath, "CharacterInfo.json");

        if(File.Exists(filePath))
        {
            // JSON ���� �б�
            string jsonData = File.ReadAllText(filePath);

            // JSON �����͸� JSONList_Character ��ũ��Ʈ�� ���� CharacterList ��ü�� ��ȯ
            CharacterList characterList = JsonUtility.FromJson<CharacterList>(jsonData);

            // ����Ʈ ��ȯ
            return characterList.Characters;
        }
        else
        {
            Debug.LogError("There's no Character JSON File: " + filePath);
            return new List<Character>();
        }
    }
}
