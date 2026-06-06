# Task 02 - Mini Citta Statica ed Edifici Entrabili

## Descrizione breve

Creare una piccola porzione di New York stilizzata usando gli asset gia pronti. La citta deve contenere pochi punti chiave, alcuni edifici riconoscibili e almeno un edificio entrabile.

## Obiettivo didattico

Impostare una scena esplorabile e collegare luoghi fisici a dati di gameplay come seed, interni e punti investigativi.

## Checklist

- Creare una piccola area urbana giocabile.
- Posizionare strade, edifici e landmark con asset gia pronti.
- Definire 3-5 edifici importanti.
- Aggiungere collider e trigger di ingresso.
- Associare ogni edificio a un seed.
- Preparare almeno un interno attivabile.
- Disattivare gli interni quando il player e fuori.

## Acceptance criteria

- Il player puo esplorare una piccola area urbana.
- Almeno un edificio e entrabile.
- Ogni edificio importante ha un nome o identificativo.
- L'ingresso in un edificio attiva il relativo interno senza cambio scena.
- L'uscita dall'edificio disattiva o nasconde l'interno.

## Pseudo codice

```pseudo
class Building:
    id
    displayName
    seed
    isGenerated
    isActive
    interiorRoot

OnPlayerNearBuilding(building):
    if building.isGenerated == false:
        building.interiorRoot = InteriorGenerator.Generate(building.seed)
        building.isGenerated = true

OnPlayerEnterBuilding(building):
    building.interiorRoot.SetActive(true)
    building.isActive = true

OnPlayerExitBuilding(building):
    building.interiorRoot.SetActive(false)
    building.isActive = false
```

## Note

Per il prototipo basta una citta piccola ma leggibile. Non serve simulare tutta New York.
