using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Reference to the pause menu UI
    public bool isPaused = false; // Track whether the game is paused

    void Update()
    {
        // Toggle pause when the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true); // Show the pause menu
        Time.timeScale = 0f; // Freeze game time
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false); // Hide the pause menu
        Time.timeScale = 1f; // Resume the game time
        isPaused = false;
    }

    public void Quit()
    {
        // Add any quit functionality here, like saving data
        Time.timeScale = 1f; // Ensure the game resumes before quitting
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu"); // Go to the main menu or quit
    }
}
