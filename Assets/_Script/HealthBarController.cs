using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarController : MonoBehaviour
{
    public TextMeshProUGUI valueText;
    public Image fillBar;

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        if (maxValue <= 0 || float.IsNaN(currentValue) || float.IsNaN(maxValue))
        {
            Debug.LogWarning("Skip health update: invalid value");
            return;
        }

        fillBar.fillAmount = currentValue / maxValue;
        valueText.text = Mathf.Clamp(currentValue, 0, maxValue).ToString("0");
    }
}
