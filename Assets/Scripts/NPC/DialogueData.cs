using UnityEngine;

/// <summary>
/// Asset di dialogo statico e non localizzato, pescato a runtime da un
/// <see cref="IDialogueProvider"/>. Contiene solo il contenuto da mostrare;
/// la logica di selezione (evidence / neutro / fuori pista) vive altrove.
/// </summary>
[CreateAssetMenu(fileName = "DialogueData", menuName = "Investigation/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [SerializeField] private EDialogueKind _kind = EDialogueKind.Neutral;
    [SerializeField] private string _speakerName;
    [SerializeField, TextArea(3, 10)] private string _body;

    /// <summary>Categoria semantica per l'evidence system (non per la UI).</summary>
    public EDialogueKind Kind => _kind;

    /// <summary>Nome mostrato di chi parla. Può essere vuoto.</summary>
    public string SpeakerName => _speakerName;

    /// <summary>Testo completo del dialogo, mostrato in un'unica schermata.</summary>
    public string Body => _body;
}
