using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapStarDisplay : MonoBehaviour
{
    public GameObject[] stars; // size = 3
    public int mapIndex;

    void Start()
    {
        int starCount = PlayerPrefs.GetInt($"Map{mapIndex}_Star", 0);
        UpdateStars(starCount);
    }

    public void UpdateStars(int count)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(i < count);
        }
    }
}
