using System.Collections;
using UnityEngine;

public class EnvironmentTransitionManager : MonoBehaviour
{
    [Header("Global Fade")]
    [SerializeField] private CanvasGroup fadePanel;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float blackScreenDuration = 0.3f;

    private GameObject activeTransitionEnvironment;
    private GameObject pendingEnvironment;

    private bool transitionInProgress;

    public void BeginTransition(
        GameObject currentEnvironment,
        GameObject transitionEnvironment,
        GameObject nextEnvironment,
        string[] transitionLines,
        string buttonLabel)
    {
        if (transitionInProgress)
            return;

        StartCoroutine(BeginTransitionRoutine(
            currentEnvironment,
            transitionEnvironment,
            nextEnvironment,
            transitionLines,
            buttonLabel));
    }

    private IEnumerator BeginTransitionRoutine(
        GameObject currentEnvironment,
        GameObject transitionEnvironment,
        GameObject nextEnvironment,
        string[] transitionLines,
        string buttonLabel)
    {
        transitionInProgress = true;

        yield return Fade(0f, 1f);

        if (currentEnvironment != null)
            currentEnvironment.SetActive(false);

        if (transitionEnvironment != null)
            transitionEnvironment.SetActive(true);

        activeTransitionEnvironment = transitionEnvironment;
        pendingEnvironment = nextEnvironment;

        yield return null;
        yield return new WaitForSeconds(blackScreenDuration);

        yield return Fade(1f, 0f);

        MemoryTransitionUI transitionUI =
            transitionEnvironment != null
                ? transitionEnvironment.GetComponentInChildren<MemoryTransitionUI>(true)
                : null;

        if (transitionUI != null)
        {
            transitionUI.Begin(
                transitionLines,
                buttonLabel,
                CompleteTransition);
        }
        else
        {
            Debug.LogError(
                "No MemoryTransitionUI was found inside the transition environment.");
            transitionInProgress = false;
        }
    }

    private void CompleteTransition()
    {
        StartCoroutine(CompleteTransitionRoutine());
    }

    private IEnumerator CompleteTransitionRoutine()
    {
        yield return Fade(0f, 1f);

        if (activeTransitionEnvironment != null)
            activeTransitionEnvironment.SetActive(false);

        if (pendingEnvironment != null)
            pendingEnvironment.SetActive(true);

        activeTransitionEnvironment = null;
        pendingEnvironment = null;

        yield return null;
        yield return new WaitForSeconds(blackScreenDuration);

        yield return Fade(1f, 0f);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        transitionInProgress = false;
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        if (fadePanel == null)
            yield break;

        fadePanel.gameObject.SetActive(true);
        fadePanel.alpha = startAlpha;
        fadePanel.blocksRaycasts = true;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            fadePanel.alpha = Mathf.Lerp(
                startAlpha,
                endAlpha,
                elapsed / fadeDuration);

            yield return null;
        }

        fadePanel.alpha = endAlpha;

        if (Mathf.Approximately(endAlpha, 0f))
        {
            fadePanel.blocksRaycasts = false;
            fadePanel.gameObject.SetActive(false);
        }
    }
}