using UnityEngine;

public class StateTemplate : State
{
    private readonly Animator animator;
    private readonly NpcHandler _owner;


    public StateTemplate(NpcHandler owner, Animator animator)
    {
        _owner = owner;
        this.animator = animator;
    }

    public override void OnCollisionEnter()
    {
        throw new System.NotImplementedException();
    }

    public override void OnCollisionExit()
    {
        throw new System.NotImplementedException();
    }

    public override void OnEnd()
    {
        throw new System.NotImplementedException();
    }

    public override void OnFixedUpdate()
    {
        throw new System.NotImplementedException();
    }

    public override void OnStart()
    {
        animator.SetTrigger("Template");
    }

    public override void OnTriggerEnter()
    {
        throw new System.NotImplementedException();
    }

    public override void OnTriggerExit()
    {
        throw new System.NotImplementedException();
    }

    public override void OnUpdate()
    {
        throw new System.NotImplementedException();
    }
}
