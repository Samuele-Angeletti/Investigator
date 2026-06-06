# Task 07 - NPC Statici e Dialoghi Investigativi

## Descrizione breve

Creare NPC interrogabili che forniscono rumor, testimonianze o informazioni utili sul caso. Gli NPC possono essere statici: per il prototipo non serve movimento avanzato.

## Obiettivo didattico

Collegare personaggi, dialoghi e dati investigativi in un loop semplice ma giocabile.

## Checklist

- Creare struttura dati `NpcData`.
- Posizionare alcuni NPC nella citta.
- Assegnare nome, stato informativo e ruolo a ogni NPC.
- Implementare interazione dialogo tramite `IInteractable`.
- Mostrare 1-3 frasi per NPC.
- Collegare alcuni dialoghi a `EvidenceNode`.
- Salvare testimonianze utili nel journal.

## Acceptance criteria

- Il player puo parlare con gli NPC.
- Ogni NPC mostra almeno una frase di dialogo.
- Alcuni NPC danno informazioni utili.
- Una testimonianza utile viene registrata nel journal.
- Gli NPC possono avere stati informativi diversi: S0, S1, S2.

## Pseudo codice

```pseudo
class NpcData:
    id
    displayName
    awarenessState
    knownEvidence
    genericRumor

OnInteractNPC(npc):
    if npc.awarenessState == S0:
        Dialogue.Show("Non so nulla.")

    else if npc.awarenessState == S1:
        Dialogue.Show(npc.genericRumor)

    else if npc.awarenessState == S2:
        evidence = npc.knownEvidence
        Dialogue.Show(evidence.description)
        Journal.Add(evidence)
```

## Note

Per la jam e meglio puntare su pochi NPC riconoscibili invece che su una folla numerosa.
