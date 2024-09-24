using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangingManager : MonoBehaviour
{
    //private Stack<string> sceneHistory = new Stack<string>();   // �� �̵� ����� ���� ����
    //private string returnToScene = "";                          // ������ ������ ���ƿ� �� ����� ����
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
    //// �� ��ȯ �� �� ���� ���ÿ� �����ϰ� �� ������ �̵�
    //public void ChangeScene(string sceneName)
    //{
    //    string currentScene = SceneManager.GetActiveScene().name;
    //    sceneHistory.Push(currentScene);    // ���� ���� ���ÿ� ����
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
    //        string previousScene = sceneHistory.Pop();  // ���ÿ��� ���� �� ������
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
    //    returnToScene = "03_SelectStage"; // ���ƿ� ���� ����
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

    //// Dock ������ ĳ���� ���� �� SelectStage�� ���ư�
    //public void SelectCharacterAndReturn(int characterIndex)
    //{
    //    Player.Instance.selectedCharacterIndex = characterIndex;
    //    Player.Instance.SavePlayerData();
    //
    //    // �ٽ� SelectStage ������ ���ư�
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
