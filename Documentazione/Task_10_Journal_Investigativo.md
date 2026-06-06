# Task 10 - Journal Investigativo

## Descrizione breve

Creare una UI semplice che raccoglie automaticamente indizi, testimonianze e sospetti. Il journal deve aiutare il player a ricordare cosa ha scoperto e a scegliere chi accusare.

## Obiettivo didattico

Costruire una UI funzionale collegata ai dati di gameplay.

## Checklist

- Creare pannello journal apribile e chiudibile.
- Mostrare lista degli indizi raccolti.
- Mostrare lista dei sospetti.
- Mostrare dettaglio dell'indizio selezionato.
- Evitare duplicati nella lista.
- Collegare il journal all'`EvidenceSystem`.
- Preparare accesso alla schermata di accusa o al sistema di fine caso.

## Acceptance criteria

- Il player puo aprire e chiudere il journal.
- Ogni indizio raccolto compare nel journal.
- Le testimonianze NPC possono comparire nel journal.
- Selezionando un indizio si vede una descrizione.
- La UI resta leggibile durante il prototipo.

## Pseudo codice

```pseudo
class Journal:
    collectedEvidences
    suspects

    Add(evidence):
        if collectedEvidences.Contains(evidence):
            return

        collectedEvidences.Add(evidence)
        RefreshUI()

    RefreshUI():
        ClearEvidenceList()

        for each evidence in collectedEvidences:
            CreateEvidenceButton(evidence.description)

    OnEvidenceSelected(evidence):
        ShowEvidenceDetails(evidence)
```

## Note

Non serve una UI complessa. Deve essere veloce da usare e utile per completare il caso.
