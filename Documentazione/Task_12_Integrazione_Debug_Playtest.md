# Task 12 - Integrazione, Debug e Playtest

## Descrizione breve

Collegare tutti i sistemi, verificare il loop completo e aggiungere strumenti minimi di debug. Questa task serve a trasformare le parti separate in un prototipo giocabile.

## Obiettivo didattico

Imparare a integrare sistemi sviluppati da persone diverse e testare rapidamente un gameplay loop.

## Checklist

- Creare un `PrototypeGameManager`.
- Avviare generazione del caso a inizio gioco.
- Collegare player, citta, NPC, evidence, journal e accusa.
- Aggiungere seed visibile o configurabile.
- Aggiungere comando per rigenerare caso.
- Aggiungere debug lista indizi generati.
- Eseguire playtest completo da inizio a fine caso.
- Correggere blocchi critici di collisioni, interazioni e UI.

## Acceptance criteria

- Il prototipo parte senza configurazioni manuali in editor.
- E possibile giocare un caso dall'inizio alla fine.
- Il player puo trovare prove, parlare con NPC, aprire journal e accusare.
- Il seed puo essere cambiato per generare un caso diverso.
- Gli sviluppatori possono usare il debug per capire dove sono gli indizi.

## Pseudo codice

```pseudo
StartPrototype():
    seed = GetSeedOrRandom()

    City.Setup(seed)
    currentCase = CaseGenerator.GenerateCase(seed)

    EvidenceSystem.Clear()

    for each evidence in currentCase.evidences:
        EvidenceSystem.RegisterEvidence(evidence)

    NpcSystem.SpawnOrSetupNPCs(currentCase)
    Journal.Clear()
    AccusationSystem.SetCurrentCase(currentCase)

DebugRegenerateCase():
    ClearCurrentPrototype()
    StartPrototype()
```

## Note

Questa task dovrebbe partire presto come scheletro minimo e continuare fino alla fine della jam. L'integrazione non va lasciata tutta all'ultima ora.
