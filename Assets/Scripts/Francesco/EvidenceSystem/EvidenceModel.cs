using System;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;

public class EvidenceModel : MonoBehaviour, IInteractable
{
    [SerializeField] private EvidenceNode _evidenceNode; 
    public EvidenceNode EvidenceNode
    {
        get => _evidenceNode; 
        set => _evidenceNode = value; 
    }

    [SerializeField, Range(0f, 1f)]
    private float _visibility = 0f; 

   
    public float Visibility => _visibility;

    private bool _isCollected; 
    public bool IsCollected => _isCollected; 

    [SerializeField] private Behaviour _outlineEffect;

    [SerializeField] private Renderer _evidenceRenderer;


    private MaterialPropertyBlock _propBlock;
    private int _colorPropId;
    private Color _originalColor;

    private void OnEnable()
    {
        AttemptRegistration();
    }
    private void Awake()
    {
        if(_evidenceNode.EvidenceType==EEvidenceType.FOOTSTEPS)
        {
            _evidenceRenderer = GetComponent<Renderer>();
        }
       
        if (_evidenceRenderer!=null) 
        {
           _propBlock= new MaterialPropertyBlock();

            _colorPropId = Shader.PropertyToID("_Color");

            _originalColor= _evidenceRenderer.sharedMaterial.GetColor(_colorPropId);

            UpdateEvidenceAlpha(0f);
        }
    }

    public void UpdateEvidenceAlpha(float visibility)
    {
        if (_evidenceRenderer == null) return;

        bool shouldBeEnabled = visibility > 0.01f;

        if(_evidenceRenderer.enabled!=shouldBeEnabled)
        {
            _evidenceRenderer.enabled= shouldBeEnabled;
        }

        if (!shouldBeEnabled) return;

        Color newColor= _originalColor;
        newColor.a = visibility;

        _evidenceRenderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor(_colorPropId, newColor);
        _evidenceRenderer.SetPropertyBlock(_propBlock);
    }

    private void Start()
    {
        AttemptRegistration();
    }

    private void AttemptRegistration()
    {
       
        if (LensManager.Instance != null)
        {
            LensManager.Instance.RegisterEvidence(this);
        }
    }

    private void OnDisable()
    {
       
        if (LensManager.Instance != null)
        {
            LensManager.Instance.UnregisterEvidence(this);
        }

       
        Highlight(false);
    }

   
    public void Highlight(bool state)
    {
        if (_outlineEffect != null)
        {
            _outlineEffect.enabled = state;
        }
    }

  
    public void AddVisibility(float visibility)
    {
        _visibility += visibility; 
        _visibility = Mathf.Clamp01(_visibility);
    }

    public void Initialize(EvidenceNode evidenceNode)
    {
        _evidenceNode = evidenceNode; 
        _isCollected = false;
        _visibility = 0f; 
    }

    public void Interact()
    {
        Journal.Instance.AddEvidence(_evidenceNode);
        Destroy(gameObject);
    }
}