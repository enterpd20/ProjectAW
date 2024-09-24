using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangingManager : MonoBehaviour
{
    //private Stack<string> sceneHistory = new Stack<string>();   // 씬 이동 기록을 위한 스택
    //private string returnToScene = "";                          // 선택한 씬으로 돌아올 때 사용할 변수
    //
    //public static SceneChangingManager Instance { get; private set; }
    //
    //private void Awake()
    //{
    //    if(Instance == null)
    //    {
    //        Instance = this;
    //        DontDestroyOnLoad(this.gameObject);
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }
    //}
    //
    //// 씬 전환 시 현 씬을 스택에 저장하고 새 씬으로 이동
    //public void ChangeScene(string sceneName)
    //{
    //    string currentScene = SceneManager.GetActiveScene().name;
    //    sceneHistory.Push(currentScene);    // 현재 씬을 스택에 저장
    //
    //    Debug.Log($"Changing scene from {currentScene} to {sceneName}");
    //
    //    SceneManager.LoadScene(sceneName);
    //}
    //
    //public void GoBack()
    //{
    //    if(sceneHistory.Count > 0)
    //    {
    //        string previousScene = sceneHistory.Pop();  // 스택에서 이전 씬 꺼내기
    //        SceneManager.LoadScene(previousScene);
    //    }
    //    else
    //    {
    //        Debug.LogWarning("There's no previous scene.");
    //    }
    //}
    //
    //public void GoToDockFromSelectStage()
    //{
    //    returnToScene = "03_SelectStage"; // 돌아올 씬을 저장
    //    SceneManager.LoadScene("02_Dock");
    //}

    public void To_Title()
    {
        SceneManager.LoadScene("00_Title");
    }
    public void To_Lobby()
    {
        SceneManager.LoadScene("01_Lobby");
    }
    public void To_Dock()
    {
        SceneManager.LoadScene("02_Dock");
    }
    public void To_DockDetail()
    {
        SceneManager.LoadScene("02_DockDetail");
    }
    public void To_Depot()
    {
        SceneManager.LoadScene("02_Depot");
    }
    public void To_Build()
    {
        SceneManager.LoadScene("02_Build");
    }
    public void To_Lab()
    {
        SceneManager.LoadScene("02_Lab");
    }
    public void To_SelectBattle()
    {
        SceneManager.LoadScene("03_SelectBattle");
    }
    public void To_SelectStage()
    {
        SceneManager.LoadScene("03_SelectStage");
    }
    public void To_Battle()
    {
        SceneManager.LoadScene("04_Battle");
    }

    //// Dock 씬에서 캐릭터 선택 후 SelectStage로 돌아감
    //public void SelectCharacterAndReturn(int characterIndex)
    //{
    //    Player.Instance.selectedCharacterIndex = characterIndex;
    //    Player.Instance.SavePlayerData();
    //
    //    // 다시 SelectStage 씬으로 돌아감
    //    string returnToScene = Player.Instance.returnToScene;
    //    if (!string.IsNullOrEmpty(returnToScene))
    //    {
    //        SceneManager.LoadScene(returnToScene);
    //        Player.Instance.returnToScene = "";
    //        Player.Instance.SavePlayerData();
    //    }
    //    else
    //    {
    //        Debug.LogError("There's no scene to return.");
    //    }
    //}

}
