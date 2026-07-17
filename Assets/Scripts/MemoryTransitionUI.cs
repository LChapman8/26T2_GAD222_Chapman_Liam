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

    [Header("Timing")]
    [SerializeField] private float initialDelay = 0.5f;
    [SerializeField] private float timeBetweenLines = 1.5f;

    private Coroutine sequenceRoutine;
    private Action continueAction;

    private void Awake()
    {
        HideUI();
    }

    public void Begin(
        string[] lines,
        string buttonLabel,
        Action onContinue)
    {
        gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        continueAction = onContinue;

        if (continueButtonText != null)
            continueButtonText.text = buttonLabel;

        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(HandleContinueClicked);

        if (sequenceRoutine != null)
            StopCoroutine(sequenceRoutine);

        sequenceRoutine = StartCoroutine(PlaySequence(lines));
    }

    private IEnumerator PlaySequence(string[] lines)
    {
        sequenceText.gameObject.SetActive(true);
        sequenceText.text = string.Empty;

        continueButton.gameObject.SetActive(false);

        yield return new WaitForSeconds(initialDelay);

        if (lines != null)
        {
            foreach (string line in lines)
            {
                sequenceText.text = line;
                yield return new WaitForSeconds(timeBetweenLines);
            }
        }

        continueButton.gameObject.SetActive(true);
    }

    private void HandleContinueClicked()
    {
        continueButton.interactable = false;

        HideUI();
        continueAction?.Invoke();

        continueButton.interactable = true;
    }

    private void HideUI()
    {
        if (sequenceText != null)
        {
            sequenceText.text = string.Empty;
            sequenceText.gameObject.SetActive(false);
        }

        if (continueButton != null)
            continueButton.gameObject.SetActive(false);
    }
}