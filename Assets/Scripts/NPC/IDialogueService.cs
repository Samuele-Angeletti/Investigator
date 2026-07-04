using System;

/// <summary>
/// Servizio di orchestrazione dei dialoghi. Fa da mediator tra la sorgente
/// (<see cref="IDialogueProvider"/>) e la vista (<see cref="IDialoguePresenter"/>),
/// notificando il chiamante quando il dialogo termina.
/// </summary>
public interface IDialogueService
{
    /// <summary>È attualmente in corso un dialogo?</summary>
    bool IsOpen { get; }

    /// <summary>
    /// Avvia un dialogo pescandolo dal provider. Se il provider non fornisce
    /// alcun dialogo, <paramref name="onComplete"/> viene invocato subito.
    /// </summary>
    /// <param name="provider">Sorgente da cui pescare il dialogo.</param>
    /// <param name="onComplete">Callback invocata alla chiusura del dialogo.</param>
    void Begin(IDialogueProvider provider, Action onComplete);
}
