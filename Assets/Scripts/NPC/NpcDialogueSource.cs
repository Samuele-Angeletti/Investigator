using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implementazione di <see cref="IDialogueProvider"/> per un NPC: sceglie un
/// dialogo da un pool assegnato in Inspector, tra quelli attualmente ammissibili.
/// La strategia di selezione è isolata in <see cref="Select"/>, così potrà essere
/// pilotata in seguito dal Cellular Automata senza toccare interazione o UI.
/// </summary>
public class NpcDialogueSource : MonoBehaviour, IDialogueProvider
{
    [SerializeField] private List<DialogueData> _pool = new();

    // Riutilizzato tra le chiamate per evitare allocazioni a ogni interazione.
    private readonly List<DialogueData> _candidatesBuffer = new();

    /// <inheritdoc />
    public bool TryGetDialogue(out DialogueData dialogue)
    {
        dialogue = Select(_pool);
        return dialogue != null;
    }

    /// <summary>
    /// Sceglie un dialogo tra quelli ammissibili del pool.
    /// </summary>
    /// <param name="pool">Dialoghi disponibili per questo NPC.</param>
    /// <returns>Il dialogo scelto, oppure <c>null</c> se nessuno è ammissibile.</returns>
    protected virtual DialogueData Select(IReadOnlyList<DialogueData> pool)
    {
        if (pool == null || pool.Count == 0)
            return null;

        _candidatesBuffer.Clear();

        for (int i = 0; i < pool.Count; i++)
        {
            DialogueData dialogue = pool[i];
            if (dialogue == null)
                continue;

            // Guard permanente: un dialogo Evidence è ammesso solo se la sua prova
            // è presente nel caso corrente (vedi IsEligible).
            if (!IsEligible(dialogue))
                continue;

            // TEMPORANEO: finché la scelta non sarà pilotata dal Cellular Automata,
            // l'NPC fornisce solo dialoghi neutri. Rimuovere questa riga per
            // riabilitare Evidence / Misleading.
            if (dialogue.Kind != EDialogueKind.Neutral)
                continue;

            _candidatesBuffer.Add(dialogue);
        }

        if (_candidatesBuffer.Count == 0)
            return null;

        return _candidatesBuffer[Random.Range(0, _candidatesBuffer.Count)];
    }

    /// <summary>
    /// Determina se un dialogo può essere mostrato in base allo stato di gioco.
    /// Neutral e Misleading sono sempre ammissibili; Evidence lo è solo se la prova
    /// collegata esiste nel caso corrente (<see cref="EvidenceSystem.ContainsEvidence"/>).
    /// </summary>
    protected bool IsEligible(DialogueData dialogue)
    {
        switch (dialogue.Kind)
        {
            case EDialogueKind.Evidence:
                return dialogue.LinkedEvidence != null
                    && EvidenceSystem.Instance != null
                    && EvidenceSystem.Instance.ContainsEvidence(dialogue.LinkedEvidence);

            case EDialogueKind.Neutral:
            case EDialogueKind.Misleading:
            default:
                return true;
        }
    }
}
