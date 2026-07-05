using UnityEngine;
using System.Collections.Generic;
using StarterAssets;
using System.Collections;
using Unity.Cinemachine;


public class LensManager : MonoBehaviour
{
   
    public static LensManager Instance { get; private set; }

    [Header("Investigation Settings")]
    [Tooltip("Raggio massimo entro cui il giocatore può rilevare gli indizi (in metri).")]
    [SerializeField] private float _revealRadius = 10f;

    [Tooltip("Soglia di visibilità minima richiesta dall'indizio per essere evidenziato.")]
    [SerializeField, Range(0f, 1f)] private float _minVisibility = 0.2f;

    private bool _isInvestigationModeActive = false;
    public bool IsInvestigationModeActive => _isInvestigationModeActive;

    
    private FirstPersonController _playerController;

   
    public List<EvidenceModel> _activeEvidenceModels = new List<EvidenceModel>();

   
    private float _revealRadiusSqr;

    private float _distanceSqr;

    [Header("Investigation Mode Camera Settings")]
    [SerializeField] private CinemachineCamera _zoomCamera;
    [SerializeField] private GameObject _lensObject;

     

    private const int PRIORITY_ACTIVE = 15;
    private const int PRIORITY_INACTIVE = 9;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        
        _playerController = Object.FindFirstObjectByType<FirstPersonController>();
        if (_playerController == null)
        {
            Debug.LogError("LensManager: Nessun FirstPersonController trovato nella scena!");
        }
        _revealRadiusSqr = _revealRadius * _revealRadius;

        _lensObject.SetActive(false);
    }

   
    public void RegisterEvidence(EvidenceModel evidence)
    {
        if (!_activeEvidenceModels.Contains(evidence))
        {
            _activeEvidenceModels.Add(evidence);
        }
    }

    public void UnregisterEvidence(EvidenceModel evidence)
    {
        if (_activeEvidenceModels.Contains(evidence))
        {
            _activeEvidenceModels.Remove(evidence);
        }
    }

   
    public void ToggleInvestigationMode()
    {
        _isInvestigationModeActive = !_isInvestigationModeActive;

        
        if (UiManager.Instance != null)
        {
            UiManager.Instance.ToggleInvestigationMode(_isInvestigationModeActive);
        }

        if(_zoomCamera!=null)
        {
            _zoomCamera.Priority = _isInvestigationModeActive ? PRIORITY_ACTIVE : PRIORITY_INACTIVE;
        }

        if(_lensObject!=null)
        {
            _lensObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("LensManager: Riferimento a _zoomCamera mancante nell'Inspector!");
        }


        if (!_isInvestigationModeActive)
        {
            DisableAllHighlights();
        }
    }

    

    private void Update()
    {
       
        if (!_isInvestigationModeActive || _playerController == null) return;
        Debug.Log("Cacca");
        ExecuteInvestigationLoop();
    }

   
    private void ExecuteInvestigationLoop()
    {
        Vector3 playerPosition = _playerController.transform.position;

        for (int i = 0; i < _activeEvidenceModels.Count; i++)
        {
            var evidence = _activeEvidenceModels[i];
            if (evidence == null || evidence.EvidenceNode == null) continue;

            float distanceSqr = (evidence.transform.position - playerPosition).sqrMagnitude;
            bool isWithinRadius = distanceSqr <= _revealRadiusSqr;

            if (evidence.EvidenceNode.EvidenceType == EEvidenceType.FOOTSTEPS)
            {
                if (isWithinRadius)
                {
                    float distance = Mathf.Sqrt(distanceSqr);
                    float proximity = 1f - (distance / _revealRadius);
                    float visibilityGain = proximity * 0.5f * Time.deltaTime;
                    
                    evidence.AddVisibility(visibilityGain);
                }
                else
                {
                    evidence.AddVisibility(-0.2f * Time.deltaTime);
                }

                
                evidence.UpdateEvidenceAlpha(evidence.Visibility);
            }
            else
            {
                bool canRead = evidence.Visibility >= _minVisibility;
                if (canRead && isWithinRadius)
                {
                    evidence.Highlight(true);
                }
            }
        }
    }


    

    private void DisableAllHighlights()
    {
        foreach (var evidence in _activeEvidenceModels)
        {
            if (evidence != null)
            {
                evidence.Highlight(false);

                if(evidence.EvidenceNode != null && evidence.EvidenceNode.EvidenceType==EEvidenceType.FOOTSTEPS)
                {
                    evidence.UpdateEvidenceAlpha(0f);
                }
            }
        }

        _lensObject.SetActive(false);
    }
}
