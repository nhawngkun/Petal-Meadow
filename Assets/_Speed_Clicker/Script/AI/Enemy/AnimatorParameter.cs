using UnityEngine;

public static class AnimatorParameters
{
    //Player
    public static readonly int IS_MOVE = Animator.StringToHash("IsMove");
    public static readonly int IS_JUMP = Animator.StringToHash("IsJump");
    public static readonly int IS_ATTACK = Animator.StringToHash("IsAttack");
    public static readonly int IDLE = Animator.StringToHash("Idle");
    public static readonly int BLEND___IDLE__ID = Animator.StringToHash("Blend_Idle_ID");
    public static readonly int BLEND___MOVE__ID = Animator.StringToHash("Blend_Move_ID");
    public static readonly int IS_HIT = Animator.StringToHash("IsHit");
    public static readonly int IS_DIE = Animator.StringToHash("IsDie");
    public static readonly int IS_PUT_ITEM = Animator.StringToHash("PutItem");

    ////Enemy
    public static readonly int E_IS_RUN = Animator.StringToHash("Moving");
    public static readonly int E_BLEND___MOVE__ID = Animator.StringToHash("Move_ID");
    public static readonly int E_IS_ATTACK = Animator.StringToHash("Atk");

    public static readonly int V_IS_RUN = Animator.StringToHash("IsMove");
    public static readonly int V_IS_RAGGING = Animator.StringToHash("IsRagging");
}