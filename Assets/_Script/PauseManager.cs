using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject panelPause;
    [SerializeField] private GameObject panelSetting;
    public void PauseGame()
    {
        Time.timeScale = 0f;
        AudioListener.pause = true;

        panelPause.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        panelPause.SetActive(false);

    }

    public void AppearPanelSetting()
    {
        panelSetting.SetActive(true);
        panelPause.SetActive(false);
    }


    public void HiddenPanelSetting()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        panelSetting.SetActive(false);

    }

    public void ExitMatch()
    {
        StartCoroutine(RestartThenGoToMainMenu());
    }

    IEnumerator RestartThenGoToMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        yield return null;

        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("MenuChoiceScene");
    }
}
