# Task 11 - Sistema Accusa e Fine Caso

## Descrizione breve

Permettere al player di scegliere un sospetto come colpevole e ricevere un esito. Questa task chiude il loop investigativo del prototipo.

## Obiettivo didattico

Completare il ciclo di gioco: esplorazione, raccolta prove, deduzione, scelta finale.

## Checklist

- Creare UI con lista sospetti.
- Aggiungere comando o bottone per accusare.
- Confrontare il sospetto scelto con il colpevole nel `CaseData`.
- Mostrare schermata di vittoria o sconfitta.
- Mostrare il colpevole corretto a fine caso.
- Permettere restart o rigenerazione del caso.

## Acceptance criteria

- Il player puo selezionare un sospetto.
- Il player puo confermare un'accusa.
- Se il sospetto e corretto, compare esito positivo.
- Se il sospetto e sbagliato, compare esito negativo.
- Il caso puo essere riavviato o rigenerato dopo la fine.

## Pseudo codice

```pseudo
Accuse(suspect):
    if suspect == currentCase.culprit:
        result = "Caso risolto"
    else:
        result = "Accusa sbagliata"

    EndCase(result)

EndCase(result):
    ShowEndScreen(result)
    ShowCorrectCulprit(currentCase.culprit)
    ShowCollectedEvidenceSummary(Journal.collectedEvidences)
    ShowRestartButton()
```

## Note

Questa task e fondamentale per rendere il prototipo giocabile dall'inizio alla fine.
