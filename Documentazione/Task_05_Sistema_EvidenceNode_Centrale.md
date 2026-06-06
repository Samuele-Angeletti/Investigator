# Task 05 - Sistema EvidenceNode Centrale

## Descrizione breve

Implementare una struttura unica per rappresentare tutti gli indizi: oggetti fisici, testimonianze NPC, tracce spaziali e informazioni generate dagli algoritmi procedurali.

## Obiettivo didattico

Separare i dati investigativi dalla loro rappresentazione in scena o in UI.

## Checklist

- Creare classe o struct `EvidenceNode`.
- Gestire posizione, tipo, sorgente, visibilita e valore di verita.
- Collegare ogni indizio a un sospetto o luogo.
- Creare un `EvidenceSystem` centrale.
- Registrare tutti gli indizi generati dal caso.
- Rendere gli indizi raccoglibili.
- Inviare gli indizi raccolti al journal.

## Acceptance criteria

- Tutti gli indizi usano la stessa struttura dati.
- Gli indizi possono essere generati da sistemi diversi.
- Il player puo raccogliere almeno un indizio fisico.
- Un indizio raccolto viene salvato e non duplicato.
- Il journal riceve correttamente gli indizi raccolti.

## Pseudo codice

```pseudo
class EvidenceNode:
    id
    position
    type
    source
    visibility
    truthValue
    description
    linkedSuspect
    isCollected

class EvidenceSystem:
    allEvidences

    RegisterEvidence(evidence):
        allEvidences.Add(evidence)
        SpawnEvidenceObject(evidence)

    CollectEvidence(evidence):
        if evidence.isCollected:
            return

        evidence.isCollected = true
        Journal.Add(evidence)
```

## Note

Questa task e centrale: BSP, Random Walk, NPC e Cellular Automata dovrebbero produrre o modificare dati compatibili con `EvidenceNode`.
