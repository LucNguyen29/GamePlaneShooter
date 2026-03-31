using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Boss_2_Controller : EnemyController
{
    [SerializeField] private Transform[] attackPos;
    [SerializeField] private BulletData[] bullets;
    private enum BossState
    {
        Right,
        Left,
        Up,
        Down,
        Idle
    }
    private Transform player;
    private bool stopped = false;
    private float moveSpeed = 2.0f;
    private float delayDuration = 5.0f;
    private bool isCreateLight = false;
    private Coroutine currentFireCoroutine = null;

    private BossState[] movePattern = new BossState[]
    {
        BossState.Left, BossState.Down, BossState.Right, BossState.Idle,
        BossState.Right, BossState.Up, BossState.Left, BossState.Idle
    };

    private int currentStepIndex = 0;
    private float delayTimer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        SetupHealthBoss(1500.0f);
        player = GameObject.FindGameObjectWithTag("Plane")?.transform;
    }

    // Update is called once per frame
    void Update()
    {
        BossState currentState = movePattern[currentStepIndex];

        if (currentState == BossState.Idle)
        {
            HandleIdle();
        }
        else
        {
            Move(GetDirection(currentState), GetLimit(currentState), GetAxis(currentState));
        }
    }

    IEnumerator AutoFire(System.Action fireMethod)
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            fireMethod();
        }
    }

    IEnumerator FireLightning()
    {
        isCreateLight = true;
        float timer = 0f;
        while (timer < delayDuration)
        {
            yield return new WaitForSeconds(1.0f);
            CreateLightning();

            timer += 1.0f;
        }

        isCreateLight = false;
    }

    void CreateBomb_1()
    {
        Instantiate(bullets[1].bulletPrefab, attackPos[3].position, Quaternion.identity);
        Instantiate(bullets[1].bulletPrefab, attackPos[2].position, Quaternion.identity);
    }

    void CreateBomb_2()
    {
        Instantiate(bullets[2].bulletPrefab, attackPos[0].position, Quaternion.identity);
        Instantiate(bullets[2].bulletPrefab, attackPos[1].position, Quaternion.identity);
    }

    void CreateBomb_3()
    {
        Instantiate(bullets[3].bulletPrefab, attackPos[4].position, Quaternion.identity);
        Instantiate(bullets[3].bulletPrefab, attackPos[5].position, Quaternion.identity);
    }

    void CreateLightning()
    {
        GameObject lightning = Instantiate(bullets[0].bulletPrefab, attackPos[3].position, Quaternion.identity);
        float heightOffset = lightning.GetComponent<SpriteRenderer>().bounds.size.y / 2;
        lightning.transform.position -= new Vector3(0, heightOffset, 0);

        GameObject lightning_1 = Instantiate(bullets[0].bulletPrefab, attackPos[2].position, Quaternion.identity);
        float heightOffset_1 = lightning_1.GetComponent<SpriteRenderer>().bounds.size.y / 2;
        lightning_1.transform.position -= new Vector3(0, heightOffset_1, 0);
    }

    void HandleIdle()
    {
        delayTimer += Time.deltaTime;
        if (delayTimer >= delayDuration)
        {
            delayTimer = 0f;
            AdvanceToNextStep();
        }
    }

    enum Axis { X, Y }

    void Move(Vector3 dir, float limit, Axis axis)
    {
        if (stopped)
        {
            delayTimer += Time.deltaTime;
            if (delayTimer >= delayDuration)
            {
                delayTimer = 0f;
                stopped = false;
                AdvanceToNextStep();
            }
            return;
        }

        transform.position += dir * moveSpeed * Time.deltaTime;

        bool reached = false;
        if (axis == Axis.X)
            reached = dir.x < 0 ? transform.position.x <= limit : transform.position.x >= limit;
        else
            reached = dir.y < 0 ? transform.position.y <= limit : transform.position.y >= limit;

        if (reached)
        {
            // Snap đúng vị trí
            transform.position = new Vector3(
                axis == Axis.X ? limit : transform.position.x,
                axis == Axis.Y ? limit : transform.position.y,
                transform.position.z
            );

            stopped = true;

            if (!isCreateLight)
            {
                StartCoroutine(FireLightning());
            }
        }
    }

    void AdvanceToNextStep()
    {
        currentStepIndex++;
        if (currentStepIndex >= movePattern.Length)
        {
            currentStepIndex = 0;
        }
        BossState currentState = movePattern[currentStepIndex];

        if (currentFireCoroutine != null)
        {
            StopCoroutine(currentFireCoroutine);
        }

        // Dừng tất cả các loại đạn cũ và bắt đầu bắn đạn mới theo hướng di chuyển
        if (currentState == BossState.Left)
        {
            currentFireCoroutine = StartCoroutine(AutoFire(CreateBomb_1));  // Bắt đầu bắn đạn trái
        }
        else if (currentState == BossState.Right)
        {
            currentFireCoroutine = StartCoroutine(AutoFire(CreateBomb_2));  // Bắt đầu bắn đạn phải
        }
        else if (currentState == BossState.Up)
        {
            currentFireCoroutine = StartCoroutine(AutoFire(CreateBomb_3));  // Bắt đầu bắn đạn lên
        }
        else if (currentState == BossState.Down)
        {
            currentFireCoroutine = StartCoroutine(AutoFire(CreateBomb_3));  // Bắt đầu bắn đạn xuống
        }
    }

    Vector3 GetDirection(BossState state)
    {
        switch (state)
        {
            case BossState.Left: return Vector3.left;
            case BossState.Right: return Vector3.right;
            case BossState.Up: return Vector3.up;
            case BossState.Down: return Vector3.down;
            default: return Vector3.zero;
        }
    }

    float GetLimit(BossState state)
    {
        switch (state)
        {
            case BossState.Left: return -1.6f;
            case BossState.Right: return 1.6f;
            case BossState.Down: return 0f;
            case BossState.Up: return 3.128f;
            default: return 0f;
        }
    }

    Axis GetAxis(BossState state)
    {
        if (state == BossState.Left || state == BossState.Right) return Axis.X;
        if (state == BossState.Up || state == BossState.Down) return Axis.Y;
        return Axis.X;
    }


}
