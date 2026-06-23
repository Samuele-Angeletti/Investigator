using UnityEngine;

public class WalkingState : State
{
    private readonly Animator _animator;
    private readonly NpcHandler _owner;

    public WalkingState(NpcHandler owner, Animator animator)
    {
        _owner = owner;
        _animator = animator;
    }

    public override void OnStart()
    {
        _animator.SetTrigger("Walking");
    }

    public override void OnEnd() { }

    public override void OnUpdate()
    {
        if (_owner.HasReachedDestination())
        {
            _owner.GoToIdle();
        }
    }

    public override void OnFixedUpdate() { }
    public override void OnTriggerEnter() { }
    public override void OnTriggerExit() { }
    public override void OnCollisionEnter() { }
    public override void OnCollisionExit() { }
}