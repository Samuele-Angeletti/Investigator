using StarterAssets;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private float _interactionDistance = 1.0f;
    [SerializeField] private LayerMask _interactableLayer;

    private Camera _mainCamera;
    private StarterAssetsInputs _inputs;

    //Debug Variables
    private Vector3 _gizmoStart;
    private Vector3 _gizmoDirection;
    
    
    private bool _hitSomething;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _inputs = GetComponent<StarterAssetsInputs>();
    }

    private void Update()
    {
        CheckForInteractables();
    }

    private void CheckForInteractables()
    {
        Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

       
        _gizmoStart = ray.origin;
        _gizmoDirection = ray.direction;

        if (Physics.Raycast(ray, out hit, _interactionDistance, _interactableLayer))
        {
            _hitSomething = true; 
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                if(UiManager.Instance!=null)
                {
                    UiManager.Instance.SetReticleState(true);
                    UiManager.Instance.TogglePrompt(true);
                }
                if (_inputs.interact)
                {
                    interactable.Click();
                    _inputs.interact = false;
                }
                return;
            }
        }
        _hitSomething = false; 
        if(UiManager.Instance != null)
        {
            UiManager.Instance.SetReticleState(false);
            UiManager.Instance.TogglePrompt(false);
        }
        
        
        //Avoid repetions of interact
        if (_inputs != null)
        {
            _inputs.interact = false;
        }
    }

#if UNITY_EDITOR
  
    private void OnDrawGizmos()
    {
        if (_mainCamera == null) return;

       
        Gizmos.color = _hitSomething ? Color.green : Color.red;

       
        Gizmos.DrawRay(_gizmoStart, _gizmoDirection * _interactionDistance);

       
        Gizmos.DrawWireSphere(_gizmoStart + (_gizmoDirection * _interactionDistance), 0.05f);
    }
#endif
}