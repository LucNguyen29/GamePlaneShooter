using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sound_BTN : MonoBehaviour
{
    private Button btn;
    // Start is called before the first frame update
    void Awake()
    {
        btn = GetComponent<Button>();
    }

    private void Start()
    {
        if (btn == null) return;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => PlaySound());
    }

    // Update is called once per frame
    private void PlaySound()
    {
        Debug.Log("Button clicked");

        // Kiểm tra xem SFXController đã sẵn sàng chưa
        if (SFXController.Ins == null || SFXController.Ins.SFXSource == null)
        {
            Debug.Log("SFXController is null or SFXSource is missing!");
            return;
        }

        // Đảm bảo rằng âm thanh không phát khi chưa sẵn sàng
        if (!SFXController.Ins.SFXSource.isPlaying)
        {
            SFXController.Ins.PlaySound(SFXController.Ins.popSound);
        }
    }
}
