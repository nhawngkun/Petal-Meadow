using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMove : MonoBehaviour
{
    public GroundCheckConfig _GroundCheckConfig;
    private const string JOYSTICK_MOVE = "Move";
    public CharacterController _controller;
    public PlayerController _player;
    public StarterAssetsInput _input;

    private Collider[] _groundHits = new Collider[2];
    public bool IsGrounded = true;
    [SerializeField] private float _groundLockTime = 0.15f;
    private bool _groundLocked;
    private float _groundLockTimer;
    private bool _wasGrounded;

    [Header("Config")]
    [SerializeField] private float _rotationSmoothTime = 0.12f;
    [SerializeField] private float _speedChangeRate = 10f;

    //public CharacterController _controller;
    public Transform _camTransform;
    private float _speed;
    private float _targetRotation;
    private float _rotationVelocity;
    private float _verticalVelocity;

    public float baseMoveSpeed = 5f;
    public bool isPushed;
    public float _tilePushSpeed;
    private Vector3 _tilePushDir = Vector3.forward;
    public bool hasInput => _input.move != Vector2.zero;
    private SoundManager _soundManager;



    public void Update()
    {
   
        Move();
        JumpAndGravity();
    }


    public void FixedUpdate()
    {
        GroundCheck();
    }

    private void GroundCheck()
    {

        // ===== Ground lock countdown =====
        if (_groundLocked)
        {
            _groundLockTimer -= Time.fixedDeltaTime;
            if (_groundLockTimer <= 0f)
                _groundLocked = false;
        }

        _wasGrounded = IsGrounded;

        if (_groundLocked)
        {
            IsGrounded = false;
            return;
        }

        Vector3 spherePosition = new Vector3(
            transform.position.x,
            transform.position.y - _GroundCheckConfig.GroundedOffset,
            transform.position.z
        );

        int hitCount = Physics.OverlapSphereNonAlloc(
            spherePosition,
            _GroundCheckConfig.GroundedRadius,
            _groundHits,
            _GroundCheckConfig.GroundLayers,
            QueryTriggerInteraction.Ignore
        );

        IsGrounded = hitCount > 0;

        // 🔥 LANDING FRAME (CHUẨN)
        if (!_wasGrounded && IsGrounded)
        {
            _player._playerAction._IsJump = false;
        }
        // 🔥 Vừa tiếp đất

    }
    public void Move()
    {
        if (IsInputOrControllerNull()) return;

        float targetSpeed = _input.move == Vector2.zero ? 0.0f : baseMoveSpeed + _player.bonusSpeed * _player.MulSpeed();
        float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;
        _speed = Mathf.Lerp(_speed, targetSpeed * inputMagnitude, Time.deltaTime * _player._PlayerConfig.SpeedChangeRate);
        _speed = Mathf.Round(_speed * 1000f) / 1000f;
        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
        if (hasInput) 
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                _camTransform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                _player._PlayerConfig.RotationSmoothTime);
            if (!float.IsNaN(rotation) && !float.IsInfinity(rotation))
            {
                transform.rotation = Quaternion.Euler(0f, rotation, 0f);
            }

        }
        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        Vector3 horizontalVelocity = Vector3.zero;

        if (isPushed)
        {
            float netSpeed = Mathf.Max(targetSpeed - _tilePushSpeed, 0.5f);

            if (_input.move.y > 0)
            {
                // Có input → tiến nhưng bị giảm tốc
                horizontalVelocity = targetDirection.normalized * netSpeed;
            }
            else
            {
                // Không input → bị đẩy theo hướng tile
                horizontalVelocity = _tilePushDir.normalized * _tilePushSpeed;
            }
        }
        else
        {
            horizontalVelocity = targetDirection.normalized * _speed;
        }

        // MOVE DUY NHẤT 1 LẦN
        Vector3 move =
            horizontalVelocity * Time.deltaTime +
            Vector3.up * _verticalVelocity * Time.deltaTime;

        _controller.Move(move);


        CheckAnimation();
    }

    public void JumpAndGravity()
    {
        // Nếu đang đứng trên mặt đất, reset vận tốc rơi
        if (IsGrounded)
        {
            if (_verticalVelocity < 0.00f)
            {
                _verticalVelocity = -2f;
            }
            if (_input.jump)
            {

                //SoundManager.Instance.PlaySFX(SoundType.Jump);
                _player._playerAction._IsJump = true;
                _verticalVelocity = Mathf.Sqrt(_player._PlayerConfig.JumpHeight * -2f * _player._PlayerConfig.Gravity);
                //fakeShadow.TurnOffShadow();
                //StartCoroutine(JumpReaddy());

                _input.jump = false;
                //AudioManager.Instance.PlaySound(SoundType.jump);

            }

        }
        else
        {
            _input.jump = false;
        }

        // Áp dụng trọng lực để rơi xuống
        _verticalVelocity += _player._PlayerConfig.Gravity * Time.deltaTime;
    }
    private bool IsInputOrControllerNull()
    {
        if (_input == null || _controller == null)
        {
            return true;
        }

        return false;
    }

    public void GetInput()
    {
        //_input.move.x = UltimateJoystick.GetHorizontalAxis(JOYSTICK_MOVE);
        //_input.move.y = UltimateJoystick.GetVerticalAxis(JOYSTICK_MOVE);
    }



    public void MoveJoystick()
    {
        Move();
    }
    private bool _wasMoving;
    private void CheckAnimation()
    {
        _player._playerAction._IsMove = _input.move != Vector2.zero;
        bool isMoving = _input.move.sqrMagnitude > 0.0001f;

        // 1️⃣ Animation state
        _player._playerAction._IsMove = isMoving;

        // 2️⃣ Không đổi trạng thái → thoát sớm
        if (isMoving == _wasMoving)
            return;

        // 3️⃣ Audio (chỉ khi state thay đổi)
        if (_soundManager != null)
        {
            if (isMoving)
                _soundManager.PlaySoundMove();
            else
                _soundManager.StopSoundMove();
        }

        _wasMoving = isMoving;
    }
   
}
