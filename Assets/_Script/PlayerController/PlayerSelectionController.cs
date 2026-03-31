using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSelectionController : MonoBehaviour
{
    public GameObject[] playerOptions;
    private int currentIndex = 0;
    private GameObject currentPlayer;
    [SerializeField] private AudioClip gameBGM;


    // Start is called before the first frame update
    public void NextPlayer()
    {
        currentIndex = (currentIndex + 1) % playerOptions.Length;
        UpdatePlayerDisplay();
    }

    public void PreviousPlayer()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = playerOptions.Length - 1;
        UpdatePlayerDisplay();
    }

    void UpdatePlayerDisplay()
    {

        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
        }

        currentPlayer = Instantiate(playerOptions[currentIndex], new Vector2(0, 0.35f), Quaternion.identity);

        PlayerController controller = currentPlayer.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.isDemo = true;
        }
    }

    public void ConfirmSelection()
    {
        PlayerPrefs.SetInt("SelectedPlayer", currentIndex);

        string selectedMap = PlayerPrefs.GetString("SelectedMap");
        if (!string.IsNullOrEmpty(selectedMap))
        {
            if (Music.Instance != null)
            {
                Music.Instance.PlayClip(gameBGM); // hoặc nhạc tùy scene
            }

            SceneManager.LoadScene(selectedMap); // Load lại scene chơi đã chọn
        }
        else
        {
            Debug.LogError("Không tìm thấy map đã chọn!");
        }
    }

    void Start()
    {
        currentIndex = PlayerPrefs.GetInt("SelectedPlayer", 0);

        UpdatePlayerDisplay();
    }

    public void LoadMusicGame()
    {
        if (Music.Instance != null)
        {
            Music.Instance.PlayClip(gameBGM);
        }
    }
}
