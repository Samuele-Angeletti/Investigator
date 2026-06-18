
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; set; }

    [Header("Ui Ref")]
    [SerializeField] private GameObject _interactionPrompt;
    [SerializeField] private TextMeshProUGUI promptText;

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

    public void TogglePrompt(bool visible)
    {
        if(_interactionPrompt!=null)
        {
            _interactionPrompt.SetActive(visible);
        }
    }
}
