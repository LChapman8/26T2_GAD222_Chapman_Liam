using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FinalSceneSequence : MonoBehaviour
{
    [Serializable]
    public class DialogueLine
    {
        [Header("Subtitle")]
        public string speakerName;

        [TextArea(2, 5)]
        public string subtitle;

        [Header("Audio")]
        public AudioClip audioClip;

        [Min(0f)]
        public float pauseAfterLine = 0.15f;
    }

    [Header("Dialogue")]
    [SerializeField] private DialogueLine[] dialogueLines;
    [SerializeField] private AudioSource dialogueAudioSource;
    [SerializeField] private DialogueSubtitleUI subtitleUI;

    [Header("Player Controls")]
    [SerializeField] private Behaviour[] playerControlScripts;

    [Header("Start Settings")]
    [Tooltip("Delay after the final scene becomes active before dialogue begins.")]
    [SerializeField] private float startDelay = 0.5f;

    [Header("Ending Fade")]
    [SerializeField] private CanvasGroup blackFadePanel;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float pauseBeforeResults = 1f;

    [Header("Results UI")]
    [SerializeField] private GameObject endingPanel;
    [SerializeField] private TMP_Text endingSummaryText;
    [SerializeField] private Button continueButton;

    [Header("Final Question")]
    [SerializeField] private GameObject finalQuestionPanel;
    [SerializeField] private TMP_Text finalQuestionText;

    [TextArea(3, 6)]
    [SerializeField]
    private string finalQuestion =
        "If your life could only be remembered through a handful of memories...\n\nWhich memories would remain?";

    private bool hasStarted;

    private void Awake()
    {
        if (endingPanel != null)
            endingPanel.SetActive(false);

        if (finalQuestionPanel != null)
            finalQuestionPanel.SetActive(false);

        if (blackFadePanel != null)
        {
            blackFadePanel.alpha = 0f;
            blackFadePanel.blocksRaycasts = false;
            blackFadePanel.gameObject.SetActive(false);
        }

        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(ShowFinalQuestion);
        }

        if (dialogueAudioSource != null)
        {
            dialogueAudioSource.playOnAwake = false;
            dialogueAudioSource.loop = false;
        }
    }

    private void OnEnable()
    {
        if (!hasStarted)
        {
            StartCoroutine(StartFinalSequenceAutomatically());
        }
    }

    private IEnumerator StartFinalSequenceAutomatically()
    {
        yield return new WaitForSeconds(startDelay);

        StartFinalSequence();
    }

    public void StartFinalSequence()
    {
        if (hasStarted)
            return;

        hasStarted = true;

        StartCoroutine(FinalSequenceRoutine());
    }

    private IEnumerator FinalSequenceRoutine()
    {
        SetPlayerControl(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // -------------------------
        // PLAY FINAL DIALOGUE
        // -------------------------

        if (dialogueLines != null)
        {
            foreach (DialogueLine line in dialogueLines)
            {
                if (line == null)
                    continue;

                if (subtitleUI != null)
                {
                    subtitleUI.Show(
                        line.speakerName,
                        line.subtitle);
                }

                if (line.audioClip != null &&
                    dialogueAudioSource != null)
                {
                    dialogueAudioSource.clip = line.audioClip;
                    dialogueAudioSource.Play();

                    yield return new WaitWhile(
                        () => dialogueAudioSource.isPlaying);
                }
                else
                {
                    float fallbackDuration =
                        CalculateFallbackDuration(line.subtitle);

                    yield return new WaitForSeconds(
                        fallbackDuration);
                }

                if (line.pauseAfterLine > 0f)
                {
                    yield return new WaitForSeconds(
                        line.pauseAfterLine);
                }
            }
        }

        if (subtitleUI != null)
            subtitleUI.Hide();

        // -------------------------
        // FADE TO BLACK
        // -------------------------

        yield return FadeToBlack();

        yield return new WaitForSeconds(
            pauseBeforeResults);

        // -------------------------
        // DISPLAY RESULTS
        // -------------------------

        BuildEndingSummary();

        if (endingPanel != null)
            endingPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void BuildEndingSummary()
    {
        if (endingSummaryText == null)
            return;

        MemoryPathTracker.MemoryPath path =
            MemoryPathTracker.Instance != null
                ? MemoryPathTracker.Instance.SelectedPath
                : MemoryPathTracker.MemoryPath.None;

        switch (path)
        {
            case MemoryPathTracker.MemoryPath.Career:

                endingSummaryText.text =
                    "YOUR WORK ALLOWED THE PATIENT TO REMEMBER:\n\n" +
                    "His passion for science.\n" +
                    "His dedication to research.\n" +
                    "The technology he spent his life creating.\n\n\n" +

                    "THE MEMORIES THAT WERE LOST:\n\n" +
                    "His first date with the woman he loved.\n" +
                    "His wedding day.\n" +
                    "The birth of his daughter, Daisy.\n" +
                    "A day fishing with his father.\n" +
                    "A family holiday with his parents.\n" +
                    "His parents' final goodbye.";

                break;

            case MemoryPathTracker.MemoryPath.Relationship:

                endingSummaryText.text =
                    "YOUR WORK ALLOWED THE PATIENT TO REMEMBER:\n\n" +
                    "His first date with the woman he loved.\n" +
                    "His wedding day.\n" +
                    "The birth of his daughter, Daisy.\n\n\n" +

                    "THE MEMORIES THAT WERE LOST:\n\n" +
                    "His passion for science.\n" +
                    "His university graduation.\n" +
                    "The technology he spent his life creating.\n" +
                    "A day fishing with his father.\n" +
                    "A family holiday with his parents.\n" +
                    "His parents' final goodbye.";

                break;

            case MemoryPathTracker.MemoryPath.Family:

                endingSummaryText.text =
                    "YOUR WORK ALLOWED THE PATIENT TO REMEMBER:\n\n" +
                    "A day fishing with his father.\n" +
                    "A family holiday with his parents.\n" +
                    "His parents' final goodbye.\n\n\n" +

                    "THE MEMORIES THAT WERE LOST:\n\n" +
                    "His first date with the woman he loved.\n" +
                    "His wedding day.\n" +
                    "The birth of his daughter, Daisy.\n" +
                    "His passion for science.\n" +
                    "His university graduation.\n" +
                    "The technology he spent his life creating.";

                break;

            default:

                endingSummaryText.text =
                    "Memory reconstruction data unavailable.";

                Debug.LogWarning(
                    "No memory path was recorded before the ending.");

                break;
        }
    }

    private void ShowFinalQuestion()
    {
        if (endingPanel != null)
            endingPanel.SetActive(false);

        if (finalQuestionPanel != null)
            finalQuestionPanel.SetActive(true);

        if (finalQuestionText != null)
            finalQuestionText.text = finalQuestion;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private IEnumerator FadeToBlack()
    {
        if (blackFadePanel == null)
            yield break;

        blackFadePanel.gameObject.SetActive(true);
        blackFadePanel.blocksRaycasts = true;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            blackFadePanel.alpha =
                Mathf.Lerp(
                    0f,
                    1f,
                    elapsed / fadeDuration);

            yield return null;
        }

        blackFadePanel.alpha = 1f;
    }

    private float CalculateFallbackDuration(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 1f;

        return Mathf.Max(
            1.5f,
            text.Length / 14f);
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