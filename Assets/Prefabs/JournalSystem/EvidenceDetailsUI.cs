using System.Linq;
using TMPro;
using UnityEngine;

public class EvidenceDetailsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textName;
    [SerializeField] private TextMeshProUGUI _textType;
    [SerializeField] private TextMeshProUGUI _textTruthValue;
    [SerializeField] private TextMeshProUGUI _textTags;
    [SerializeField] private TextMeshProUGUI _textDescription;

    public void SetUp(EvidenceNode evidenceNode)
    {
        if (evidenceNode == null)
        {
            _textName.text = string.Empty;
            _textType.text = string.Empty;
            _textTruthValue.text = string.Empty;
            _textTags.text = string.Empty;
            _textDescription.text = string.Empty;
            return;
        }

        _textName.text = evidenceNode.name;
        _textType.text = $"Type: {evidenceNode.EvidenceType}";
        _textTruthValue.text = $"Truth Value: {evidenceNode.TruthValue}";
        _textTags.text = $"Tags: {string.Join(",", evidenceNode.EvidenceTags.Select(tag => tag.name))}";
        _textDescription.text = $"Description: {evidenceNode.Description}";
    }
}