using UnityEngine;

/// <summary>
/// Stato "in conversazione". La durata NON è più a tempo: lo stato resta attivo
/// finché il dialogo è aperto. L'uscita è pilotata dall'esterno tramite
/// <see cref="NpcHandler.GoToIdle"/>, invocata alla chiusura del dialogo.
/// </summary>
public class TalkingState : State
{
    private readonly Animator _animator;
    private readonly NpcHandler _owner;

    public TalkingState(NpcHandler owner, Animator animator)
    {
        _owner = owner;
        _animator = animator;
    }

    public override void OnStart()
    {
        _owner.StopMovement();
        _animator.SetTrigger("Talking");
    }

    public override void OnEnd() { }

    // Nessuna uscita a tempo: si esce alla chiusura del dialogo (vedi NpcHandler).
    public override void OnUpdate() { }

    public override void OnFixedUpdate() { }
    public override void OnTriggerEnter() { }
    public override void OnTriggerExit() { }
    public override void OnCollisionEnter() { }
    public override void OnCollisionExit() { }
}
