using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitConfirmationManager : MonoBehaviour
{
    [Header("Exit Confirmation Panel")]
    public GameObject exitConfirmationPanel;

    public Button yesButton;
    public Button noButton;

    void Start()
    {
        yesButton.onClick.AddListener(OnYesButtonClicked);
        noButton.onClick.AddListener(OnNoButtonClicked);

        exitConfirmationPanel.SetActive(false);
    }

    void OnYesButtonClicked()
    {
        SceneManager.LoadScene("SampleScene");
    }

    void OnNoButtonClicked()
    {

        exitConfirmationPanel.SetActive(false);
    }

    public void ShowExitConfirmationPanel()
    {
        exitConfirmationPanel.SetActive(true);
    }
}
