using System.Collections;
using UnityEngine;

public class SceneTransitionManager : MonoBehaviour
{
    [Header("Fade Panels")]
    public CanvasGroup fadePanel;
    public CanvasGroup whiteFlashPanel;

    [Header("Timing")]
    public float fadeTime = 1f;
    public float blackScreenPause = 0.5f;
    public float delayBeforeSequenceStarts = 0.5f;
    public float flashDuration = 0.2f;

    private GameObject currentTransitionScene;
    private GameObject nextMemoryScene;

    public void BeginMemoryTransition(
        GameObject objectToDisable,
        GameObject transitionScene,
        GameObject memorySceneToEnable,
        string[] sequenceLines,
        string buttonText
    )
    {
        StartCoroutine(TransitionRoutine(
            objectToDisable,
            transitionScene,
            memorySceneToEnable,
            sequenceLines,
            buttonText
        ));
    }

    private IEnumerator TransitionRoutine(
        GameObject objectToDisable,
        GameObject transitionScene,
        GameObject memorySceneToEnable,
        string[] sequenceLines,
        string buttonText
    )
    {
        yield return StartCoroutine(FadeOut());

        if (objectToDisable != null)
            objectToDisable.SetActive(false);

        if (transitionScene != null)
            transitionScene.SetActive(true);

        currentTransitionScene = transitionScene;
        nextMemoryScene = memorySceneToEnable;

        yield return null;
        yield return new WaitForSeconds(blackScreenPause);

        yield return StartCoroutine(FadeIn());

        yield return new WaitForSeconds(delayBeforeSequenceStarts);

        MemorySequenceUI sequenceUI =
            transitionScene.GetComponentInChildren<MemorySequenceUI>(true);

        if (sequenceUI != null)
        {
            sequenceUI.StartSequence(sequenceLines, buttonText, EnterMemory);
        }
    }

    private void EnterMemory()
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

    private IEnumerator FadeOut()
    {
        fadePanel.gameObject.SetActive(true);

        float elapsed = 0f;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeTime);
            yield return null;
        }

        fadePanel.alpha = 1f;
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            yield return null;
        }

        fadePanel.alpha = 0f;
        fadePanel.gameObject.SetActive(false);
    }

    private IEnumerator WhiteFlash()
    {
        if (whiteFlashPanel == null)
            yield break;

        whiteFlashPanel.gameObject.SetActive(true);

        float elapsed = 0f;

        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            whiteFlashPanel.alpha = Mathf.Lerp(0f, 1f, elapsed / flashDuration);
            yield return null;
        }

        whiteFlashPanel.alpha = 1f;

        elapsed = 0f;

        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            whiteFlashPanel.alpha = Mathf.Lerp(1f, 0f, elapsed / flashDuration);
            yield return null;
        }

        whiteFlashPanel.alpha = 0f;
        whiteFlashPanel.gameObject.SetActive(false);
    }
}