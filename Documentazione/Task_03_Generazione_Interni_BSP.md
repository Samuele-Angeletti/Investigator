# Task 03 - Generazione Interni BSP

## Descrizione breve

Usare e adattare l'algoritmo BSP fatto in classe per generare interni semplici: stanze, corridoi e punti di interesse. Gli interni devono essere esplorabili e riproducibili tramite seed.

## Obiettivo didattico

Applicare un algoritmo procedurale gia studiato a un problema concreto di level design.

## Checklist

- Usare e adattare l'algoritmo BSP fatto in classe.
- Generare un layout composto da stanze e corridoi.
- Creare floor, pareti e porte o placeholder equivalenti.
- Inserire punti di interesse nelle stanze.
- Esporre seed e parametri principali in Inspector.
- Collegare il generatore agli edifici della citta.
- Aggiungere un debug visual opzionale per stanze e corridoi.

## Acceptance criteria

- Un edificio puo generare un interno tramite BSP.
- Lo stesso seed genera sempre lo stesso layout.
- Seed diversi producono layout diversi.
- Il player puo camminare negli interni generati.
- Almeno una stanza contiene un punto di interesse utilizzabile da altri sistemi.

## Pseudo codice

```pseudo
GenerateInterior(seed, buildingBounds):
    Random.Init(seed)

    rootArea = CreateRectangle(buildingBounds.width, buildingBounds.depth)
    partitions = BSP.Split(rootArea, minRoomSize, maxIterations)

    rooms = []

    for each partition in partitions:
        room = CreateRoomInside(partition)
        rooms.Add(room)
        SpawnFloor(room)
        SpawnWalls(room)

    corridors = ConnectRooms(rooms)

    for each corridor in corridors:
        SpawnCorridor(corridor)

    for each room in rooms:
        if RandomChance(pointOfInterestChance):
            SpawnPointOfInterest(room.center)

    return InteriorData(rooms, corridors)
```

## Note

Non serve creare interni perfetti. Per la jam e sufficiente che siano navigabili, coerenti e utili al posizionamento di indizi.
