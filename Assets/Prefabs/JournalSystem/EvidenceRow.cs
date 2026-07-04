using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EvidenceRow : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _textName;

    public event Action<EvidenceRow> OnClicked;

    void OnEnable()
    {
        _button.onClick.RemoveListener(ButtonClicked);
        _button.onClick.AddListener(ButtonClicked);
    }

    void OnDisable()
    {
        _button.onClick.RemoveListener(ButtonClicked);
    }

    private void ButtonClicked()
    {
        OnClicked?.Invoke(this);
    }

    public void SetUp(string name)
    {
        _textName.text = name;
    }
}
