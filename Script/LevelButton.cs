using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    public int levelId;

    public void OnLevelSelected()
    {
        GameData.selectedLevelId = levelId;
        SceneManager.LoadScene("type1Scene");
    }
}
