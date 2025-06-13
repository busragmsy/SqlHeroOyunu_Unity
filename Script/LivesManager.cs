using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LivesManager : MonoBehaviour
{
    public static LivesManager Instance { get; private set; }
    public int totalLives = 3;
    private int currentLives;
    public Image[] heartImages;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindHeartImages();
        ResetLives();
    }

    private void FindHeartImages()
    {
        GameObject[] hearts = GameObject.FindGameObjectsWithTag("Heart");
        heartImages = new Image[hearts.Length];
        for (int i = 0; i < hearts.Length; i++)
        {
            heartImages[i] = hearts[i].GetComponent<Image>();
        }
    }

    public void LoseLife()
    {
        currentLives--;
        if (currentLives < 0) currentLives = 0;
        UpdateHearts();
    }

    public int GetLives() => currentLives;

    public bool IsGameOver() => currentLives <= 0;

    private void UpdateHearts()
    {
        if (heartImages == null || heartImages.Length == 0)
        {
            FindHeartImages();
        }

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] != null)
            {
                heartImages[i].sprite = (i < currentLives) ? fullHeart : emptyHeart;
            }
        }
    }

    public void ResetLives()
    {
        currentLives = totalLives;
        UpdateHearts();
    }
}
