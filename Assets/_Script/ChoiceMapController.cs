using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChoiceMapController : MonoBehaviour
{
    [SerializeField] private GameObject[] panel;
    [SerializeField] private GameObject[] starOb;
    [SerializeField] private MapStarDisplay[] starDisplays;
    private int selectedMapIndex = -1;

    void Start()
    {
        LoadAllMapStars();
    }
    private void LoadAllMapStars()
    {
        foreach (var display in starDisplays)
        {
            int count = PlayerPrefs.GetInt($"Map{display.mapIndex}_Star", 0);
            display.UpdateStars(count);
        }
    }



    public void choiceMap_1()
    {
        ShowPanel(0);
    }

    public void choiceMap_2()
    {
        ShowPanel(1);
    }
    public void choiceMap_3()
    {
        ShowPanel(2);

    }
    public void choiceMap_4()
    {
        ShowPanel(3);
    }
    public void ForWardScene()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void ShowPanel(int index)
    {
        foreach (GameObject p in panel)
        {
            p.SetActive(false);
        }

        panel[index].SetActive(true);
        selectedMapIndex = index;

    }

    public void ConfirmMapSelection()
    {
        if (selectedMapIndex >= 0 && selectedMapIndex < panel.Length)
        {
            foreach (GameObject p in panel)
            {
                p.SetActive(false);
            }
            string mapName = "Map_" + (selectedMapIndex + 1); // VD: "Map1"
            PlayerPrefs.SetString("SelectedMap", mapName);
            PlayerPrefs.Save();

            Debug.Log("Map đã xác nhận: " + mapName);

            SceneManager.LoadScene("ChoicePlane"); // Đổi tên scene cho đúng với của bạn
        }
        else
        {
            Debug.LogWarning("Chưa chọn map nào!");
        }

    }

    public void ClosePanelByIndex(int index)
    {
        if (index >= 0 && index < panel.Length)
        {
            panel[index].SetActive(false);
        }
    }

}
