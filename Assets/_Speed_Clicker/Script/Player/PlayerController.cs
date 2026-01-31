using Sirenix.OdinInspector;
using System;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
public class PlayerController : MonoBehaviour
{

    public PlayerConfig _PlayerConfig;


    public PlayerMove _playerMove;
    public PlayerAction _playerAction;
    public Transform LookAt;
    public float bonusSpeed=0;


    [FoldoutGroup("Jump Settings")]
    public float jumpHeight = 2f;
    public float jumpDuration = 0.5f;
    public AnimationCurve jumpCurve =
        AnimationCurve.EaseInOut(0, 0, 1, 1);


  
    private void Update()
    {
        _playerAction.SetAnimation();
#if !UNITY_EDITOR
        _playerMove.GetInput();
#endif
 
        //_playerMove.Move();
        AutoClick();

    }
    public bool isAutoClick = false;


    private float _autoClickTimer;
    public void AutoClick()
    {
        if (!isAutoClick)
        {
            _autoClickTimer = 0f;
            return;
        }

        _autoClickTimer += Time.deltaTime;

        if (_autoClickTimer >= 1f)
        {
            _autoClickTimer -= 1f; // giữ nhịp ổn định
            AddBonus(1);
        }
    }
    public void AddBonus(float bonus)
    {
        bonusSpeed += bonus;
        UIManager.Instance.gamePlayPanel.UpdateText();
    }
 

   

    public float MulSpeed()
    {
        float bonus = bonusSpeed;

        if (bonus <= 100f) return 0.1f;
        if (bonus <= 5000f) return 0.05f;
        return 0.01f;
    }


}
[Serializable]
public class PlayerConfig
{
    public float MoveSpeed = 8.0f;
    public float RotationSmoothTime = 0.12f;
    public float JumpHeight = 3f;
    public float Gravity = -15.0f;
    public float SpeedChangeRate = 10.0f;
}

[Serializable]
public class GroundCheckConfig
{
    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.28f;

    public LayerMask GroundLayers;
    public Vector3 GroundPosCheck;
}