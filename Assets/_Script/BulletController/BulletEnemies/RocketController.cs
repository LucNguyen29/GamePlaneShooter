using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : BulletController
{
    private Vector3 startPosition;
    private Vector3 targetPosition;

    private float arcHeight = 3f;
    private float travelTime = 1.5f;
    private float elapSpeedTime = 0.0f;
    private float arcDirection = 1f; // Hướng vòng cung (1: phải, -1: trái)

    public void SetTarget(Vector3 playerPosition, float direction)
    {
        startPosition = transform.position;
        targetPosition = playerPosition;
        arcDirection = direction;
    }

    void Update()
    {
        elapSpeedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapSpeedTime / travelTime); // Nội suy từ 0 -> 1

        // Tính vị trí di chuyển thẳng từ điểm xuất phát đến player
        Vector3 flatPosition = Vector3.Lerp(startPosition, targetPosition, t);

        // Tạo độ cao theo vòng cung (parabol)
        float height = Mathf.Sin(t * Mathf.PI) * arcHeight;

        // Tạo chuyển động ngang theo vòng cung (từ ngoài vào)
        float sideOffset = Mathf.Cos(t * Mathf.PI) * arcDirection * arcHeight; 

        // Gọi phương thức Move từ BulletController để di chuyển
        Vector3 moveDirection = new Vector3(flatPosition.x + sideOffset, flatPosition.y + height, flatPosition.z) - transform.position;
        Move(moveDirection.normalized); 

        // Hủy rocket khi đến gần mục tiêu
        // if (t >= 1f)
        // {
        //     Destroy(gameObject);
        // }
    }
}
