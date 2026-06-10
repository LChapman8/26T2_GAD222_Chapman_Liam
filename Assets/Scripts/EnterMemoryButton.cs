using System.Collections;
using UnityEngine;

public class EnterMemoryButton : MonoBehaviour
{
    [Header("Scene Objects")]
    public GameObject currentTransitionScene;
    public GameObject nextMemoryScene;

    [Header("White Flash")]
    public CanvasGroup whiteFlashPanel;
    public float flashDuration = 0.2f;

    public void EnterMemory()
    {
        StartCoroutine(EnterMemoryRoutine());
    }

    private IEnumerator EnterMemoryRoutine()
    {
        yield return StartCoroutine(WhiteFlash());

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (currentTransitionScene != null)
            currentTransitionScene.SetActive(false);

        if (nextMemoryScene != null)
            nextMemoryScene.SetActive(true);
    }

    private IEnumerator WhiteFlash()
    {
        if (whiteFlashPanel == null)
            yield break;

        whiteFlashPanel.gameObject.SetActive(true);

        // Fade In
        float elapsed = 0f;

        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;

            whiteFlashPanel.alpha =
                Mathf.Lerp(0f, 1f, elapsed / flashDuration);

            yield return null;
        }

        whiteFlashPanel.alpha = 1f;

        // Fade Out
        elapsed = 0f;

        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;

            whiteFlashPanel.alpha =
                Mathf.Lerp(1f, 0f, elapsed / flashDuration);

            yield return null;
        }

        whiteFlashPanel.alpha = 0f;
        whiteFlashPanel.gameObject.SetActive(false);
    }
}