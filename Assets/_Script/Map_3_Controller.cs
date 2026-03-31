using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Map_3_Controller : GameController
{
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private GameObject healthBarPrefab;
    private List<Transform> enemyTankList = new List<Transform>();
    private Transform enemyGroupTanks;
    private Transform enemyGroups;
    private Transform enemyWave_3;
    private Transform bossWave;
    private int row = 2;
    private int cols = 5;
    private float spacingX = 1.0f;
    private float spacingY = 0.7f;
    private float moveDistance = 3.0f;
    private float stopWay;
    private float screenHeight;
    private float screenWidth;
    private float totalHeight;
    private float totalWidth;
    private float radius = 1.5f;
    private Transform currentWave;
    private int waveIndex;
    [SerializeField] private int enemyCount_Wave;
    [SerializeField] private bool hasReachedTarget = false;
    [SerializeField] private int killed;
    [SerializeField] private bool isAllWaveSpawned = false;
    protected bool isSqawning = false;
    private bool isBossAlive = false;

    void Start()
    {
        SpawnPlayerWithJoystick();
        star = 0;
        waveIndex = 0;
        enemyCount_Wave = 0;
        enemyGroups = new GameObject("enemyGroups").transform;
        enemyGroupTanks = new GameObject("EnemyGroupTanks").transform;
        enemyWave_3 = new GameObject("EnemyWave_3").transform;
        bossWave = new GameObject("Boss").transform;
        totalEnemy = enemyDont.Concat(enemyLaser).Concat(enemyParticle).Concat(enemyRush).Concat(enemyTanks).ToArray();

        stopWay = enemyGroups.position.y - moveDistance;
        killed = 0;

        StartCoroutine(SpawnWavesSequentially());
    }

    void Update()
    {
        MoveCurrentWave();
    }

    IEnumerator SpawnWavesSequentially()
    {
        yield return new WaitForSeconds(3.0f);
        while (waveIndex < 4)
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
                currentWave = enemyGroups;
                SqawnEnemyWave_1(currentWave);
                break;
            case 1:
                currentWave = enemyGroupTanks;
                SqawnEnemyWave_2(currentWave);
                break;
            case 2:
                currentWave = enemyWave_3;
                StartCoroutine(SqawnEnemyWave_3(currentWave));
                break;
            case 3:
                currentWave = bossWave;
                SqawnBoss(currentWave);
                break;
            default:
                Debug.LogWarning("Unhandled wave: " + currentWave);
                break;
        }
    }


    void SqawnEnemyWave_1(Transform parent)
    {
        screenWidth = Camera.main.orthographicSize * 2.0f * Screen.width / Screen.height;
        screenHeight = Camera.main.orthographicSize * 2.0f * Screen.height / Screen.width;

        totalWidth = (cols - 1) * spacingX;
        totalHeight = (row - 1) * spacingY;

        float topY = Camera.main.transform.position.y + screenHeight / 2;

        // Vị trí bắt đầu spawn (căn giữa theo chiều ngang, sát cạnh trên)
        Vector2 startPosition = new Vector2(0 - totalWidth / 2, topY - 2.0f); // cach le tren

        // Spawn enemy theo hàng và cột
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                int randomValue = Random.Range(0, totalEnemy.Length);
                Vector2 spawnPos = new Vector2(startPosition.x + j * spacingX, startPosition.y - i * spacingY);
                GameObject enemy = Instantiate(totalEnemy[randomValue].enemyPrefabs, spawnPos, transform.rotation);
                enemy.transform.parent = parent;

                enemyCount_Wave++;
                enemyTotalCount++;
            }
        }
    }

    void SqawnEnemyWave_2(Transform parent)
    {
        enemyTankList.Clear();

        float topX = Camera.main.transform.position.y + screenWidth / 8;
        Vector2 startPosition = new Vector2(-3, topX + 1.0f);

        enemyCount_Wave = 0;
        for (int i = 0; i < 10; i++)
        {
            int randomValue = Random.Range(0, enemyTanks.Length);
            Vector2 spawnPos = new Vector2(startPosition.x, startPosition.y + i * 0.4f);
            GameObject enemy = Instantiate(enemyTanks[randomValue].enemyPrefabs, spawnPos, transform.rotation);

            enemy.transform.parent = parent;

            enemyTankList.Add(enemy.transform);

            enemyCount_Wave++;
            enemyTotalCount++;
        }

    }

    IEnumerator SqawnEnemyWave_3(Transform parent)
    {
        enemyCount_Wave = 10;
        for (int i = 0; i < 10; i++)
        {
            int randomValue = Random.Range(0, totalEnemy.Length);
            int randomPosX = Random.Range(-2, 3);

            GameObject enemy = Instantiate(totalEnemy[randomValue].enemyPrefabs, new Vector2(randomPosX, transform.position.y), transform.rotation);
            enemy.transform.parent = parent;

            enemyTotalCount++;
            yield return new WaitForSeconds(1f);
        }
    }

    void SqawnBoss(Transform parent)
    {
        GameObject _boss = Instantiate(boss, sqawnBoss.position, transform.rotation);
        _boss.transform.parent = parent;

        GameObject healthBar = Instantiate(healthBarPrefab, uiCanvas.transform);
        BossController bossController = _boss.GetComponent<BossController>();
        bossController.SetHealthBar(healthBar);

        isBossAlive = true;
        isAllWaveSpawned = true;
    }

    void MoveCurrentWave()
    {
        float screenHeight = Camera.main.orthographicSize * 2;
        Vector2 targetPosition = new Vector2(0, screenHeight / 3);
        float moveSpeed = 2.0f;
        float rotationSpeed = 1.5f;

        if (currentWave == enemyGroupTanks)
        {
            if (!hasReachedTarget)
            {
                for (int i = 0; i < enemyTankList.Count; i++)
                {
                    if (enemyTankList[i] == null) continue;
                    if (i == 0)
                    {
                        enemyTankList[i].position = Vector2.MoveTowards(enemyTankList[i].position, targetPosition, moveSpeed * Time.deltaTime);
                    }
                    else
                    {
                       Vector2 followTarget = enemyTankList[i - 1] != null ? enemyTankList[i - 1].position : targetPosition;
                        enemyTankList[i].position = Vector2.MoveTowards(enemyTankList[i].position, followTarget, moveSpeed * Time.deltaTime);
                    }
                }

                if (Vector2.Distance(enemyTankList[0].position, targetPosition) < 0.1f)
                {
                    hasReachedTarget = true;
                }
            }
            else
            {
                for (int i = 0; i < enemyTankList.Count; i++)
                {
                    if (enemyTankList[i] == null) continue;
                    float angleOffset = i * (Mathf.PI * 2 / enemyTankList.Count);
                    float angle = Time.time * rotationSpeed + angleOffset;

                    float x = targetPosition.x + Mathf.Cos(angle) * radius;
                    float y = targetPosition.y + Mathf.Sin(angle) * radius;
                    enemyTankList[i].position = new Vector2(x, y);
                }
            }
        }
        else if (currentWave == enemyGroups)
        {
            if (currentWave.position.y > stopWay)
            {
                currentWave.position += Vector3.down * 3 * Time.deltaTime;
            }
        }
        else if (currentWave == enemyWave_3)
        {
            // Rain wave: cho từng enemy rơi xuống
            foreach (Transform child in currentWave)
            {
                child.position += Vector3.down * 3 * Time.deltaTime;
            }
        }

    }

    public override void EnemyDestroyedByBullet()
    {
        enemyCount_Wave--;
        killed++;
    }

    public override void EnemyOutOfScreen()
    {
        enemyCount_Wave--;
    }

    private void TryCheckPlayerWin()
    {
        if (isAllWaveSpawned && enemyCount_Wave <= 0 && !isBossAlive && !isGameOver)
        {
            CheckPlayerWin();
        }
    }

    public override void CheckPlayerWin()
    {
        isPlayerWin = true;
        if (numberOfPlay == 3)
        {
            star = (killed == enemyTotalCount) ? 3 : 2;
            if (star == 3)
            {
                star_3();
            }
            else if (star == 2)
            {
                star_2_alives();
            }
        }
        else if (numberOfPlay > 0 && numberOfPlay < 3)
        {
            star = (killed == enemyTotalCount) ? 2 : 1;

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

    public override void BossDie()
    {
        isBossAlive = false;
        Debug.Log("Boss Die");
        TryCheckPlayerWin();
    }
}
