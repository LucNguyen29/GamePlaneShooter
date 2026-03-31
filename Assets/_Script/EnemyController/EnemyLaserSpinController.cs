using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaserSpinController : EnemyController
{
    [SerializeField] private BulletData laserSpin;
    [SerializeField] private Transform attackPos;
    // Start is called before the first frame update
    void Start()
    {
        SetupHealth(50);
        transform.rotation = Quaternion.Euler(0, 0, 180);
        StartCoroutine(AutoFire());
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsVisibleOnScreen() && !isHandlingOutOfScreen)
        {
            isHandlingOutOfScreen = true;
            StartCoroutine(HandleOutOfScreen());
        }
    }

    IEnumerator HandleOutOfScreen()
    {
        // Tắt collider để tránh va chạm trong thời gian chờ
        GetComponent<Collider2D>().enabled = false;

        // Gọi hàm trừ enemyCount
        GameController.Instance.EnemyOutOfScreen();

        anim.Play("Explosion");
        // Chờ 0.5s rồi mới huỷ (có thể show animation nhỏ nếu thích)
        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }

    IEnumerator AutoFire()
    {
        while (true)
        {
            yield return new WaitForSeconds(5.0f);
            CreateLaserSpin();
        }
    }
    void CreateLaserSpin()
    {
        Instantiate(laserSpin.bulletPrefab, attackPos.position, transform.rotation);
    }
}
