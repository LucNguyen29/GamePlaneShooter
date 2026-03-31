using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject joystickPrefab;
    [SerializeField] private GameObject livesBarPrefab;
    private LiveBarController currentLivesBar;
    private GameObject currentPlayer;
    private FixedJoystick currentJoystick;

    [SerializeField] private GameObject[] allPlayers;

    public static GameController Instance;
    public int numberOfPlay = 3;
    protected bool isGameOver = false;
    protected bool isPlayerWin = false;
    [SerializeField] protected int enemyTotalCount;
    protected int enemyRemaining;
    [SerializeField] protected EnemyData[] enemyTanks;
    [SerializeField] protected EnemyData[] enemyDont;
    [SerializeField] protected EnemyData[] enemyLaser;
    [SerializeField] protected EnemyData[] enemyRush;
    [SerializeField] protected EnemyData[] enemyParticle;
    [SerializeField] protected GameObject boss;
    [SerializeField] protected Transform sqawnBoss;
    protected EnemyData[] totalEnemy;

    [SerializeField] private GameObject endPanel;
    [SerializeField] protected GameObject star_3_panel;
    [SerializeField] protected GameObject star_2_panel_killed_0;
    [SerializeField] protected GameObject star_2_panel_alives_3;
    [SerializeField] protected GameObject star_1_panel;
    [SerializeField] protected GameObject lose_panel;
    protected int star;

    void Awake()
    {
        Instance = this;
    }

    public virtual void EnemyDestroyedByBullet() { }

    public virtual void EnemyOutOfScreen() { }

    public void PlayerDie()
    {
        numberOfPlay--;
        if (currentLivesBar != null)
        {
            currentLivesBar.UpdateLives(numberOfPlay);
        }
        GameOver();
    }

    public void PlayerRevived()
    {
        if (numberOfPlay > 0 && !isGameOver)
        {
            Debug.Log("Player được hồi sinh!");

            SpawnPlayerWithJoystick();
        }
    }

    public IEnumerator DeplayRevived()
    {
        Debug.Log("Đang chờ hồi sinh...");

        yield return new WaitForSeconds(2.0f);

        if (!isGameOver && numberOfPlay > 0)
        {
            Debug.Log("Gọi hàm hồi sinh");
            SpawnPlayerWithJoystick();

        }
        else
        {
            Debug.Log("Không hồi sinh vì Game Over");
        }

    }

    IEnumerator EnableInvincibility(PlayerController player)
    {
        if (player == null) yield break;

        player.isInvincible = true;
        Debug.Log("Bắt đầu bất tử");

        if (player.shiled != null)
            player.shiled.SetActive(true);

        yield return new WaitForSeconds(5f);

        if (player == null) yield break;

        player.isInvincible = false;
        Debug.Log("Hết bất tử");

        if (player.shiled != null)
            player.shiled.SetActive(false);
    }

    public void GameOver()
    {
        if (numberOfPlay <= 0 && !isGameOver)
        {
            isGameOver = true;
            Debug.Log("Game over");

            Lose();
        }
    }
    protected void SpawnPlayerWithJoystick()
    {
        if (currentJoystick != null)
        {
            Destroy(currentJoystick.gameObject);
            currentJoystick = null;
        }

        GameObject joystickGO = Instantiate(joystickPrefab, FindObjectOfType<Canvas>().transform);
        currentJoystick = joystickGO.GetComponent<FixedJoystick>();
        int selectedIndex = PlayerPrefs.GetInt("SelectedPlayer", 0); // Lấy player đã chọn
        GameObject selectedPlayer = allPlayers[selectedIndex]; // Chọn player tương ứng

        currentPlayer = Instantiate(selectedPlayer, new Vector3(0, -1.5f, 0), Quaternion.identity);

        PlayerController controller = currentPlayer.GetComponent<PlayerController>();
        if (controller == null)
        {
            Debug.LogError("Không tìm thấy PlayerController trong prefab player!");
            return;
        }
        controller.joystick = currentJoystick;

        if (currentLivesBar == null)
        {
            GameObject livesUI = Instantiate(livesBarPrefab, FindObjectOfType<Canvas>().transform);
            currentLivesBar = livesUI.GetComponent<LiveBarController>();
        }

        // Cập nhật số mạng còn lại lên UI
        currentLivesBar.UpdateLives(numberOfPlay);

        StartCoroutine(EnableInvincibility(controller));

    }


    public virtual void CheckPlayerWin()
    {

    }

    public virtual void BossDie() { }

    public void star_3()
    {
        endPanel.SetActive(true);
        star_3_panel.SetActive(true);
        SaveStarIfHigher(3);
    }
    public void star_2_killed()
    {
        endPanel.SetActive(true);
        star_2_panel_killed_0.SetActive(true);
        SaveStarIfHigher(2);
    }
    public void star_2_alives()
    {
        endPanel.SetActive(true);
        star_2_panel_alives_3.SetActive(true);
        SaveStarIfHigher(2);
    }
    public void star_1()
    {
        endPanel.SetActive(true);
        star_1_panel.SetActive(true);
        SaveStarIfHigher(1);
    }

    public void Lose()
    {
        endPanel.SetActive(true);
        lose_panel.SetActive(true);
    }

    public void SaveStarIfHigher(int newStar)
    {
        string selectedMapName = PlayerPrefs.GetString("SelectedMap", "Map_1");
        int mapIndex = int.Parse(selectedMapName.Split('_')[1]);

        string key = $"Map{mapIndex}_Star";
        int currentStar = PlayerPrefs.GetInt(key, 0);

        Debug.Log($"Hiện tại số sao của Map{mapIndex}: {currentStar}");

        if (newStar > currentStar)
        {
            PlayerPrefs.SetInt(key, newStar);
            PlayerPrefs.Save();
            Debug.Log($"Đã lưu {newStar} sao cho Map {mapIndex}");
        }
    }
}
