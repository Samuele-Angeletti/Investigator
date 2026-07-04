using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Cervello dell'NPC: possiede la state machine e l'API pubblica di movimento.
/// È anche il punto di interazione del giocatore (<see cref="IInteractable"/>):
/// su interazione avvia un dialogo delegandone selezione e visualizzazione,
/// senza conoscerne il contenuto né la categoria.
/// </summary>
public class NpcHandler : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _agent;

    [Header("Idle")]
    [SerializeField] private float _minIdleDuration = 3f;
    [SerializeField] private float _maxIdleDuration = 3f;

    [Header("Walking")]
    [SerializeField] private float _walkSearchRadius = 5f;
    [SerializeField] private float _destinationReachedDistance = 0.3f;

    private GenericStateMachine<ECharactertState> _stateMachine;
    private GenericStateMachine<ECharacterInfoState> _infoStateMachine;
    private IDialogueProvider _dialogueProvider;

    public float IdleDuration => UnityEngine.Random.Range(_minIdleDuration, _maxIdleDuration);
    public float WalkSearchRadius => _walkSearchRadius;
    public float DestinationReachedDistance => _destinationReachedDistance;

    public NavMeshAgent Agent => _agent;
    public ECharactertState CurrentState => _stateMachine.CurrentStateType;

    private void Awake()
    {
        if (_agent == null)
            _agent = GetComponent<NavMeshAgent>();

        // La sorgente del dialogo è opzionale: un NPC senza dialoghi non è interrogabile.
        _dialogueProvider = GetComponent<IDialogueProvider>();

        _stateMachine = new GenericStateMachine<ECharactertState>();
        _infoStateMachine = new GenericStateMachine<ECharacterInfoState>();

        _stateMachine.RegisterState(ECharactertState.Idle, new IdleState(this, _animator));
        _stateMachine.RegisterState(ECharactertState.Walking, new WalkingState(this, _animator));
        _stateMachine.RegisterState(ECharactertState.Talking, new TalkingState(this, _animator));

        _infoStateMachine.RegisterState(ECharacterInfoState.Unaware, new UnawareState(this));
        _infoStateMachine.RegisterState(ECharacterInfoState.Informed, new InformedState(this));

        _stateMachine.SetState(ECharactertState.Idle);
        SetInfoState(ECharacterInfoState.Unaware);
    }

    private void Update() => _stateMachine.OnUpdate();
    private void FixedUpdate() => _stateMachine.OnFixedUpdate();

    private void OnTriggerEnter(Collider other) => _stateMachine.OnTriggerEnter();
    private void OnTriggerExit(Collider other) => _stateMachine.OnTriggerExit();
    private void OnCollisionEnter(Collision col) => _stateMachine.OnCollisionEnter();
    private void OnCollisionExit(Collision col) => _stateMachine.OnCollisionExit();

    // --- Interazione ---

    /// <summary>
    /// Chiamato dal sistema di interazione del giocatore. Avvia un dialogo se
    /// l'NPC non sta già parlando e se una sorgente di dialoghi è disponibile.
    /// L'NPC torna in Idle automaticamente alla chiusura del dialogo.
    /// </summary>
    public void Interact()
    {
        if (CurrentState == ECharactertState.Talking)
            return;

        if (DialogueController.Instance == null || _dialogueProvider == null)
            return;

        StartTalking();
        DialogueController.Instance.Begin(_dialogueProvider, GoToIdle);
    }

    // --- Transizioni di stato ---

    public void StartTalking()
    {
        StopMovement();
        _stateMachine.SetState(ECharactertState.Talking);
    }

    public void GoToIdle()
    {
        StopMovement();
        _stateMachine.SetState(ECharactertState.Idle);
    }

    public void GoToWalking()
    {
        _stateMachine.SetState(ECharactertState.Walking);
    }

    // --- Movimento ---

    public bool TryFindRandomNavMeshPoint(out Vector3 result)
    {
        Vector3 randomDirection = Random.insideUnitSphere * _walkSearchRadius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, _walkSearchRadius, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = transform.position;
        return false;
    }

    public void StopMovement()
    {
        if (_agent == null || !_agent.enabled)
            return;

        if (_agent.isOnNavMesh)
        {
            _agent.ResetPath();
            _agent.isStopped = true;
        }
    }

    public void MoveTo(Vector3 destination)
    {
        if (_agent == null || !_agent.enabled || !_agent.isOnNavMesh)
            return;

        _agent.isStopped = false;
        _agent.SetDestination(destination);
    }

    public bool HasReachedDestination()
    {
        if (_agent == null || !_agent.enabled || !_agent.isOnNavMesh)
            return true;

        if (_agent.pathPending)
            return false;

        return _agent.remainingDistance <= _destinationReachedDistance;
    }

    #region SET INFO STATE METHODS

    public void SetInfoState(ECharacterInfoState newState)
    {
        _infoStateMachine.SetState(newState);
    }
    #endregion
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_maxIdleDuration < _minIdleDuration)
            _maxIdleDuration = _minIdleDuration;
    }
#endif
}
