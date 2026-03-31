using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaserParticleController : EnemyController
{
    [SerializeField] private Transform[] attackPos;
    [SerializeField] private BulletData bulletParticle;
    // Start is called before the first frame update
    void Start()
    {
        SetupHealth(50);
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
        isHandlingOutOfScreen = false;
    }

    IEnumerator AutoFire()
    {
        while (true)
        {
            yield return new WaitForSeconds(3.0f);
            CreateBullet();
        }
    }

    void CreateBullet()
    {
        Instantiate(bulletParticle.bulletPrefab, attackPos[0].position, transform.rotation);
        Instantiate(bulletParticle.bulletPrefab, attackPos[1].position, transform.rotation);
    }
}
