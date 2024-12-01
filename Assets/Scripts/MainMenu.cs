using UnityEngine;
using UnityEngine.SceneManagement;  // To load scenes
using UnityEngine.UI;  // For Button

public class MainMenu : MonoBehaviour
{
    public Button startButton;
    public Button exitButton;

    void Start()
    {
        // Ensure buttons are assigned
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitGame);
        }
    }

    // Method to start the game (load a new scene)
    void StartGame()
    {
        // Replace "GameScene" with the name of the scene you want to load
        SceneManager.LoadScene("SampleScene");  // Make sure this scene is added in the build settings
    }

    // Method to exit the game
    void ExitGame()
    {
#if UNITY_EDITOR
        // Exit play mode in the Unity editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Quit the game in a build
        Application.Quit();
#endif
    }
}
