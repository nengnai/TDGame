using UnityEngine;

public class UnitAnim
{
    public Animator animator;
    public static readonly int Anim_Idle = Animator.StringToHash("Idle");
    public static readonly int Anim_Moving = Animator.StringToHash("Moving");
    public static readonly int Anim_AttackStart = Animator.StringToHash("AttackStart");
    public static readonly int Anim_Attack = Animator.StringToHash("Attack");
    public static readonly int Anim_AttackEnd = Animator.StringToHash("AttackEnd");
    public static readonly int Anim_Reloading = Animator.StringToHash("Reloading");
    public static readonly int Anim_IdleReloading = Animator.StringToHash("Idle_Reload");
    public static readonly int Anim_Stun = Animator.StringToHash("Stun");




    public UnitAnim(Animator animator)
    {
        this.animator = animator;
    }

    public float GetAnimLength(string clipName)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        for(int i = 0; i < clips.Length; i++)
        {
            if(clips[i].name == clipName)
            {
                return clips[i].length;
            }
        }
        return 1f;
    }
}
