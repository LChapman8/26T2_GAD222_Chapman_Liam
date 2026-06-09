using System.Collections;
using UnityEngine;

public class SceneTransitionManager : MonoBehaviour
{
    [Header("Fade")]
    public CanvasGroup fadePanel;
    public float fadeTime = 1f;
    public float blackScreenPause = 0.5f;
    public float delayBeforeSequenceStarts = 0.5f;

    public void SwapObjects(GameObject objectToDisable, GameObject objectToEnable)
    {
        StartCoroutine(SwapRoutine(objectToDisable, objectToEnable));
    }

    private IEnumerator SwapRoutine(GameObject objectToDisable, GameObject objectToEnable)
    {
        yield return StartCoroutine(FadeOut());

        if (objectToDisable != null)
            objectToDisable.SetActive(false);

        if (objectToEnable != null)
            objectToEnable.SetActive(true);

        yield return null;
        yield return new WaitForSeconds(blackScreenPause);

        yield return StartCoroutine(FadeIn());

        yield return new WaitForSeconds(delayBeforeSequenceStarts);

        MemorySequenceUI sequence =
            objectToEnable.GetComponentInChildren<MemorySequenceUI>(true);

        if (sequence != null)
            sequence.StartSequence();
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
}