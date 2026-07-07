using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    private void Start()
    {
        // Mostrar el cursor al entrar a la escena de derrota
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 1f; // Por si el juego estaba pausado
    }
    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Sweet-Dream"); // Cambia "Gameplay" por el nombre de tu escena 2
    }

    // Regresa al menú principal
    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("UI_Inicio"); // Cambia "MainMenu" por el nombre de tu escena de inicio
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}