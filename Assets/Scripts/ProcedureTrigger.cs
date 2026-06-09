using UnityEngine;
using UnityEngine.InputSystem;

public class ProcedureTrigger : MonoBehaviour
{
    public GameObject promptUI;
    public GameObject objectToDisable;
    public GameObject objectToEnable;
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

            transitionManager.SwapObjects(objectToDisable, objectToEnable);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            playerInZone = true;

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