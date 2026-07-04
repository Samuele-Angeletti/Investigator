using UnityEngine;

public class IdleState : State
{
    private readonly Animator _animator;
    private readonly NpcHandler _owner;

    private float _timer;

    public IdleState(NpcHandler owner, Animator animator)
    {
        _owner = owner;
        _animator = animator;
    }

    public override void OnStart()
    {
        _timer = 0f;
        _owner.StopMovement();
        _animator.SetTrigger("Idle");
    }

    public override void OnEnd() { }

    public override void OnUpdate()
    {
        _timer += Time.deltaTime;

        if (_timer < _owner.IdleDuration)
            return;

        if (_owner.TryFindRandomNavMeshPoint(out Vector3 destination))
        {
            _owner.MoveTo(destination);
            _owner.GoToWalking();
        }
        else
        {
            _timer = 0f;
        }
    }

    public override void OnFixedUpdate() { }
    public override void OnTriggerEnter() { }
    public override void OnTriggerExit() { }
    public override void OnCollisionEnter() { }
    public override void OnCollisionExit() { }
}