using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using TMPro;

public class Map_1_Controller : GameController
{
    private int maxScore;
    private int miniumScore;
    private Transform enemyWave;
    [SerializeField] protected int score;

    [SerializeField] private TextMeshProUGUI scoreTextUI;

    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayerWithJoystick();
        star = 0;
        maxScore = 20;
        miniumScore = 15;
        enemyWave = new GameObject("EnemyWave").transform;
        totalEnemy = enemyDont.Concat(enemyLaser).Concat(enemyParticle).Concat(enemyRush).Concat(enemyTanks).ToArray();

        StartCoroutine(SqawnEnemyWave(enemyWave));
        UpdateScoreUI();
    }

    // Update is called once per frame
    void Update()
    {
        EnemyWaveMove();
    }

    IEnumerator SqawnEnemyWave(Transform parent)
    {
        enemyTotalCount = 20;
        yield return new WaitForSeconds(3.0f);
        for (int i = 0; i < 20; i++)
        {
            int randomValue = Random.Range(0, totalEnemy.Length);
            int randomPosX = Random.Range(-2, 3);

            GameObject enemy = Instantiate(totalEnemy[randomValue].enemyPrefabs, new Vector2(randomPosX, transform.position.y), transform.rotation);
            enemy.transform.parent = parent;

            yield return new WaitForSeconds(2.5f);
        }
    }

    void EnemyWaveMove()
    {
        foreach (Transform child in enemyWave)
        {
            child.position += Vector3.down * 1.5f * Time.deltaTime;
        }
    }

    public override void EnemyDestroyedByBullet()
    {
        base.EnemyDestroyedByBullet();
        enemyTotalCount--;
        score++;
        UpdateScoreUI();

        if (enemyTotalCount == 0 && !isGameOver)
        {
            CheckPlayerWin();
        }

    }

    private void UpdateScoreUI()
    {
        if (scoreTextUI != null)
        {
            scoreTextUI.text = "Score: " + score.ToString();
        }
    }

    public override void EnemyOutOfScreen()
    {
        base.EnemyOutOfScreen();
        enemyTotalCount--;
    }


    public override void CheckPlayerWin()
    {
        isPlayerWin = true;

        if (score >= miniumScore)
        {
            if (numberOfPlay == 3)
            {
                star = (score == maxScore) ? 3 : 1;
                if(star == 3)
                {
                    star_3();
                }
                else
                {
                    star_1();
                }
            }
            else if (numberOfPlay < 3 && numberOfPlay > 0)
            {
                star = (score == maxScore) ? 2 : 1;

                if(star == 2)
                {
                    star_2_killed();
                }
                else
                {
                    star_1();
                }
            }
        }
        else
        {
            isPlayerWin = false;

            Debug.Log("Player thua");
            Lose();
        }
    }
}

