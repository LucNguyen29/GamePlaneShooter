using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BackGroundController : MonoBehaviour
{
    public Transform mainCam;
    public Transform midBg;
    public Transform sideBg;
    public float length;
    public float speed = 2f;
    public float resetPositionY = -1920f;
    private Vector3 startPosition;
    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        // Nếu nền đi hết, đặt lại vị trí ban đầu
        if (transform.position.y <= resetPositionY)
        {
            transform.position = startPosition;
        }

        if (mainCam.position.y > midBg.position.y)
        {
            UpdateBackground(Vector3.up);
        }
        if (mainCam.position.y < midBg.position.y)
        {
            UpdateBackground(Vector3.down);
        }
    }

    void UpdateBackground(Vector3 direction)
    {
        sideBg.position = midBg.position + direction * length;

        Transform temp = midBg;
        midBg = sideBg;
        sideBg = temp;
    }

}
