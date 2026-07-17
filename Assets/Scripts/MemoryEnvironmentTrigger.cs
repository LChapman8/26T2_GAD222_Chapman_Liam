using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class MemoryEnvironmentTrigger : MonoBehaviour
{
    [Header("Prompt")]
    [SerializeField] private GameObject promptUI;
    [SerializeField] private TMP_Text promptText;

    [SerializeField]
    private string promptMessage = "Press E to access memory";

    [Header("Environment References")]
    [SerializeField] private GameObject currentEnvironment;
    [SerializeField] private GameObject transitionEnvironment;
    [SerializeField] private GameObject nextEnvironment;

    [Header("Transition Content")]
    [TextArea(2, 5)]
    [SerializeField] private string[] transitionLines;

    [SerializeField]
    private string transitionButtonText = "Enter Memory";

    [Header("Manager")]
    [SerializeField]
    private EnvironmentTransitionManager transitionManager;

    [Header("Settings")]
    [SerializeField] private bool canOnlyTriggerOnce = true;

    private bool playerInside;
    private bool hasTriggered;

    private void Awake()
    {
        Collider triggerCollider = GetComponent<Collider>();
        triggerCollider.isTrigger = true;

        HidePrompt();
    }

    private void Update()
    {
        if (!playerInside || hasTriggered)
            return;

        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            ActivateTransition();
        }
    }

    private void ActivateTransition()
    {
        if (transitionManager == null)
        {
            Debug.LogError(
                $"No transition manager assigned to {gameObject.name}.");
            return;
        }

        if (canOnlyTriggerOnce)
            hasTriggered = true;

        HidePrompt();

        transitionManager.BeginTransition(
            currentEnvironment,
            transitionEnvironment,
            nextEnvironment,
            transitionLines,
            transitionButtonText);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || hasTriggered)
            return;

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
            promptText.text = promptMessage;

        if (promptUI != null)
            promptUI.SetActive(true);
    }

    private void HidePrompt()
    {
        if (promptUI != null)
            promptUI.SetActive(false);
    }
}