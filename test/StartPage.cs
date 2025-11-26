using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartPage : MonoBehaviour
{
    public GameObject go_info;
    public GameObject go_opInfo;
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadStart()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void ShowInfo()
    {
        go_info.SetActive(true);
        go_opInfo.SetActive(false);
    }

    public void ShowOpInfo()
    {
        go_info.SetActive(false);
        go_opInfo.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
