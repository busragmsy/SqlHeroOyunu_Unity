using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;  
    public Button[] levelButtons;  
    private int currentLevel = 1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  

        }
        else
        {
            Destroy(gameObject); 
        }
    }

    void Start()
    {
        PlayerPrefs.DeleteAll();
        LoadLevelProgress();

        StartCoroutine(InitializeLevelsAfterFrame());
    }

    private void FindLevelButtonsInScene()
    {
        LevelButton[] levelObjs = GameObject.FindObjectsOfType<LevelButton>();
        levelButtons = new Button[levelObjs.Length];

        for (int i = 0; i < levelObjs.Length; i++)
        {
            levelButtons[i] = levelObjs[i].GetComponent<Button>();
        }
    }

    private IEnumerator InitializeLevelsAfterFrame()
    {
        yield return new WaitForEndOfFrame();
        FindLevelButtonsInScene();
        UnlockLevels();
    }

    public void LoadLevelProgress()
    {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
    }

    public void UnlockLevels()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            Button btn = levelButtons[i];
            if (btn != null)
            {
                LevelButton levelBtn = btn.GetComponent<LevelButton>();
                if (levelBtn != null)
                {
                    btn.interactable = (levelBtn.levelId <= currentLevel);
                }
            }
        }
    }

    public void LevelCompleted()
    {
        if (currentLevel < levelButtons.Length)
        {
            currentLevel++;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
            PlayerPrefs.Save();
            UnlockLevels();
        }
    }

    public void GoToMainMenu()
    {
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        PlayerPrefs.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelScene");
        
    }

    public void RefreshLevelButtons()
    {
        StartCoroutine(InitializeLevelsAfterFrame());
    }
}
