using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LiveBarController : MonoBehaviour
{
    public Image[] livesIcons;

    // Gọi khi player mất mạng
    public void UpdateLives(int remainingLives)
    {
        for (int i = 0; i < livesIcons.Length; i++)
        {
            livesIcons[i].enabled = i < remainingLives;
        }
    }
}
