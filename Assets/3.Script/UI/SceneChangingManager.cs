using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangingManager : MonoBehaviour
{
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
}
