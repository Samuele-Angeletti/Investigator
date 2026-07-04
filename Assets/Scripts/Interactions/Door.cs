using StarterAssets;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Door : MonoBehaviour, IInteractable
{
    private Transform _activeWaypoint = null;
    private bool _canInteract = false;

    public void EnterInDoor(Transform targetWaypoint)
    {
        _activeWaypoint = targetWaypoint;
        _canInteract = true;
    }

 
    public void ExitFromDoor()
    {
        _activeWaypoint = null;
        _canInteract = false;
    }

    public void Interact()
    {
        if (!_canInteract || _activeWaypoint == null) return;

        FirstPersonController fpController = FindFirstObjectByType<FirstPersonController>();
        if (fpController != null)
        {
            TeleportPlayer(fpController.gameObject);
        }
    }

    private void TeleportPlayer(GameObject player)
    {
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            player.transform.position = _activeWaypoint.position;
            player.transform.rotation = _activeWaypoint.rotation;
            cc.enabled = true;

          
        }
    }
}