using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MemorySequenceUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text sequenceText;
    public Button continueButton;

    [Header("Sequence Lines")]
    [TextArea]
    public string[] sequenceLines;

    [Header("Timing")]
    public float timeBetweenLines = 1.5f;

    private Coroutine sequenceCoroutine;

    private void Awake()
    {
        HideUI();
    }

    private void Start()
    {
        HideUI();
    }

    public void StartSequence()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameObject.SetActive(true);

        if (sequenceCoroutine != null)
            StopCoroutine(sequenceCoroutine);

        sequenceCoroutine = StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        ShowTextOnly();

        foreach (string line in sequenceLines)
        {
            sequenceText.text = line;
            yield return new WaitForSeconds(timeBetweenLines);
        }

        if (continueButton != null)
            continueButton.gameObject.SetActive(true);
    }

    private void ShowTextOnly()
    {
        if (sequenceText != null)
        {
            sequenceText.gameObject.SetActive(true);
            sequenceText.text = "";
        }

        if (continueButton != null)
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