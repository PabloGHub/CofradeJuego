using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public int StartScene;
    public void StartGame()
    {
        if (StartScene >= 0)
            SceneManager.LoadScene(StartScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
