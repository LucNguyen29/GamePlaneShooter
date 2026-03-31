using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonEvent : MonoBehaviour
{
    public Button[] acceptButtons;

    void Start()
    {
        // Gán sự kiện cho tất cả các nút khi scene load
        SetupButtonEvents();
    }

    // Phương thức gán event cho các nút Accept
    void SetupButtonEvents()
    {
        if (SceneFlowManager.instance == null) return;

        foreach (Button btn in acceptButtons)
        {
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();  // Xóa sự kiện cũ
                btn.onClick.AddListener(OnButtonClick);  // Thêm sự kiện mới
            }
        }
    }

    // Phương thức được gọi khi button nhấn
    public void OnButtonClick()
    {
        // Thực hiện hành động khi nhấn button
        Debug.Log("Button clicked!");
        SceneFlowManager.instance.Accept();
    }
}
