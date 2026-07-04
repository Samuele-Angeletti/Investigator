using System;
using StarterAssets;
using UnityEngine;

/// <summary>
/// Orchestratore dei dialoghi (mediator). Collega la sorgente del dialogo alla
/// vista e gestisce l'input di chiusura. È esposto come singleton per coerenza
/// con le altre facciate globali del progetto (es. <c>UiManager</c>); i
/// collaboratori restano però dietro interfacce, così la logica resta
/// sostituibile e testabile.
/// </summary>
public class DialogueController : MonoBehaviour, IDialogueService
{
    /// <summary>Istanza globale del servizio dialoghi.</summary>
    public static DialogueController Instance { get; private set; }

    [Header("References")]
    [SerializeField] private DialogueUIPresenter _presenterBehaviour;
    [SerializeField] private StarterAssetsInputs _inputs;

    private IDialoguePresenter _presenter;
    private Action _onComplete;
    private int _openFrame = -1;

    /// <inheritdoc />
    public bool IsOpen { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _presenter = _presenterBehaviour;

        if (_inputs == null)
            _inputs = FindFirstObjectByType<StarterAssetsInputs>();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Update()
    {
        if (!IsOpen)
            return;

        // Ignora l'input nel frame di apertura: lo stesso "interact" che apre il
        // dialogo non deve chiuderlo nello stesso frame.
        if (Time.frameCount <= _openFrame)
            return;

        if (_inputs != null && _inputs.interact)
        {
            _inputs.interact = false;
            Close();
        }
    }

    /// <inheritdoc />
    public void Begin(IDialogueProvider provider, Action onComplete)
    {
        if (IsOpen)
            return;

        if (provider == null || !provider.TryGetDialogue(out DialogueData dialogue) || dialogue == null)
        {
            onComplete?.Invoke();
            return;
        }

        _onComplete = onComplete;
        _openFrame = Time.frameCount;
        IsOpen = true;

        _presenter?.Show(dialogue);

        // Nasconde reticolo e prompt mentre si legge.
        if (UiManager.Instance != null)
        {
            UiManager.Instance.SetReticleState(false);
            UiManager.Instance.TogglePrompt(false);
        }
    }

    private void Close()
    {
        IsOpen = false;
        _presenter?.Hide();

        // La callback viene azzerata prima di invocarla per evitare rientri.
        Action callback = _onComplete;
        _onComplete = null;
        callback?.Invoke();
    }
}
