# Task 06 - Random Walk per Tracce del Colpevole

## Descrizione breve

Usare e adattare l'algoritmo Random Walk fatto in classe per generare il percorso storico del colpevole. Il percorso non deve essere mostrato direttamente, ma deve produrre tracce e indizi.

## Obiettivo didattico

Usare un algoritmo procedurale per creare una sequenza spaziale che abbia significato investigativo.

## Checklist

- Usare e adattare l'algoritmo Random Walk fatto in classe.
- Generare un percorso partendo dal luogo del delitto.
- Aggiungere bias verso luoghi narrativi importanti.
- Selezionare alcuni step del percorso come punti indizio.
- Creare almeno 3 tracce lungo il percorso.
- Collegare le tracce al `CaseData`.
- Registrare le tracce nell'`EvidenceSystem`.

## Acceptance criteria

- Il percorso cambia con seed diverso.
- Lo stesso seed produce lo stesso percorso.
- Almeno 3 indizi sono posizionati tramite Random Walk.
- Gli indizi formano una pista interpretabile dal player.
- Il percorso non e visibile direttamente senza debug.

## Pseudo codice

```pseudo
GenerateCulpritTrail(caseData):
    path = RandomWalk.Generate(
        start = caseData.crimeLocation,
        steps = trailLength,
        biasTargets = caseData.culprit.relatedLocations
    )

    keySteps = SelectImportantSteps(path, count = 3)

    for each step in keySteps:
        evidence = CreateEvidenceNode()
        evidence.position = step.position
        evidence.type = PickTrailEvidenceType()
        evidence.source = "RandomWalk"
        evidence.linkedSuspect = caseData.culprit

        EvidenceSystem.RegisterEvidence(evidence)

    return path
```

## Note

Il Random Walk deve servire la leggibilita del caso. Se il risultato e troppo caotico, ridurre la casualita e aumentare il bias.
