using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Linq;
using TMPro;

public class Map_2_Controller : GameController
{
    [SerializeField] private int score;
    private Transform enemyWave_1;
    private Transform enemyWave_2;
    private Transform enemyWave_2_Left;
    private Transform enemyWave_2_Right;
    private Transform enemyWave_3;
    private Transform currentWave;
    private int waveIndex;
    protected bool isSqawning = false;
    protected bool isWaveStopped = false;
    [SerializeField] private bool isAllWaveSpawned = false;
    private float moveDistance = 3.0f;
    private float stopWay;
    [SerializeField] private int enemyCount_Wave;
    [SerializeField] private TextMeshProUGUI scoreTextUI;
    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayerWithJoystick();
        star = 0;
        //wave 1

        enemyWave_1 = new GameObject("EnemyWave_1").transform;

        //wave 2
        enemyWave_2 = new GameObject("enemyWave_2").transform;
        enemyWave_2_Right = new GameObject("Enemy_Wave_2_Right").transform;
        enemyWave_2_Left = new GameObject("Enemy_Wave_2_Left").transform;

        enemyWave_2_Right.SetParent(enemyWave_2, false);
        enemyWave_2_Left.SetParent(enemyWave_2, false);

        //wave 3
        enemyWave_3 = new GameObject("enemyWave_3").transform;
        stopWay = enemyWave_3.position.y - moveDistance;
        waveIndex = 0;
        enemyTotalCount = 0;
        enemyCount_Wave = 0;
        StartCoroutine(SpawnWavesSequentially());
    }

    // Update is called once per frame
    void Update()
    {
        currentWaveMove();
    }

    IEnumerator SpawnWavesSequentially()
    {
        yield return new WaitForSeconds(3.0f);
        while (waveIndex <= 2)
        {
            SqawnWave(waveIndex);
            isSqawning = true;

            yield return new WaitUntil(() => enemyCount_Wave == 0);

            isSqawning = false;

            yield return new WaitForSeconds(3.0f);

            waveIndex++;
        }
    }

    void SqawnWave(int index)
    {
        switch (index)
        {
            case 0:
                currentWave = enemyWave_1;
                SqawnEnemyWave_1(currentWave);
                break;
            case 1:
                currentWave = enemyWave_2;
                SqawnEnemyWave_2(enemyWave_2_Right, new Vector2(3, 3)); // Nhóm 1
                SqawnEnemyWave_2(enemyWave_2_Left, new Vector2(-3, 3));  // Nhóm 2
                break;
            case 2:
                currentWave = enemyWave_3;
                StartCoroutine(SqawnEnemyWave_3(currentWave));
                break;
            default:
                Debug.LogWarning("Unhandled wave: " + currentWave);
                break;
        }
    }

    // wave 1
    void SqawnEnemyWave_1(Transform parent)
    {
        float spacingX = 1.0f;
        float spacingY = 0.7f;
        int row = 2;
        int cols = 5;
        float screenHeight = Camera.main.orthographicSize * 2.0f * Screen.height / Screen.width;

        float totalWidth = (5 - 1) * spacingX;

        float topY = Camera.main.transform.position.y + screenHeight / 2;

        // Vị trí bắt đầu spawn (căn giữa theo chiều ngang, sát cạnh trên)
        Vector2 startPosition = new Vector2(0 - totalWidth / 2, topY - 2.0f); // cach le tren

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                int randomValue = Random.Range(0, enemyTanks.Length);
                Vector2 spawnPos = new Vector2(startPosition.x + j * spacingX, startPosition.y - i * spacingY);
                GameObject enemy = Instantiate(enemyTanks[randomValue].enemyPrefabs, spawnPos, transform.rotation);
                enemy.transform.parent = parent;

                enemyCount_Wave++;
                enemyTotalCount++;
            }
        }
    }

    void Wave1_Move(Transform waveMove)
    {
        if (waveMove.position.y > stopWay)
        {
            waveMove.position += Vector3.down * 3 * Time.deltaTime;
        }
    }


    // wave 2: 2 wave xoay
    void SqawnEnemyWave_2(Transform parent, Vector2 startPosition)
    {
        EnemyData[] totalEnemyTanksAndDont = enemyDont.Concat(enemyTanks).ToArray();

        for (int i = 0; i < 6; i++)
        {
            int randomValue = Random.Range(0, totalEnemyTanksAndDont.Length);
            Vector2 spawnPos = new Vector2(startPosition.x, startPosition.y + i * 0.4f);
            GameObject enemy = Instantiate(totalEnemyTanksAndDont[randomValue].enemyPrefabs, spawnPos, transform.rotation);

            enemy.transform.parent = parent;
            enemyCount_Wave++;
            enemyTotalCount++;
        }


    }

    void Wave2_Move(Transform waveMove, Vector3 direction, Vector2 stopPosition, float PosX)
    {
        float screenHeight = Camera.main.orthographicSize * 2;
        Vector2 targetPosition = new Vector2(PosX, screenHeight / 3);
        float rotationSpeed = 1.5f;
        float radius = 1.0f;

        if (!isWaveStopped)
        {
            for (int i = 0; i < waveMove.childCount; i++)
            {
                if (i == 0)
                {
                    waveMove.GetChild(i).transform.position += direction * Time.deltaTime * 2.0f;
                    if ((direction.x > 0 && waveMove.GetChild(i).position.x >= stopPosition.x) ||
                        (direction.x < 0 && waveMove.GetChild(i).position.x <= stopPosition.x))
                    {
                        isWaveStopped = true;
                        break;
                    }
                }
                else
                {
                    Vector2 followTarget = waveMove.GetChild(i - 1).position;
                    waveMove.GetChild(i).position = Vector2.MoveTowards(waveMove.GetChild(i).position, followTarget, Time.deltaTime * 2.0f);
                }
            }
        }
        else
        {
            for (int i = 0; i < waveMove.childCount; i++)
            {
                if (waveMove.GetChild(i) == null) continue;
                float angleOffset = i * (Mathf.PI * 2 / waveMove.childCount);
                float angle = Time.time * rotationSpeed + angleOffset;

                float x = targetPosition.x + Mathf.Cos(angle) * radius;
                float y = targetPosition.y + Mathf.Sin(angle) * radius;
                waveMove.GetChild(i).position = new Vector2(x, y);
            }
        }
    }


    // wave 3: 10 enemy rush to player

    IEnumerator SqawnEnemyWave_3(Transform parent)
    {

        for (int i = 0; i < 10; i++)
        {
            int randomValue = Random.Range(0, enemyRush.Length);
            int randomPosX = Random.Range(-2, 3);

            GameObject enemy = Instantiate(enemyRush[randomValue].enemyPrefabs, new Vector2(randomPosX, transform.position.y), transform.rotation);
            enemy.transform.parent = parent;

            enemyCount_Wave++;
            enemyTotalCount++;
            yield return new WaitForSeconds(2.0f);
        }

        isAllWaveSpawned = true;
        Debug.Log("✅ All waves have been spawned!");
    }

    void Wave3_Move(Transform moveWave)
    {
        foreach (Transform child in moveWave)
        {
            child.position += Vector3.down * 1f * Time.deltaTime;
        }

    }

    // current wave move
    void currentWaveMove()
    {
        if (currentWave == enemyWave_1)
        {
            Wave1_Move(currentWave);
        }
        else if (currentWave == enemyWave_2)
        {
            Wave2_Move(enemyWave_2_Left, Vector3.right, new Vector2(2, 3), 1.5f);
            Wave2_Move(enemyWave_2_Right, Vector3.left, new Vector2(-2, 3), -1.5f);
        }
        else if (currentWave == enemyWave_3)
        {
            Wave3_Move(currentWave);
        }
    }

    public override void EnemyDestroyedByBullet()
    {
        base.EnemyDestroyedByBullet();
        enemyCount_Wave--;
        score++;
        UpdateScoreUI();
        TryCheckPlayerWin();
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
        enemyCount_Wave--;
        TryCheckPlayerWin();
    }

    public override void CheckPlayerWin()
    {
        if (score >= 15)
        {
            isPlayerWin = true;

            if (numberOfPlay == 3)
            {
                star = (score == enemyTotalCount) ? 3 : 2;

                if (star == 3)
                {
                    star_3();
                }
                else
                {
                    star_2_alives();
                }
            }
            else if (numberOfPlay < 3 && numberOfPlay > 0)
            {
                star = (score == enemyTotalCount) ? 2 : 1;
                if (star == 2)
                {
                    star_2_killed();
                }
                else
                {
                    star_1();
                }
            }

            Debug.Log($"{star} sao");
        }
        else
        {
            isPlayerWin = false;
            Lose();
            Debug.Log("Player thua");
        }
    }

    private void TryCheckPlayerWin()
    {
        if (isAllWaveSpawned && enemyCount_Wave == 0 && !isGameOver)
        {
            Debug.Log("Player win");
            CheckPlayerWin();
        }
    }
}
