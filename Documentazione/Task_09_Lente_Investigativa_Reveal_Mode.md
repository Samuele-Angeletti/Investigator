# Task 09 - Lente Investigativa e Reveal Mode

## Descrizione breve

Implementare una modalita investigativa attivabile dal player. Quando e attiva, gli indizi vicini diventano piu leggibili tramite highlight o feedback visivo.

## Obiettivo didattico

Creare una meccanica di lettura del mondo che aiuti il giocatore senza risolvere automaticamente il caso.

## Checklist

- Aggiungere input per attivare/disattivare la lente.
- Definire raggio di rivelazione.
- Leggere gli indizi dall'`EvidenceSystem`.
- Evidenziare solo gli indizi abbastanza vicini.
- Considerare il valore `visibility` degli indizi.
- Spegnere tutti gli highlight quando la lente e disattivata.
- Aggiungere feedback UI minimo per indicare che la modalita e attiva.

## Acceptance criteria

- Il player puo attivare e disattivare la lente.
- Gli indizi vicini vengono evidenziati.
- Gli indizi lontani non vengono evidenziati.
- Gli indizi con visibilita troppo bassa restano nascosti.
- La lente non aggiunge automaticamente indizi al journal.

## Pseudo codice

```pseudo
When InvestigationModeActive:
    for each evidence in EvidenceSystem.allEvidences:
        distance = Distance(player.position, evidence.position)

        canReveal = distance < revealRadius
        canRead = evidence.visibility >= minVisibility

        if canReveal and canRead:
            evidence.Highlight(true)
        else:
            evidence.Highlight(false)

When InvestigationModeInactive:
    for each evidence in EvidenceSystem.allEvidences:
        evidence.Highlight(false)
```

## Note

Per il prototipo l'highlight puo essere molto semplice: cambio materiale, outline, icona o luce colorata.
