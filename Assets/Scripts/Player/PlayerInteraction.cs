using StarterAssets;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private float _interactionDistance = 3.0f;
    [SerializeField] private LayerMask _interactableLayer;

    

    private Camera _mainCamera;
    private StarterAssetsInputs _inputs;



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

        if(Physics.Raycast(ray,out hit,_interactionDistance,_interactableLayer))
        {
            IInteractable interactable= hit.collider.GetComponent<IInteractable>();

            if(interactable != null) 
            {
                UiManager.Instance.TogglePrompt(true);

                if(_inputs.interact)
                {
                    interactable.Click();
                    _inputs.interact = false;
                }

                return;
            }

            if(UiManager.Instance!=null)
            {
                UiManager.Instance.TogglePrompt(false);
            }
            if(_inputs!=null)
            {
                _inputs.interact = false;
            }

        }
    }

   
}
