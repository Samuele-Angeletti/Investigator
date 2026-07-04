/// <summary>
/// Sorgente di dialoghi. Nasconde a chi mostra il dialogo <b>da dove</b> questo
/// arrivi: SO fisso, pool casuale, selezione guidata dalle prove, ecc.
/// È il seam che permette all'interazione e alla UI di ignorare del tutto la
/// categoria e la provenienza del dialogo.
/// </summary>
public interface IDialogueProvider
{
    /// <summary>
    /// Tenta di fornire il prossimo dialogo da mostrare.
    /// </summary>
    /// <param name="dialogue">Il dialogo selezionato, se presente.</param>
    /// <returns><c>true</c> se è disponibile un dialogo, altrimenti <c>false</c>.</returns>
    bool TryGetDialogue(out DialogueData dialogue);
}
