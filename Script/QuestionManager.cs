using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class QuestionManager : MonoBehaviour
{
    private int correctCount = 0;
    private int wrongCount = 0;

    [Header("Genel Ayarlar")]
    public TextAsset jsonFile;
    private List<Question> allQuestions;
    private Question currentQuestion;
    private int currentQuestionIndex = 0;

    private List<string> selectedOrder = new List<string>();
    private List<string> correctOrder = new List<string>();

    [Header("True/False Panel")]
    public GameObject panelTrueFalse;
    public TextMeshProUGUI tfQuestionText;
    public Button trueButton;
    public Button falseButton;

    [Header("Multiple Choice Panel")]
    public GameObject panelMultipleChoice;
    public TextMeshProUGUI mcQuestionText;
    public List<Button> mcButtons;

    [Header("Order Words Panel")]
    public GameObject panelOrderWords;
    public TextMeshProUGUI owQuestionText;
    public Transform choicesArea;
    public Transform selectedArea;
    public GameObject wordButtonPrefab;
    public Button kontrolEtButton;
    public TextMeshProUGUI resultText;

    [Header("Geri Bildirim Panelleri")]
    public GameObject panelCorrect;
    public GameObject panelWrong;
    public TextMeshProUGUI trueAnswerText;

    public SoundManager soundManagerTrue;
    public SoundManager soundManagerFalse;

    [Header("Level Bitti Paneli")]
    public GameObject panelCongrats;

    [Header("Lives Panel")]
    public GameObject livesPanel;

    [Header("Game Over Paneli")]
    public GameObject panelGameOver;

    public TextMeshProUGUI timerText;
    [Header("Zamanlayıcı Ayarları")]
    public float questionTimeLimit = 10f;
    private float timeLeft;
    private bool isTimerRunning;
    private float bonusTimeFromPreviousQuestion = 0f; // Önceki sorudan kalan bonus süre

    private LevelManager levelManager;

    [Header("Sonuç Paneli")]
    public TextMeshProUGUI resultCountText;

    public TextMeshProUGUI gameOverCountText;

    public ExitConfirmationManager exitConfirmationManager;

    public void OnExitButtonClicked()
    {
        exitConfirmationManager.ShowExitConfirmationPanel();
    }

    void Start()
    {
        LivesManager.Instance.ResetLives();
        levelManager = LevelManager.Instance;
        LoadLevelQuestions();
        StartNewQuestion();
    }

    void Update()
    {
        if (isTimerRunning)
        {
            timeLeft -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timeLeft).ToString();

            if (timeLeft <= 0)
            {
                isTimerRunning = false;
                HandleWrongAnswer("Zaman doldu!");
            }
        }
    }

    void StartNewQuestion()
    {
        // Her yeni soruda temel süreyi ayarla ve bonus süreyi ekle
        timeLeft = questionTimeLimit + bonusTimeFromPreviousQuestion;
        bonusTimeFromPreviousQuestion = 0f; // Bonus süreyi kullandık, sıfırla
        isTimerRunning = true;

        ShowCurrentQuestion();
    }

    void LoadLevelQuestions()
    {
        QuestionData allData = JsonUtility.FromJson<QuestionData>(jsonFile.text);
        int levelId = GameData.selectedLevelId;

        Level level = allData.levels.Find(l => l.levelId == levelId);

        if (level != null)
        {
            allQuestions = level.questions;
            currentQuestionIndex = 0;
        }
        else
        {
            Debug.LogError("Seçilen levelId'ye ait veri bulunamadı!");
        }
    }

    void ShowCongratsPanel()
    {
        isTimerRunning = false; // Zamanlayıcıyı durdur
        HideAllPanels();
        panelCongrats.SetActive(true);
        livesPanel.SetActive(false);
        timerText.gameObject.SetActive(false); // Süreyi gizle
        resultCountText.text = "Doğru: " + correctCount + "\nYanlış: " + wrongCount;

        // Seviye tamamlandı - level manager'a bildir
        if (levelManager != null)
        {
            levelManager.LevelCompleted();
        }

        // Button'un aktif olup olmadığını kontrol et
        Button backToMenuButton = panelCongrats.GetComponentInChildren<Button>();
        if (backToMenuButton != null)
        {
            backToMenuButton.onClick.RemoveAllListeners();
            backToMenuButton.onClick.AddListener(() => {
                ReturnToMenu(); 
            });
        }
        else
        {
            Debug.LogError("Ana menüye dön butonu bulunamadı!");
        }
    }

    void ShowCurrentQuestion()
    {
        if (allQuestions == null || allQuestions.Count == 0)
            return;

        if (currentQuestionIndex >= allQuestions.Count)
        {
            ShowCongratsPanel();
            return;
        }

        currentQuestion = allQuestions[currentQuestionIndex];
        HideAllPanels();

        switch (currentQuestion.type)
        {
            case "trueFalse": ShowTrueFalse(); break;
            case "multipleChoice": ShowMultipleChoice(); break;
            case "orderWords": ShowOrderWords(); break;
        }
    }

    public void NextQuestion()
    {
        panelCorrect.SetActive(false);
        panelWrong.SetActive(false);
        livesPanel.SetActive(true);
        timerText.gameObject.SetActive(true); // Süreyi tekrar göster
        currentQuestionIndex++;

        if (currentQuestionIndex >= allQuestions.Count)
        {
            ShowCongratsPanel();
            return;
        }

        StartNewQuestion(); // Yeni soruyu başlat
    }

    void HideAllPanels()
    {
        panelTrueFalse.SetActive(false);
        panelMultipleChoice.SetActive(false);
        panelOrderWords.SetActive(false);
        panelCorrect.SetActive(false);
        panelWrong.SetActive(false);
    }

    void ShowTrueFalse()
    {
        panelTrueFalse.SetActive(true);
        tfQuestionText.text = currentQuestion.question;

        trueButton.onClick.RemoveAllListeners();
        falseButton.onClick.RemoveAllListeners();

        trueButton.onClick.AddListener(() => CheckTrueFalse("True"));
        falseButton.onClick.AddListener(() => CheckTrueFalse("False"));
    }

    void CheckTrueFalse(string selected)
    {
        string correct = currentQuestion.answer[0];
        if (selected == correct)
        {
            HandleCorrectAnswer();
        }
        else
        {
            HandleWrongAnswer("Doğru cevap: " + correct);
        }
    }

    void ShowMultipleChoice()
    {
        panelMultipleChoice.SetActive(true);
        mcQuestionText.text = currentQuestion.question;

        for (int i = 0; i < mcButtons.Count; i++)
        {
            mcButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.options[i];
            string selected = currentQuestion.options[i];

            mcButtons[i].onClick.RemoveAllListeners();
            mcButtons[i].onClick.AddListener(() => CheckMultipleChoice(selected));
        }
    }

    void CheckMultipleChoice(string selected)
    {
        string correct = currentQuestion.answer[0];
        if (selected == correct)
        {
            HandleCorrectAnswer();
        }
        else
        {
            HandleWrongAnswer("Doğru cevap: " + correct);
        }
    }

    void ShowOrderWords()
    {
        panelOrderWords.SetActive(true);
        owQuestionText.text = "Aşağıdaki SQL komutunu doğru sıralayın.";
        correctOrder = currentQuestion.answer;
        selectedOrder.Clear();

        foreach (Transform child in choicesArea) Destroy(child.gameObject);
        foreach (Transform child in selectedArea) Destroy(child.gameObject);

        foreach (string word in currentQuestion.orderWords)
        {
            GameObject btn = Instantiate(wordButtonPrefab, choicesArea);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = word;

            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                selectedOrder.Add(word);
                btn.transform.SetParent(selectedArea);
                btn.GetComponent<Button>().interactable = false;
            });
        }

        kontrolEtButton.onClick.RemoveAllListeners();
        kontrolEtButton.onClick.AddListener(CheckOrderWords);
    }

    void CheckOrderWords()
    {
        if (selectedOrder.Count != correctOrder.Count)
        {
            resultText.text = "⚠️ Tüm kelimeleri sıralayın!";
            return;
        }

        for (int i = 0; i < correctOrder.Count; i++)
        {
            if (selectedOrder[i] != correctOrder[i])
            {
                HandleWrongAnswer("Doğru cevap: " + string.Join(" ", correctOrder));
                return;
            }
        }

        HandleCorrectAnswer();
    }

    void HandleCorrectAnswer()
    {
        correctCount++;

        isTimerRunning = false; // Zamanlayıcıyı durdur
        soundManagerTrue.PlayCorrectSound();

        // Kalan süreyi bonus olarak kaydet
        if (timeLeft > 0)
        {
            bonusTimeFromPreviousQuestion = timeLeft;
        }

        ShowCorrectPanel();
    }
    
    void ShowCorrectPanel()
    {
        HideAllPanels();
        livesPanel.SetActive(false);
        panelCorrect.SetActive(true);
        timerText.gameObject.SetActive(false); // Süreyi gizle
    }

    void HandleWrongAnswer(string correctAnswer)
    {
        wrongCount++;

        isTimerRunning = false; // Zamanlayıcıyı durdur
        LivesManager.Instance.LoseLife();
        soundManagerFalse.PlayWrongSound();

        HideAllPanels();
        livesPanel.SetActive(false);
        panelWrong.SetActive(true);
        trueAnswerText.text = correctAnswer;
        timerText.gameObject.SetActive(false); // Süreyi gizle

        // Yanlış cevap durumunda bonus süre sıfırlanır
        bonusTimeFromPreviousQuestion = 0f;

        if (LivesManager.Instance.IsGameOver())
        {
            Invoke("ShowGameOverPanel", 1.5f);
        }
    }

    void ShowGameOverPanel()
    {
        panelWrong.SetActive(false);
        panelTrueFalse.SetActive(false);
        panelMultipleChoice.SetActive(false);
        panelOrderWords.SetActive(false);
        panelGameOver.SetActive(true);
        timerText.gameObject.SetActive(false); // Süreyi gizle

        gameOverCountText.text = "Doğru: " + correctCount + "\nYanlış: " + wrongCount;
    }

    public void ReturnToMenu()
    {
        LivesManager.Instance.ResetLives();
        levelManager.BroadcastMessage("LoadLevelProgress"); // Seviye durumunu sıfırla
        SceneManager.LoadScene("LevelScene");
        
    }
}