using StarterAssets;
using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour, IInteractable
{
    public Transform Enter = null;
    public UnityEvent onInteract;
    public void Interact()
    {
        if (Enter == null) return;
        onInteract?.Invoke();
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
            player.transform.position = Enter.position;
            player.transform.rotation = Enter.rotation;
            cc.enabled = true;
        }
    }
}