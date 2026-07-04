using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SuspectRow : MonoBehaviour
{
    [SerializeField] private Toggle _toggle;
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _textName;

    public event Action<SuspectRow> OnClicked;

    void OnEnable()
    {
        _toggle.onValueChanged.RemoveListener(ToggleClicked);
        _toggle.onValueChanged.AddListener(ToggleClicked);
    }

    void OnDisable()
    {
        _toggle.onValueChanged.RemoveListener(ToggleClicked);
    }

    private void ToggleClicked(bool value)
    {
        OnClicked?.Invoke(this);
    }

    public void SetUp(Sprite sprite, string name)
    {
        _image.sprite = sprite;
        _textName.text = name; 
    }
}
