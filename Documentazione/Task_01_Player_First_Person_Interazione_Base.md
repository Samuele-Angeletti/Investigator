# Task 01 - Player First Person e Interazione Base

## Descrizione breve

Implementare il movimento in prima persona, la camera e un sistema di interazione generico tramite raycast. Questa task crea la base con cui il giocatore esplora la citta, raccoglie indizi, parla con NPC e usa porte o trigger.

## Obiettivo didattico

Capire come collegare input, movimento, camera e interazioni in un prototipo Unity giocabile.

## Checklist

- Configurare movimento WASD.
- Configurare mouse look in prima persona.
- Aggiungere un `CharacterController` o controller equivalente.
- Creare interfaccia comune `IInteractable`.
- Implementare raycast dal centro della camera.
- Mostrare un prompt quando si guarda un oggetto interagibile.
- Eseguire l'interazione quando viene premuto il tasto dedicato.

## Acceptance criteria

- Il player puo muoversi liberamente nella scena.
- Il player puo ruotare la visuale con il mouse.
- Guardando un oggetto interagibile appare un prompt.
- Premendo il tasto interazione viene chiamata la logica corretta dell'oggetto.
- Il sistema funziona con almeno tre tipi diversi di oggetti: indizio, NPC e porta.

## Pseudo codice

```pseudo
Every Frame:
    movementInput = ReadMovementInput()
    lookInput = ReadMouseInput()

    MovePlayer(movementInput)
    RotateCamera(lookInput)

    ray = Camera.CenterRay()
    hit = Raycast(ray, interactionDistance)

    if hit has IInteractable:
        currentInteractable = hit.GetComponent(IInteractable)
        ShowPrompt(currentInteractable.GetInteractionText())

        if InteractButtonPressed:
            currentInteractable.Interact(player)
    else:
        currentInteractable = null
        HidePrompt()
```

## Note

Questa task e una dipendenza per quasi tutte le altre. Meglio completarla presto e mantenerla semplice.
