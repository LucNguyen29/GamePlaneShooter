using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;


public class Map_4_Controller : GameController
{
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private GameObject healthBarPrefab;
    // wave 1
    private Transform enemyWave1;
    private Transform enemyWave_1_Right;
    private Transform enemyWave_1_Left;

    // wave 2
    private Transform enemyWave2;
    private Transform enemyWave_2_Right;
    private Transform enemyWave_2_Left;

    // wave 3
    private Transform enemyWave_3;

    // wave boss
    private Transform bossWave;
    private int waveIndex;
    private Transform currentWave;
    List<bool> enemyGoingLeft = new List<bool>();
    [SerializeField] private int enemyCount_Wave;
    [SerializeField] private int killed;
    private bool isBossAlive = false;
    private bool isAllWaveSpawned = false;
    protected bool isSqawning = false;
    private bool waveStoped = false;
    private bool waveStarted = false;
    private bool wave_2_Stoped = false;
    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayerWithJoystick();

        waveIndex = 0;
        star = 0;
        enemyCount_Wave = 0;
        enemyTotalCount = 0;
        killed = 0;

        // wave 1
        enemyWave1 = new GameObject("EnemyWave1").transform;
        enemyWave_1_Right = new GameObject("Enemy_Wave_1_Right").transform;
        enemyWave_1_Left = new GameObject("Enemy_Wave_1_Left").transform;

        enemyWave_1_Right.SetParent(enemyWave1, false);
        enemyWave_1_Left.SetParent(enemyWave1, false);

        // wave 2

        enemyWave2 = new GameObject("EnemyWave2").transform;
        enemyWave_2_Right = new GameObject("Enemy_Wave_2_Right").transform;
        enemyWave_2_Left = new GameObject("Enemy_Wave_2_Left").transform;

        enemyWave_2_Right.SetParent(enemyWave2, false);
        enemyWave_2_Left.SetParent(enemyWave2, false);

        // wave 3
        enemyWave_3 = new GameObject("Enemy_Wave_3").transform;

        // wave boss
        bossWave = new GameObject("BossWave").transform;

        StartCoroutine(SpawnWavesSequentially());

    }

    // Update is called once per frame
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
                currentWave = enemyWave1;
                SqawnEnemyWave_1(enemyWave_1_Right, new Vector2(3, 4)); // Nhóm 1
                SqawnEnemyWave_1(enemyWave_1_Left, new Vector2(-3, 3));  // Nhóm 2
                break;
            case 1:
                currentWave = enemyWave2;
                SqawnEnemyWave_2(enemyWave_2_Right, new Vector2(3.1f, 3));
                SqawnEnemyWave_2(enemyWave_2_Left, new Vector2(-3.7f, 3));

                StartCoroutine(DeplayWave(1.5f));
                break;
            case 2:
                currentWave = enemyWave_3;
                SqawnEnemyWave_3(currentWave);
                break;
            case 3:
                currentWave = bossWave;
                SqawnBossWave(currentWave);
                break;
            default:
                Debug.LogWarning("Unhandled wave: " + currentWave);
                break;
        }
    }
    void MoveCurrentWave()
    {
        if (currentWave == enemyWave1)
        {
            MoveWave_1(enemyWave_1_Left, Vector3.right, new Vector2(-2, 3));
            MoveWave_1(enemyWave_1_Right, Vector3.left, new Vector2(-2, 4));
        }
        else if (currentWave == enemyWave2)
        {
            int[][] indexGroupWaveLeft = new int[][]
            {
                 new int[]{2 ,5},
                 new int[]{1, 4},
                 new int[]{0, 3}

            };
            MoveWave_2(enemyWave_2_Left, Vector3.right, new Vector2(-0.5f, 3), indexGroupWaveLeft);

            int[][] indexGroupWaveRight = new int[][]
            {
                new int[]{0 ,3},
                new int[]{1,4},
                new int[]{2, 5}
            };

            MoveWave_2(enemyWave_2_Right, Vector3.left, new Vector2(0.5f, 3), indexGroupWaveRight);
        }
        else if (currentWave == enemyWave_3)
        {
            Wave_3_Action(currentWave);
        }
    }

    // -----------------------//
    // move wave 1

    void SqawnEnemyWave_1(Transform parent, Vector2 startPosition)
    {
        for (int i = 0; i < 5; i++)
        {
            int randomValue = Random.Range(0, enemyTanks.Length);
            Vector2 spawnPos = new Vector2(startPosition.x, startPosition.y + i * 0.4f);
            GameObject enemy = Instantiate(enemyTanks[randomValue].enemyPrefabs, spawnPos, transform.rotation);

            enemy.transform.parent = parent;

            enemyCount_Wave++;
            enemyTotalCount++;
        }
    }

    void MoveWave_1(Transform waveMove, Vector3 direction, Vector2 stopPosition)
    {
        if (!waveStoped)
        {
            for (int i = 0; i < waveMove.childCount; i++)
            {

                if (i == 0)
                {
                    waveMove.GetChild(i).Translate(direction * Time.deltaTime * 2.0f);
                    if (waveMove.GetChild(i).position.x <= stopPosition.x)
                    {
                        waveStoped = true;
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
            float spacing = 1.0f;
            for (int i = 0; i < waveMove.childCount; i++)
            {
                Vector2 targetPos = new Vector2(stopPosition.x + i * spacing, stopPosition.y);
                Transform enemy = waveMove.GetChild(i);
                enemy.position = Vector2.MoveTowards(enemy.position, targetPos, Time.deltaTime * 2.0f);
            }
        }
    }

    // -----------------------//
    // wave 2

    void SqawnEnemyWave_2(Transform parent, Vector2 startPosition)
    {
        EnemyData[] totalDontAndTanks = enemyTanks.Concat(enemyDont).ToArray();

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int randomValue = Random.Range(0, totalDontAndTanks.Length);
                Vector2 spawnPos = new Vector2(startPosition.x + j * 0.2f, startPosition.y - i * 0.7f);
                GameObject enemy = Instantiate(totalDontAndTanks[randomValue].enemyPrefabs, spawnPos, Quaternion.identity);
                enemy.transform.rotation = Quaternion.Euler(180, 0, 0f);
                enemy.transform.parent = parent;

                enemyCount_Wave++;
                enemyTotalCount++;
            }
        }
    }

    void MoveWave_2(Transform waveMove, Vector3 direction, Vector2 stopPosition, int[][] groups)
    {
        if (waveStarted && !wave_2_Stoped)
        {
            StartCoroutine(WaveMoveCoroutine_2(waveMove, direction, stopPosition, groups));
        }

    }

    IEnumerator WaveMoveCoroutine_2(Transform waveMove, Vector3 direction, Vector2 stopPosition, int[][] groups)
    {
        Vector2 currentStopPos = stopPosition;

        for (int i = 0; i < groups.Length; i++)
        {
            int[] group = groups[i];

            while (!TwoEnemiesMove(waveMove, group, direction, currentStopPos))
            {
                yield return null;
            }

            yield return new WaitForSeconds(1.0f);

            if (direction.x > 0)
            {
                currentStopPos.x -= 1.0f;
            }
            else if (direction.x < 0)
            {
                currentStopPos.x += 1.0f;
            }
        }
        wave_2_Stoped = true;
    }

    bool TwoEnemiesMove(Transform waveMove, int[] indices, Vector3 direction, Vector2 stopPosition)
    {
        bool allReached = true;

        foreach (int i in indices)
        {
            if (i < 0 || i >= waveMove.childCount)
            {
                Debug.LogWarning($"[TwoEnemiesMove] Enemy at index {i} is missing. Skipping movement for this group.");
                return true;
            }

            Transform enemy = waveMove.GetChild(i);

            bool rechead = false;

            if (direction.x < 0)
            {
                if (enemy.position.x > stopPosition.x)
                {
                    enemy.Translate(direction * Time.deltaTime * 1.0f);
                }
                else
                {
                    Vector3 stopPos = new Vector3(stopPosition.x, enemy.position.y, enemy.position.z);
                    enemy.position = stopPos;
                    rechead = true;
                }
            }

            if (direction.x > 0)
            {
                if (enemy.position.x < stopPosition.x)
                {
                    enemy.Translate(direction * Time.deltaTime * 1.0f);
                }
                else
                {
                    Vector3 stopPos = new Vector3(stopPosition.x, enemy.position.y, enemy.position.z);
                    enemy.position = stopPos;
                    rechead = true;
                }
            }

            if (!rechead)
                allReached = false;
        }

        return allReached;
    }

    IEnumerator DeplayWave(float time)
    {
        yield return new WaitForSeconds(time);
        waveStarted = true;
    }
    // -----------------------//
    // wave 3
    void SqawnEnemyWave_3(Transform parent)
    {
        List<Vector2> possiblePositions = new List<Vector2>();
        for (int x = -2; x <= 2; x++)
        {
            for (int y = 2; y <= 4; y++)
            {
                possiblePositions.Add(new Vector2(x, y));
            }
        }

        Shuffle(possiblePositions);

        EnemyData[] totalParticelAndLaser = enemyParticle.Concat(enemyLaser).ToArray();
        for (int i = 0; i < 6; i++)
        {
            int randomValue = Random.Range(0, totalParticelAndLaser.Length);
            Vector2 sqawnPos = possiblePositions[i];
            GameObject enemy = Instantiate(totalParticelAndLaser[randomValue].enemyPrefabs, sqawnPos, transform.rotation);

            enemy.transform.parent = parent;

            enemyCount_Wave++;
            enemyTotalCount++;
        }
        enemyGoingLeft.Clear();

        for (int i = parent.childCount - 6; i < parent.childCount; i++)
        {
            Transform enemy = parent.GetChild(i);
            bool isGoingLeft = enemy.position.x >= 0;
            enemyGoingLeft.Add(isGoingLeft);
            Debug.Log($"Enemy {enemy.name} - PosX: {enemy.position.x:F2} => isGoingLeft: {isGoingLeft}");
        }
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;

        }
    }

    void Wave_3_Action(Transform waveAction)
    {
        float speed = 2.0f;
        float delta = Time.deltaTime * speed;
        float leftLimit = -2.6f;
        float rightLimit = 2.6f;

        for (int i = 0; i < waveAction.childCount; i++)
        {
            Transform enemy = waveAction.GetChild(i);
            Vector3 pos = enemy.position;

            if (enemyGoingLeft[i])
            {
                if (pos.x - delta <= leftLimit)
                {
                    pos.x = leftLimit;
                    enemyGoingLeft[i] = false;
                }
                else
                {
                    pos.x -= delta;
                }
            }
            else
            {
                if (pos.x + delta >= rightLimit)
                {
                    pos.x = rightLimit;
                    enemyGoingLeft[i] = true;
                }
                else
                {
                    pos.x += delta;
                }
            }

            enemy.position = pos;
        }
    }

    // boss

    void SqawnBossWave(Transform parent)
    {
        GameObject _boss = Instantiate(boss, sqawnBoss.position, Quaternion.identity);
        _boss.transform.parent = parent;
        GameObject healthBar = Instantiate(healthBarPrefab, uiCanvas.transform);
        Boss_2_Controller bossController = _boss.GetComponent<Boss_2_Controller>();
        bossController.SetHealthBar(healthBar);

        isBossAlive = true;
        isAllWaveSpawned = true;
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
        Debug.Log($"[CheckWin] isAllWaveSpawned: {isAllWaveSpawned}, enemyCount_Wave: {enemyCount_Wave}, isBossAlive: {isBossAlive}, isGameOver: {isGameOver}");
        if (isAllWaveSpawned && enemyCount_Wave <= 0 && !isBossAlive && !isGameOver)
        {
            Debug.Log("Check sao");
            CheckPlayerWin();
        }
    }

    public override void BossDie()
    {
        isBossAlive = false;
        Debug.Log("Boss die");
        TryCheckPlayerWin();
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

}



