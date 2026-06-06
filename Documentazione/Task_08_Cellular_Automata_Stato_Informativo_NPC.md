# Task 08 - Cellular Automata per Stato Informativo NPC

## Descrizione breve

Usare e adattare l'algoritmo Cellular Automata fatto in classe per simulare la diffusione delle informazioni tra NPC. Il sistema modifica lo stato informativo degli NPC nel tempo.

## Obiettivo didattico

Applicare un automa cellulare a un sistema narrativo semplice: rumor, testimonianze e conoscenza degli indizi.

## Checklist

- Usare e adattare l'algoritmo Cellular Automata fatto in classe.
- Definire stati NPC: S0 ignaro, S1 rumor, S2 informato.
- Creare una griglia logica della citta.
- Associare ogni NPC a una cella.
- Aggiornare la griglia ogni N secondi.
- Aggiornare lo stato informativo degli NPC.
- Aggiungere debug visual opzionale per le celle.

## Acceptance criteria

- Gli NPC hanno uno stato informativo leggibile.
- Lo stato puo cambiare nel tempo.
- Alcuni NPC possono diventare piu utili dopo la diffusione del rumor.
- Il sistema non blocca il gameplay.
- Il numero di iterazioni e configurabile da Inspector.

## Pseudo codice

```pseudo
Every N seconds:
    infoMap = BuildMapFromNpcPositions(npcs)

    updatedMap = CellularAutomata.Generate(
        map = infoMap,
        iterations = awarenessIterations
    )

    for each npc in npcs:
        cell = WorldToCell(npc.position)
        value = updatedMap[cell.x, cell.z]

        if value == 2:
            npc.awarenessState = S2
        else if value == 1:
            npc.awarenessState = S1
        else:
            npc.awarenessState = S0
```

## Note

Il GDD parla anche di decadimento della verita. Per il prototipo basta che il sistema faccia cambiare alcuni dialoghi in modo percepibile.
