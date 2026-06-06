# GAME DESIGN DOCUMENT (GDD)
## Titolo provvisorio: *Hanno ucciso l’Uomo Ragno – Case Files*

---

# 1. OVERVIEW

## Genere
- Investigativo
- Procedurale
- Narrativo emergente

## Prospettiva
- 3D First Person

## Piattaforme target
- PC (Windows come base principale)
- Possibile espansione: Steam Deck

## Pillar del gameplay
- Investigazione basata su indizi emergenti
- Città statica ma eventi dinamici
- Procedural generation controllata (non caos puro)
- Lettura e interpretazione del mondo

---

# 2. CORE GAME LOOP

1. Il giocatore esplora New York
2. Raccoglie indizi ambientali
3. Interroga NPC
4. Analizza tracce (fisiche + informative)
5. Ricostruisce la sequenza degli eventi
6. Identifica il colpevole
7. Nuovo caso generato

---

# 3. LORE / SETTING

## Premessa narrativa
L’Uomo Ragno è stato trovato morto.

La città non sa esattamente cosa sia successo.

Ogni caso rappresenta una ricostruzione alternativa degli eventi della sua morte, filtrata attraverso:
- memoria collettiva
- distorsione mediatica
- testimonianze frammentate

## Tema centrale
> La verità non è unica, ma emergente dalla ricostruzione dei dati.

## Influenza narrativa
- “Hanno ucciso l’Uomo Ragno” (883) come base mitologica
- New York stilizzata low-poly
- tono tra noir investigativo e surreale urbano

---

# 4. AMBIENTE DI GIOCO

## Struttura città (macro-livello)
- New York su griglia semplificata
- Edifici statici posizionati a coordinate fisse
- Zone principali:
  - Strade
  - Metro
  - Grattacieli
  - Quartieri residenziali
  - Redazioni giornalistiche

---

## Interni (micro-livello)
- Generazione procedurale tramite BSP
- Ogni edificio genera:
  - stanze
  - corridoi
  - punti di interesse

---

## Oggetti ambientali
- Distribuiti tramite Random Walk controllato
- Zone “vissute” emergono dinamicamente:
  - disordine localizzato
  - clustering di oggetti
  - tracce narrative

---

## NPC
- Distribuiti nella città come entità statiche o semi-dinamiche
- Funzione principale:
  - contenitori di informazione
  - punti di dialogo

---

## Folla e stato sociale
- Cellular Automata su stato informativo:
  - S0: ignaro
  - S1: rumor / sentito dire
  - S2: informato (possiede indizio)

---

# 5. SISTEMA INVESTIGATIVO

## Generazione caso
- Sistema di eventi casuali vincolati
- Coerenza interna garantita da template narrativi

Esempio:
- Evento iniziale (morte)
- Evento intermedio (fuga / contatto / testimone)
- Evento finale (conclusione del caso)

---

## Random Walk (tracce del colpevole)
- Simula il percorso “storico” del colpevole
- Non visibile direttamente
- Produce:

### Tipi di tracce:
- fisiche (oggetti, segni)
- logiche (relazioni tra eventi)
- spaziali (posizioni visitate)

---

## Sistema di rivelazione
- Il giocatore non vede tutto subito
- Indizi diventano più leggibili con:
  - distanza
  - strumenti (lente investigativa)
  - progresso investigativo

---

# 6. ARCHITETTURA TECNICA

## Scelta generale
- Single Scene principale (World Scene)
- Streaming logico degli edifici
- Nessuna dipendenza da loading screen tradizionali

---

## Sistema edifici
```csharp
class Building
{
    int seed;
    bool isGenerated;
    bool isActive;
    InteriorData data;
}
````

### Pipeline:

1. Near → pre-generation (background)
2. Enter → activation immediata
3. Exit → disattivazione (non destroy obbligatorio)

---

## Procedural systems

### BSP (interni)

* Generazione layout stanze
* Divisione ricorsiva spazio

### Random Walk (tracce)

* Generazione percorsi e residui ambientali
* Bias verso punti narrativi

### Cellular Automata (NPC awareness)

* Diffusione informazione nella popolazione
* Simulazione rumor sociale

---

## Sistema dati unificato

```text
EvidenceNode
- position
- type
- source (BSP / RW / NPC / CA)
- visibility
- truthValue
```

---

# 7. UI / UX

* Modalità investigativa
* Lente di ingrandimento (reveal layer informativo)
* Evidenza visiva delle tracce
* Mappa stilizzata della città
* Journal investigativo automatico

---

# 8. PERFORMANCE STRATEGY

## Obiettivi

* Zero loading percepibile
* Streaming predittivo
* Generazione asincrona

## Strategia

* Pre-generation vicino al player
* Attivazione istantanea all’ingresso
* Un solo edificio attivo per volta (idealmente)

---

# 9. TOOLING UNITY (PACKAGE MANAGER)

## ESSENZIALI (Unity Official)

* Input System
* Cinemachine
* TextMeshPro
* AI Navigation (NavMesh)
* Unity Profiler (built-in)
* Shader Graph (URP consigliato)

---

## CONSIGLIATI (non default)

### 1. Odin Inspector *(Sirenix)*

* Debug e editor tooling avanzato
* Fondamentale per sistemi procedurali complessi

### 2. NaughtyAttributes (open source)

* Lightweight alternative a Odin

---

### 3. GraphView / Node Graph Tools (custom o asset)

* Visualizzazione del grafo investigativo
* Molto utile per debug Random Walk / case graph

---

### 4. DOTween (Demigiant)

* Animazioni UI e transizioni investigative

---

### 5. Unity Addressables

* Streaming avanzato contenuti (utile anche senza scene additive)

---

### 6. A* Pathfinding Project (Aron Granberg) *(opzionale)*

* Se vuoi NPC più intelligenti nel movimento urbano

---

### 7. ProBuilder (Unity)

* Blockout città e interni rapidi

---

# 10. RISCHI E SCELTE CONSAPEVOLI

## Rischi

* Over-simulazione (troppi sistemi che “parlano tra loro”)
* Debug complesso dei sistemi procedurali
* Perdita di leggibilità narrativa

---

## Mitigazioni

* Un solo “Evidence System” centrale
* Tutti i sistemi producono solo varianti dello stesso dato
* Debug overlay obbligatorio (BSP / RW / CA visibili)

---

# 11. CONCLUSIONE

Il progetto si basa su un principio chiave:

> La città è statica. La verità è procedurale.

Tutti i sistemi (BSP, Random Walk, Cellular Automata) non servono a creare caos, ma a creare **diverse letture coerenti dello stesso evento centrale**.

---

# 12. PSEUDO CODICE

## Cellular Automata — NPC Truth Decay Map

Obiettivo: aggiornare periodicamente la “mappa della verità” degli NPC usando il Cellular Automata già esistente.

- `0` = zona ancora viva / indizio presente
- `1` = zona morta / indizio sparito
- più iterazioni = maggiore decadimento dell’indizio

```pseudo
Every N seconds:

    npcPositions = GetCurrentNpcPositionsXZ()

    if npcPositions is empty:
        return currentTruthMap

    bounds = CalculateNpcBounds(npcPositions)

    /*
        bounds viene calcolato usando gli NPC più estremi:

        bottomLeft  = NPC con X minimo e Z minimo
        bottomRight = NPC con X massimo e Z minimo
        topLeft     = NPC con X minimo e Z massimo
        topRight    = NPC con X massimo e Z massimo

        La Y viene ignorata.
    */

    width  = bounds.maxX - bounds.minX
    height = bounds.maxZ - bounds.minZ

    newMap = CreateIntMap(width, height)

    for each cell in newMap:

        worldPosition = ConvertCellToWorldXZ(cell, bounds)

        if IsInsideNpcTruthArea(worldPosition, npcPositions):
            newMap[cell.x, cell.z] = 0
        else:
            newMap[cell.x, cell.z] = 1

    /*
        La nuova mappa non deriva direttamente dalla precedente,
        ma viene rigenerata ogni volta in base alla posizione attuale
        degli NPC.

        In questo modo il Cellular Automata lavora sempre su una
        fotografia aggiornata della folla.
    */

    updatedTruthMap = CellularAutomata.Generate(
        map = newMap,
        iterations = truthDecayIterations
    )

    currentTruthMap = updatedTruthMap

    return currentTruthMap
```

## Note progettuali

La mappa passata al Cellular Automata deve essere ricostruita a ogni ciclo, usando la disposizione corrente degli NPC come area logica di partenza.

Il rettangolo operativo è definito dagli NPC più distanti tra loro nello spazio XZ.

Dopo ogni aggiornamento, alcune zone dell’indizio possono decadere fino a sparire completamente. Questo è accettabile e coerente con il tema:

> “Chi sia stato non si sa.”
