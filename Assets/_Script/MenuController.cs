using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject panelSetting;
    public void StartGame()
    {
        SceneManager.LoadScene("MenuChoiceScene");
    }

    public void BackMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void AppearPanelSetting()
    {
        panelSetting.SetActive(true);
    }

    public void HiddenPanelSetting()
    {
        panelSetting.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
