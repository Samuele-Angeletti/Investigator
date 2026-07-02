
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; set; }

    [Header("Ui Ref")]
    [SerializeField] private GameObject _interactionPrompt;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private GameObject _investigationPrompt;
    [SerializeField] private TextMeshProUGUI _investigationPromptText;

    [Header("Reticle Settings")]
    [SerializeField] private Image _detectorImage;
    [SerializeField] private Vector3 _targetScale = new Vector3(2f, 2f, 2f);
    [SerializeField] private Color _defaultImageColor= Color.red;
    [SerializeField] private Color _detectedSomethingColor = Color.green;

    private void Awake()
    {
        if(Instance==null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        SetReticleState(false);
        TogglePrompt(false);
    }

    public void SetReticleState(bool canInteract)
    {
        if (_detectorImage == null) return;

        if(canInteract)
        {
            _detectorImage.color = _detectedSomethingColor;
            _detectorImage.transform.localScale = _targetScale;
        }

        else
        {
            _detectorImage.color=_defaultImageColor;
            _detectorImage.transform.localScale = Vector3.one;
        }
    }

    public void TogglePrompt(bool visible)
    {
        if(_interactionPrompt!=null)
        {
            _interactionPrompt.SetActive(visible);
        }
    }

    
    public void ToggleInvestigationMode(bool enabled)
    {
        if (_investigationPrompt!=null)
        {
            _investigationPrompt.SetActive(enabled);
        }
    }
}
