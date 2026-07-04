/// <summary>
/// Vista di un dialogo. Sa solo mostrare e nascondere il contenuto: non conosce
/// NPC, interazione, né la categoria del dialogo.
/// </summary>
public interface IDialoguePresenter
{
    /// <summary>È attualmente visibile un dialogo?</summary>
    bool IsVisible { get; }

    /// <summary>Mostra il dialogo indicato.</summary>
    /// <param name="dialogue">Contenuto da visualizzare.</param>
    void Show(DialogueData dialogue);

    /// <summary>Nasconde il dialogo corrente.</summary>
    void Hide();
}
