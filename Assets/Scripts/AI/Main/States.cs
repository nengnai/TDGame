using Unity.Transforms;
using UnityEngine;

public class States
{
    public static readonly IdleState Idle = new IdleState();
    public static readonly AttackStartState attackStart = new AttackStartState();
    public static readonly AttackState Attack = new AttackState();
    public static readonly AttackEndState attackEnd = new AttackEndState();
    public static readonly ReloadState Reload = new ReloadState();
    public static readonly IdleReloadState IdleReload = new IdleReloadState();
    public static readonly MoveState Move = new MoveState();
    public static readonly StunState Stun = new StunState();
    public static readonly DeadState Dead = new DeadState();
}
