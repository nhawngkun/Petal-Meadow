using NabaGame.UI;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    [FoldoutGroup("Animator")] public Animator _mainAnimator;

    [FoldoutGroup("bool")] public bool _IsMove;
    [FoldoutGroup("bool")] public bool _IsJump;
    [FoldoutGroup("bool")] public bool _IsDie;
    [FoldoutGroup("bool")] public bool _IsStunned;
    [FoldoutGroup("bool")] public bool _IsPutItem;
    [FoldoutGroup("bool")] public bool _HoldingGun;

    [FoldoutGroup("int")] public int _IdleId;
    [FoldoutGroup("int")] public int _MoveId;
    public AnimationCurve _MoveCurve;
    public SkinManager _skinManager;
    //void Update()
    //{
    //    SetAnimation();
    //}

    private void Start()
    {
        //SetSkin(GameManager.Instance.PlayerProfile.outfitProfile.skinEquip);
    }

    public void SetSkin(CharacterType type) 
    {
        Skin skin = _skinManager.GetSkin(type);
        //GameManager.Instance.PlayerProfile.outfitProfile.skinEquip = type;
        _mainAnimator = skin._animator;
    }

#if UNITY_EDITOR
    [Header("Editor Test")]
    public CharacterType TestSkinType;


    [Button("TEST / Set Skin")]
    public void TestSetSkin()
    {
        SetSkin(TestSkinType);
    }
#endif
    public void SetAnimation()
    {
        ApplyToAnimator(_mainAnimator);
        //_mainAnimator.speed = _IsJump ? 1 :_MoveCurve.Evaluate(0.5f + GameController.Instance.PlayerController.bonusSpeed * GameController.Instance.PlayerController.MulSpeed());
    }

    private void ApplyToAnimator(Animator anim)
    {
        if (anim == null || !anim.gameObject.activeSelf) return;

        float deltaTime = Time.deltaTime;

        anim.SetFloat(AnimatorParameters.BLEND___IDLE__ID, _IdleId, 0f, deltaTime);
        anim.SetFloat(AnimatorParameters.BLEND___MOVE__ID, _MoveId, 0f, deltaTime);

        anim.SetBool(AnimatorParameters.IS_MOVE, _IsMove);
        anim.SetBool(AnimatorParameters.IS_JUMP, _IsJump);
        //anim.SetBool(AnimatorParameters.IS_HIT, _IsStunned);
        //anim.SetBool(AnimatorParameters.IS_DIE, _IsDie);
    }

    


    public void ResetPose()
    {
        _MoveId = 0;
        _IdleId = 0;
        SetAnimation();
    }
}

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
    public static readonly int E_IS_RUN = Animator.StringToHash("Is_Move");
    public static readonly int E_BLEND___MOVE__ID = Animator.StringToHash("Move_ID");


    public static readonly int V_IS_RUN = Animator.StringToHash("IsMove");
    public static readonly int V_IS_RAGGING = Animator.StringToHash("IsRagging");
}