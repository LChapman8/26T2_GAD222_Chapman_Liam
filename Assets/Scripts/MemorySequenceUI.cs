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
        ResetSequence();
    }

    public void StartSequence()
    {
        if (sequenceCoroutine != null)
            StopCoroutine(sequenceCoroutine);

        sequenceCoroutine = StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        ResetSequence();

        foreach (string line in sequenceLines)
        {
            sequenceText.text = line;
            yield return new WaitForSeconds(timeBetweenLines);
        }

        if (continueButton != null)
            continueButton.gameObject.SetActive(true);
    }

    private void ResetSequence()
    {
        if (sequenceText != null)
            sequenceText.text = "";

        if (continueButton != null)
            continueButton.gameObject.SetActive(false);
    }
}