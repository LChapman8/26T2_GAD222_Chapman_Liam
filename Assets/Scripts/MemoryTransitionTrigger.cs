using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MemoryTransitionTrigger : MonoBehaviour
{
    [Header("Prompt")]
    public GameObject promptUI;
    public TMP_Text promptText;
    public string promptMessage = "Press 'E' to begin procedure";

    [Header("Scene Objects")]
    public GameObject objectToDisable;
    public GameObject transitionScene;
    public GameObject nextMemoryScene;

    [Header("Transition Text")]
    [TextArea]
    public string[] sequenceLines;

    public string buttonText = "Enter Memory";

    [Header("Manager")]
    public SceneTransitionManager transitionManager;

    private bool playerInZone;
    private bool hasTriggered;

    private void Start()
    {
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    private void Update()
    {
        if (playerInZone && !hasTriggered && Keyboard.current.eKey.wasPressedThisFrame)
        {
            hasTriggered = true;

            if (promptUI != null)
                promptUI.SetActive(false);

            transitionManager.BeginMemoryTransition(
                objectToDisable,
                transitionScene,
                nextMemoryScene,
                sequenceLines,
                buttonText
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            playerInZone = true;

            if (promptText != null)
                promptText.text = promptMessage;

            if (promptUI != null)
                promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;

            if (promptUI != null)
                promptUI.SetActive(false);
        }
    }
}