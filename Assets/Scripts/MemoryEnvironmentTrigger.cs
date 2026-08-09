using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class MemoryEnvironmentTrigger : MonoBehaviour
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

        [Tooltip("Optional pause after this line finishes.")]
        [Min(0f)]
        public float pauseAfterLine = 0.15f;
    }

    [Header("Interaction Prompt")]
    [SerializeField] private GameObject promptUI;
    [SerializeField] private TMP_Text promptText;

    [SerializeField]
    private string promptMessage = "Press E to interact";

    [Header("Dialogue")]
    [SerializeField] private DialogueLine[] dialogueLines;
    [SerializeField] private AudioSource dialogueAudioSource;
    [SerializeField] private DialogueSubtitleUI subtitleUI;

    [Header("Player Control During Dialogue")]
    [Tooltip(
        "Drag your First Person Controller and any camera-look scripts here. " +
        "They will be disabled while dialogue plays.")]
    [SerializeField] private Behaviour[] playerControlScripts;

    [Header("Environment References")]
    [SerializeField] private GameObject currentEnvironment;
    [SerializeField] private GameObject transitionEnvironment;
    [SerializeField] private GameObject nextEnvironment;

    [Header("Memory Transition Content")]
    [TextArea(2, 5)]
    [SerializeField] private string[] transitionLines;

    [Tooltip(
        "Add one robot voice clip for each transition line. " +
        "The order must match the Transition Lines array.")]
    [SerializeField] private AudioClip[] transitionVoiceClips;

    [SerializeField]
    private string transitionButtonText = "Enter Memory";

    [Header("Memory Path Tracking")]
    [Tooltip(
        "Enable this ONLY on the three initial birthday-party choices.")]
    [SerializeField] private bool setsMemoryPath = false;

    [SerializeField]
    private MemoryPathTracker.MemoryPath memoryPath =
        MemoryPathTracker.MemoryPath.None;

    [Header("Manager")]
    [SerializeField]
    private EnvironmentTransitionManager transitionManager;

    [Header("Settings")]
    [SerializeField] private bool canOnlyTriggerOnce = true;

    private bool playerInside;
    private bool hasTriggered;
    private bool dialoguePlaying;

    private bool skipCurrentLine;

    private Coroutine dialogueRoutine;

    private void Awake()
    {
        Collider triggerCollider = GetComponent<Collider>();
        triggerCollider.isTrigger = true;

        HidePrompt();

        if (subtitleUI != null)
            subtitleUI.Hide();

        if (dialogueAudioSource != null)
        {
            dialogueAudioSource.playOnAwake = false;
            dialogueAudioSource.loop = false;
        }
    }

    private void Update()
    {
        // --------------------------------
        // DIALOGUE SKIPPING
        // --------------------------------

        if (dialoguePlaying)
        {
            bool skipPressed = false;

            if (Keyboard.current != null &&
                Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                skipPressed = true;
            }

            if (Mouse.current != null &&
                Mouse.current.leftButton.wasPressedThisFrame)
            {
                skipPressed = true;
            }

            if (skipPressed)
            {
                SkipCurrentDialogueLine();
            }

            return;
        }

        // --------------------------------
        // NORMAL INTERACTION
        // --------------------------------

        if (!playerInside || hasTriggered)
            return;

        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            BeginInteraction();
        }
    }

    private void BeginInteraction()
    {
        if (transitionManager == null)
        {
            Debug.LogError(
                $"No EnvironmentTransitionManager assigned to {gameObject.name}.");

            return;
        }

        if (dialogueAudioSource == null &&
            dialogueLines != null &&
            dialogueLines.Length > 0)
        {
            Debug.LogError(
                $"No dialogue AudioSource assigned to {gameObject.name}.");

            return;
        }

        if (canOnlyTriggerOnce)
            hasTriggered = true;

        HidePrompt();

        // Record the player's chosen memory path.
        if (setsMemoryPath)
        {
            if (MemoryPathTracker.Instance != null)
            {
                MemoryPathTracker.Instance.SetPath(memoryPath);
            }
            else
            {
                Debug.LogWarning(
                    "This trigger is trying to set a memory path, " +
                    "but no MemoryPathTracker exists in the scene.");
            }
        }

        if (dialogueRoutine != null)
            StopCoroutine(dialogueRoutine);

        dialogueRoutine =
            StartCoroutine(PlayDialogueSequence());
    }

    private IEnumerator PlayDialogueSequence()
    {
        dialoguePlaying = true;

        SetPlayerControl(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (dialogueLines != null)
        {
            foreach (DialogueLine line in dialogueLines)
            {
                if (line == null)
                    continue;

                skipCurrentLine = false;

                // -------------------------
                // SHOW SUBTITLE
                // -------------------------

                if (subtitleUI != null)
                {
                    subtitleUI.Show(
                        line.speakerName,
                        line.subtitle);
                }

                // -------------------------
                // PLAY VOICE LINE
                // -------------------------

                if (line.audioClip != null &&
                    dialogueAudioSource != null)
                {
                    dialogueAudioSource.clip =
                        line.audioClip;

                    dialogueAudioSource.Play();

                    while (
                        dialogueAudioSource.isPlaying &&
                        !skipCurrentLine)
                    {
                        yield return null;
                    }

                    // If player skipped this line,
                    // stop immediately and advance.
                    if (skipCurrentLine)
                    {
                        dialogueAudioSource.Stop();

                        skipCurrentLine = false;

                        continue;
                    }
                }

                // -------------------------
                // SUBTITLE-ONLY FALLBACK
                // -------------------------

                else
                {
                    float fallbackDuration =
                        CalculateFallbackSubtitleDuration(
                            line.subtitle);

                    float elapsed = 0f;

                    while (
                        elapsed < fallbackDuration &&
                        !skipCurrentLine)
                    {
                        elapsed += Time.deltaTime;

                        yield return null;
                    }

                    if (skipCurrentLine)
                    {
                        skipCurrentLine = false;

                        continue;
                    }
                }

                // -------------------------
                // PAUSE AFTER LINE
                // -------------------------

                if (line.pauseAfterLine > 0f)
                {
                    float pauseElapsed = 0f;

                    while (
                        pauseElapsed < line.pauseAfterLine &&
                        !skipCurrentLine)
                    {
                        pauseElapsed += Time.deltaTime;

                        yield return null;
                    }

                    skipCurrentLine = false;
                }
            }
        }

        if (subtitleUI != null)
            subtitleUI.Hide();

        dialoguePlaying = false;

        SetPlayerControl(true);

        BeginMemoryTransition();
    }

    private void SkipCurrentDialogueLine()
    {
        if (!dialoguePlaying)
            return;

        skipCurrentLine = true;

        if (dialogueAudioSource != null &&
            dialogueAudioSource.isPlaying)
        {
            dialogueAudioSource.Stop();
        }
    }

    private void BeginMemoryTransition()
    {
        transitionManager.BeginTransition(
            currentEnvironment,
            transitionEnvironment,
            nextEnvironment,
            transitionLines,
            transitionVoiceClips,
            transitionButtonText);
    }

    private float CalculateFallbackSubtitleDuration(
        string text)
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

        foreach (Behaviour controlScript
                 in playerControlScripts)
        {
            if (controlScript != null)
            {
                controlScript.enabled = enabled;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") ||
            hasTriggered ||
            dialoguePlaying)
        {
            return;
        }

        playerInside = true;

        ShowPrompt();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;

        HidePrompt();
    }

    private void ShowPrompt()
    {
        if (promptText != null)
        {
            promptText.text = promptMessage;
        }

        if (promptUI != null)
        {
            promptUI.SetActive(true);
        }
    }

    private void HidePrompt()
    {
        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (dialogueAudioSource != null &&
            dialogueAudioSource.isPlaying)
        {
            dialogueAudioSource.Stop();
        }

        if (subtitleUI != null)
        {
            subtitleUI.Hide();
        }

        SetPlayerControl(true);

        skipCurrentLine = false;
        dialoguePlaying = false;
        playerInside = false;
    }
}