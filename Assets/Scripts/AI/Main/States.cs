using UnityEngine;

public class States
{
    public static readonly IdleState Idle = new IdleState();
    public static readonly AttackStartState AttackStart = new AttackStartState();
    public static readonly AttackState Attack = new AttackState();
    public static readonly AttackEndState AttackEnd = new AttackEndState();
    public static readonly ReloadState Reload = new ReloadState();
    public static readonly IdleReloadState IdleReload = new IdleReloadState();
    public static readonly MoveState Move = new MoveState();
    public static readonly StunState Stun = new StunState();
}
