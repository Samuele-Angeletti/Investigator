using UnityEngine;

public class EvidenceModel : MonoBehaviour//, IInteractable
{
    [SerializeField] private EvidenceNode _evidenceNode;
    public EvidenceNode EvidenceNode
    {
        get
        {
            return _evidenceNode;
        }
        set
        {
            _evidenceNode = value;
        }
    }

    [SerializeField, Range(0f, 1f)]
    private float _visibility = 0f;
    private bool _isCollected;
    public bool IsCollected => _isCollected;

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