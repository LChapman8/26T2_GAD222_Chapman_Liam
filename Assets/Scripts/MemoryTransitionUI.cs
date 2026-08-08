using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MemoryTransitionUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text sequenceText;
    [SerializeField] private Button continueButton;
    [SerializeField] private TMP_Text continueButtonText;

    [Header("Robot Voice")]
    [SerializeField] private AudioSource robotVoiceSource;

    [Header("Timing")]
    [SerializeField] private float initialDelay = 0.5f;

    [Tooltip("Used if a transition line has no audio clip.")]
    [SerializeField] private float fallbackLineDuration = 1.5f;

    [Tooltip("Small pause between robot lines.")]
    [SerializeField] private float pauseBetweenLines = 0.2f;

    private Coroutine sequenceRoutine;
    private Action continueAction;

    private void Awake()
    {
        HideUI();

        if (robotVoiceSource != null)
        {
            robotVoiceSource.playOnAwake = false;
            robotVoiceSource.loop = false;
        }
    }

    public void Begin(
        string[] lines,
        AudioClip[] voiceClips,
        string buttonLabel,
        Action onContinue)
    {
        gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        continueAction = onContinue;

        if (continueButtonText != null)
            continueButtonText.text = buttonLabel;

        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(HandleContinueClicked);
            continueButton.interactable = true;
        }

        if (sequenceRoutine != null)
            StopCoroutine(sequenceRoutine);

        sequenceRoutine = StartCoroutine(
            PlaySequence(lines, voiceClips));
    }

    private IEnumerator PlaySequence(
        string[] lines,
        AudioClip[] voiceClips)
    {
        if (sequenceText != null)
        {
            sequenceText.gameObject.SetActive(true);
            sequenceText.text = string.Empty;
        }

        if (continueButton != null)
            continueButton.gameObject.SetActive(false);

        yield return new WaitForSeconds(initialDelay);

        if (lines != null)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                if (sequenceText != null)
                    sequenceText.text = lines[i];

                AudioClip currentClip = null;

                if (voiceClips != null && i < voiceClips.Length)
                    currentClip = voiceClips[i];

                if (currentClip != null && robotVoiceSource != null)
                {
                    robotVoiceSource.clip = currentClip;
                    robotVoiceSource.Play();

                    yield return new WaitWhile(
                        () => robotVoiceSource.isPlaying);
                }
                else
                {
                    yield return new WaitForSeconds(
                        fallbackLineDuration);
                }

                if (pauseBetweenLines > 0f)
                    yield return new WaitForSeconds(
                        pauseBetweenLines);
            }
        }

        if (continueButton != null)
            continueButton.gameObject.SetActive(true);
    }

    private void HandleContinueClicked()
    {
        if (continueButton != null)
            continueButton.interactable = false;

        HideUI();
        continueAction?.Invoke();
    }

    private void HideUI()
    {
        if (robotVoiceSource != null &&
            robotVoiceSource.isPlaying)
        {
            robotVoiceSource.Stop();
        }

        if (sequenceText != null)
        {
            sequenceText.text = string.Empty;
            sequenceText.gameObject.SetActive(false);
        }

        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(false);
            continueButton.interactable = true;
        }
    }

    private void OnDisable()
    {
        if (sequenceRoutine != null)
        {
            StopCoroutine(sequenceRoutine);
            sequenceRoutine = null;
        }

        if (robotVoiceSource != null)
            robotVoiceSource.Stop();
    }
}