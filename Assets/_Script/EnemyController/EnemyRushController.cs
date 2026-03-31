using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRushController : EnemyController
{
    private Transform player;
    private Vector2 moveDirection;
    [SerializeField] private bool isRushing = false;
    private float delayBeforeRush = 3.5f;
    void Start()
    {
        SetupHealth(70);
        StartCoroutine(PrepareRush());
        anim = GetComponent<Animator>();
    }

    IEnumerator PrepareRush()
    {
        yield return new WaitForSeconds(delayBeforeRush);
        Debug.Log("Rush started!");
        player = GameObject.FindGameObjectWithTag("Plane")?.transform;
        if (player != null)
        {
            moveDirection = (player.transform.position - transform.position).normalized;
            isRushing = true;
        }
    }
    void Update()
    {
        RushToPlayer();
    }

    void RushToPlayer()
    {
        if (currentHealth <= 0)
        {
            isRushing = false;
        }
        if (isRushing && enemyData != null && currentHealth > 0)
        {
            transform.position += (Vector3)moveDirection * enemyData.speed * Time.deltaTime;

            if (!IsVisibleOnScreen() && !isHandlingOutOfScreen)
            {
                isHandlingOutOfScreen = true;
                StartCoroutine(HandleOutOfScreen());
            }
        }
        else if (!IsVisibleOnScreen() && !isHandlingOutOfScreen)
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

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Plane"))
        {
            isRushing = false;
            anim.Play("Explosion");
            GetComponent<Collider2D>().enabled = false;

            PlayerController player = collider.GetComponent<PlayerController>();
            if (player != null)
            {
                if (!player.isInvincible)
                {
                    Destroy(collider.gameObject, 0.5f);
                }
            }
            Destroy(gameObject, 0.5f); // EnemyRush tự nổ
            GameController.Instance.EnemyOutOfScreen();
        }
    }
}

