using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Scene_Home");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}