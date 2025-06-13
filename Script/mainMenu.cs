using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{

    public void playGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void settingsMenu()
    {
        SceneManager.LoadScene("Settings Menu");
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
