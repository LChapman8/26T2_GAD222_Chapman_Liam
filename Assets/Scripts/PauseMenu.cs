using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuUI;

    [Header("Player Controls")]
    [Tooltip(
        "Drag your First Person Controller and camera/look scripts here. " +
        "These will be disabled while the game is paused.")]
    [SerializeField] private Behaviour[] playerControlScripts;

    [Header("Main Menu")]
    [Tooltip("Exact name of your main menu scene.")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    private void Start()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        // Stop gameplay time.
        Time.timeScale = 0f;

        // Disable player movement / camera controls.
        SetPlayerControl(false);

        // Release mouse for UI.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isPaused = true;
    }

    public void ResumeGame()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        // Resume gameplay time.
        Time.timeScale = 1f;

        // Restore player movement / camera controls.
        SetPlayerControl(true);

        // Lock mouse back into gameplay.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isPaused = false;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        SetPlayerControl(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

        Debug.Log("Quitting Game...");

        Application.Quit();
    }

    private void SetPlayerControl(bool enabled)
    {
        if (playerControlScripts == null)
            return;

        foreach (Behaviour controlScript in playerControlScripts)
        {
            if (controlScript != null)
                controlScript.enabled = enabled;
        }
    }
}