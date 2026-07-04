using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implementazione di <see cref="IDialogueProvider"/> per un NPC: sceglie un
/// dialogo da un pool assegnato in Inspector. La strategia di selezione è
/// isolata in <see cref="Select"/>, così può essere sostituita (es. selezione
/// guidata dall'evidence system) senza toccare interazione o UI.
/// </summary>
public class NpcDialogueSource : MonoBehaviour, IDialogueProvider
{
    [SerializeField] private List<DialogueData> _pool = new();

    /// <inheritdoc />
    public bool TryGetDialogue(out DialogueData dialogue)
    {
        dialogue = Select(_pool);
        return dialogue != null;
    }

    /// <summary>
    /// Punto di estensione per la logica di selezione. Default: scelta casuale.
    /// Qui va agganciata la selezione reale (es. in base allo stato del caso)
    /// senza che nessun altro sistema debba cambiare.
    /// </summary>
    /// <param name="pool">Dialoghi disponibili per questo NPC.</param>
    /// <returns>Il dialogo scelto, oppure <c>null</c> se il pool è vuoto.</returns>
    protected virtual DialogueData Select(IReadOnlyList<DialogueData> pool)
    {
        if (pool == null || pool.Count == 0)
            return null;

        int index = Random.Range(0, pool.Count);
        return pool[index];
    }
}
