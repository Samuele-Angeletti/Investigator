using TMPro;
using UnityEngine;

/// <summary>
/// Presenter UI a schermata singola: mostra l'intero testo del dialogo in un
/// pannello TextMeshPro. Per scelta di design non c'è avanzamento riga per riga,
/// il testo è mostrato tutto insieme.
/// </summary>
public class DialogueUIPresenter : MonoBehaviour, IDialoguePresenter
{
    [Header("References")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private TMP_Text _speakerLabel;
    [SerializeField] private TMP_Text _bodyLabel;

    /// <inheritdoc />
    public bool IsVisible => _panel != null && _panel.activeSelf;

    private void Awake()
    {
        Hide();
    }

    /// <inheritdoc />
    public void Show(DialogueData dialogue)
    {
        if (dialogue == null)
            return;

        if (_speakerLabel != null)
            _speakerLabel.text = dialogue.SpeakerName;

        if (_bodyLabel != null)
            _bodyLabel.text = dialogue.Body;

        if (_panel != null)
            _panel.SetActive(true);
    }

    /// <inheritdoc />
    public void Hide()
    {
        if (_panel != null)
            _panel.SetActive(false);
    }
}
