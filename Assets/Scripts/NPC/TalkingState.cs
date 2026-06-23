using UnityEngine;

public class TalkingState : State
{
    private readonly Animator _animator;
    private readonly NpcHandler _owner;

    private float _timer;

    public TalkingState(NpcHandler owner, Animator animator)
    {
        _owner = owner;
        _animator = animator;
    }

    public override void OnStart()
    {
        _timer = 0f;
        _owner.StopMovement();
        _animator.SetTrigger("Talking");
    }

    public override void OnEnd() { }

    public override void OnUpdate()
    {
        _timer += Time.deltaTime;

        if (_timer >= _owner.TalkingDuration)
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