using UnityEngine;
using UnityEngine.AI;

public class NpcHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _agent;

    [Header("Idle")]
    [SerializeField] private float _minIdleDuration = 3f;
    [SerializeField] private float _maxIdleDuration = 3f;

    [Header("Talking")]
    [SerializeField] private float _talkingDuration = 4f;

    [Header("Walking")]
    [SerializeField] private float _walkSearchRadius = 5f;
    [SerializeField] private float _destinationReachedDistance = 0.3f;

    private GenericStateMachine<ECharactertState> _stateMachine;

    public float IdleDuration => UnityEngine.Random.Range(_minIdleDuration, _maxIdleDuration);
    public float TalkingDuration => _talkingDuration;
    public float WalkSearchRadius => _walkSearchRadius;
    public float DestinationReachedDistance => _destinationReachedDistance;

    public NavMeshAgent Agent => _agent;
    public ECharactertState CurrentState => _stateMachine.CurrentStateType;

    private void Awake()
    {
        if (_agent == null)
            _agent = GetComponent<NavMeshAgent>();

        _stateMachine = new GenericStateMachine<ECharactertState>();

        _stateMachine.RegisterState(ECharactertState.Idle, new IdleState(this, _animator));
        _stateMachine.RegisterState(ECharactertState.Walking, new WalkingState(this, _animator));
        _stateMachine.RegisterState(ECharactertState.Talking, new TalkingState(this, _animator));

        _stateMachine.SetState(ECharactertState.Idle);
    }

    private void Update() => _stateMachine.OnUpdate();
    private void FixedUpdate() => _stateMachine.OnFixedUpdate();

    private void OnTriggerEnter(Collider other) => _stateMachine.OnTriggerEnter();
    private void OnTriggerExit(Collider other) => _stateMachine.OnTriggerExit();
    private void OnCollisionEnter(Collision col) => _stateMachine.OnCollisionEnter();
    private void OnCollisionExit(Collision col) => _stateMachine.OnCollisionExit();

    // --- Unica API pubblica esterna ---
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

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_maxIdleDuration < _minIdleDuration)
            _maxIdleDuration = _minIdleDuration;
    }
#endif
}