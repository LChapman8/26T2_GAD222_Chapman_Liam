using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MemorySequenceUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text sequenceText;
    public Button continueButton;
    public TMP_Text buttonText;

    [Header("Timing")]
    public float timeBetweenLines = 1.5f;

    private Coroutine sequenceCoroutine;

    private void Awake()
    {
        HideUI();
    }

    public void StartSequence(string[] lines, string continueButtonText, Action onContinue)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        gameObject.SetActive(true);

        if (sequenceCoroutine != null)
            StopCoroutine(sequenceCoroutine);

        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() => onContinue?.Invoke());

        if (buttonText != null)
            buttonText.text = continueButtonText;

        sequenceCoroutine = StartCoroutine(PlaySequence(lines));
    }

    private IEnumerator PlaySequence(string[] lines)
    {
        ShowTextOnly();

        foreach (string line in lines)
        {
            sequenceText.text = line;
            yield return new WaitForSeconds(timeBetweenLines);
        }

        continueButton.gameObject.SetActive(true);
    }

    private void ShowTextOnly()
    {
        sequenceText.gameObject.SetActive(true);
        sequenceText.text = "";

        continueButton.gameObject.SetActive(false);
    }

    public void HideUI()
    {
        if (sequenceText != null)
        {
            sequenceText.text = "";
            sequenceText.gameObject.SetActive(false);
        }

        if (continueButton != null)
            continueButton.gameObject.SetActive(false);
    }
}