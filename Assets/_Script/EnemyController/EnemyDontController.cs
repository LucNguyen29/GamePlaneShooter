using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDontController : EnemyController
{
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        SetupHealth(50);
    }

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

}
