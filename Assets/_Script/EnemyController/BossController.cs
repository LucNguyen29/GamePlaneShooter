
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class BossController : EnemyController
{
    [SerializeField] private Transform[] attackPos;
    [SerializeField] private BulletData[] bullets;
    private Transform player;
    private int laserCount = 5;
    // private int currentBulletIndex = 0;
    private float spreadAngle = 40.0f;
    [SerializeField] float spaceTime = 0.0f;
    [SerializeField] private float spaceTime_Move = 0.0f;
    private int moveState = 0;
    private bool isCreateLight = false;
    private Coroutine currentFireCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        SetupHealthBoss(1000.0f);
        player = GameObject.FindGameObjectWithTag("Plane")?.transform;

        StartCoroutine(AutoFire());

    }

    // Update is called once per frame
    void Update()
    {
        spaceTime += Time.deltaTime;
        spaceTime_Move += Time.deltaTime;

        BossMove();
    }

    void BossMove()
    {
        switch (moveState)
        {
            case 0:
                if (spaceTime_Move >= 10.0f)
                {
                    moveState = 1;
                    spaceTime_Move = 0.0f;
                }
                break;
            case 1:
                transform.position += Vector3.down * Time.deltaTime * 2.0f;
                if (transform.position.y <= 0.0f)
                {
                    moveState = 2;
                    spaceTime_Move = 0.0f;
                    
                    if(!isCreateLight)
                    {
                        StartCoroutine(FireLightnight());
                    }
                }
                break;
            case 2:
                if (spaceTime_Move >= 5.0f)
                {
                    moveState = 3;
                    spaceTime_Move = 0.0f;
                }
                break;
            case 3:
                transform.position += Vector3.up * Time.deltaTime * 2.0f;
                if (transform.position.y >= 3.84f)
                {
                    moveState = 0;
                    spaceTime_Move = 0.0f;
                }
                break;
            default:
                break;
        }
    }

    IEnumerator AutoFire()
    {
        yield return new WaitForSeconds(3.0f);
        while (true)
        {
            if (currentFireCoroutine != null)
            {
                StopCoroutine(currentFireCoroutine);
            }
            spaceTime = 0f;
            currentFireCoroutine = StartCoroutine(FireWayLaser());
            yield return new WaitForSeconds(10.0f);

            if (currentFireCoroutine != null)
            {
                StopCoroutine(currentFireCoroutine);
            }
            spaceTime = 10f;
            currentFireCoroutine = StartCoroutine(FireRocket());
            yield return new WaitForSeconds(10.0f);
        }
    }

    IEnumerator FireWayLaser()
    {
        while (spaceTime < 10.0f)
        {
            CreateLaser();
            yield return new WaitForSeconds(1.0f);
        }
    }

    IEnumerator FireRocket()
    {
        while (spaceTime >= 10.0f)
        {
            CreateRocket();
            yield return new WaitForSeconds(1.0f);
        }
    }
    
    IEnumerator FireLightnight()
    {
        isCreateLight = true;
        float timer = 0.0f;
        
        while (timer <= 4.0f)
        {
            yield return new WaitForSeconds(1.0f);
            CreateLightning();
            timer += 1.0f;
        }

        isCreateLight  = false;

    }
    void CreateRocket()
    {
        //Fire Rocket 1st
        GameObject rocket1 = Instantiate(bullets[1].bulletPrefab, attackPos[0].position, transform.rotation);
        RocketController rocketController1 = rocket1.GetComponent<RocketController>();
        if (rocketController1 != null)
        {
            rocketController1.SetTarget(player.position, 1.0f);
        }

        //Fire Rocket 2rd
        GameObject rocket2 = Instantiate(bullets[1].bulletPrefab, attackPos[6].position, transform.rotation);
        RocketController rocketController2 = rocket2.GetComponent<RocketController>();
        if (rocketController2 != null)
        {
            rocketController2.SetTarget(player.position, -1.0f);
        }


    }

    void CreateLaser()
    {
        float startAngle = -spreadAngle / 2;
        float angleStep = spreadAngle / (laserCount - 1);

        for (int i = 0; i < laserCount; i++)
        {
            float angle = startAngle + angleStep * i;
            Quaternion rotation = Quaternion.Euler(-1, -1, angle - 180);
            Instantiate(bullets[0].bulletPrefab, attackPos[3].position, rotation);
        }
    }

    void CreateLightning()
    {
        GameObject lightning = Instantiate(bullets[2].bulletPrefab, attackPos[3].position, Quaternion.identity);
        float heightOffset = lightning.GetComponent<SpriteRenderer>().bounds.size.y / 2;
        lightning.transform.position -= new Vector3(0, heightOffset, 0);
    }
}
