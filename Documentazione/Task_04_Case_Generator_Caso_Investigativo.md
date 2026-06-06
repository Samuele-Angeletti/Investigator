# Task 04 - Case Generator e Caso Investigativo

## Descrizione breve

Implementare un generatore di caso semplice. Il sistema sceglie un colpevole, un luogo del delitto, un percorso e alcuni indizi collegati. Il risultato deve essere un caso risolvibile in pochi minuti.

## Obiettivo didattico

Costruire una struttura dati narrativa procedurale che resti coerente e giocabile.

## Checklist

- Creare struttura `CaseData`.
- Creare lista di sospetti disponibili.
- Selezionare colpevole tramite seed.
- Selezionare luogo del delitto.
- Generare o richiedere un percorso del colpevole.
- Creare 3-5 indizi collegati al colpevole.
- Rendere il caso accessibile agli altri sistemi.

## Acceptance criteria

- A ogni nuova partita viene creato un caso.
- Il caso ha un colpevole corretto.
- Il caso ha almeno un luogo del delitto.
- Il caso genera almeno 3 indizi utili.
- Gli indizi puntano in modo leggibile verso il colpevole.

## Pseudo codice

```pseudo
class CaseData:
    seed
    victim
    culprit
    crimeLocation
    culpritPath
    evidences

GenerateCase(seed):
    Random.Init(seed)

    caseData = new CaseData()
    caseData.seed = seed
    caseData.victim = "Uomo Ragno"
    caseData.culprit = PickRandom(suspectList)
    caseData.crimeLocation = PickRandom(importantLocations)

    caseData.culpritPath = RandomWalk.Generate(
        start = caseData.crimeLocation,
        biasTargets = caseData.culprit.relatedLocations
    )

    caseData.evidences = CreateEvidenceSet(caseData)

    return caseData
```

## Note

Meglio avere pochi casi semplici ma funzionanti. La coerenza e piu importante della varieta.
