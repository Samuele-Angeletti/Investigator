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

    [Header("Evidence")]
    [Tooltip("Usato solo se Kind == Evidence: la prova a cui questo dialogo si riferisce. " +
             "Il dialogo è selezionabile solo se questa prova è presente nel caso corrente.")]
    [SerializeField] private EvidenceNode _linkedEvidence;

    /// <summary>Categoria semantica per l'evidence system (non per la UI).</summary>
    public EDialogueKind Kind => _kind;

    /// <summary>Nome mostrato di chi parla. Può essere vuoto.</summary>
    public string SpeakerName => _speakerName;

    /// <summary>Testo completo del dialogo, mostrato in un'unica schermata.</summary>
    public string Body => _body;

    /// <summary>Prova collegata (rilevante solo per i dialoghi di tipo Evidence).</summary>
    public EvidenceNode LinkedEvidence => _linkedEvidence;

    /// <summary>È un dialogo che rivela una prova?</summary>
    public bool IsEvidence => _kind == EDialogueKind.Evidence;
}
