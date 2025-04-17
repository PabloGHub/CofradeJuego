using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string StartScene;
    public void StartGame()
    {
        if (StartScene.Length != 0)
            SceneManager.LoadScene(StartScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
