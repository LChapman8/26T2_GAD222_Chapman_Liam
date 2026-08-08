using TMPro;
using UnityEngine;

public class DialogueSubtitleUI : MonoBehaviour
{
    [Header("Subtitle UI")]
    [SerializeField] private GameObject subtitlePanel;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text subtitleText;

    private void Awake()
    {
        Hide();
    }

    public void Show(string speakerName, string subtitle)
    {
        if (subtitlePanel != null)
            subtitlePanel.SetActive(true);

        if (speakerNameText != null)
        {
            speakerNameText.text = speakerName;
            speakerNameText.gameObject.SetActive(
                !string.IsNullOrWhiteSpace(speakerName));
        }

        if (subtitleText != null)
            subtitleText.text = subtitle;
    }

    public void Hide()
    {
        if (speakerNameText != null)
        {
            speakerNameText.text = string.Empty;
            speakerNameText.gameObject.SetActive(false);
        }

        if (subtitleText != null)
            subtitleText.text = string.Empty;

        if (subtitlePanel != null)
            subtitlePanel.SetActive(false);
    }
}