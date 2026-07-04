/// <summary>
/// Categoria semantica di un dialogo. È consumata dall'evidence system per
/// classificare l'informazione ottenuta da un NPC. Il layer di presentazione
/// NON deve mai ramificare su questo valore: mostra il testo e basta.
/// </summary>
public enum EDialogueKind
{
    /// <summary>Dialogo neutro, senza impatto investigativo.</summary>
    Neutral,

    /// <summary>Dialogo che rivela una prova rilevante per il caso.</summary>
    Evidence,

    /// <summary>Dialogo fuorviante, pensato per mandare fuori pista.</summary>
    Misleading
}
