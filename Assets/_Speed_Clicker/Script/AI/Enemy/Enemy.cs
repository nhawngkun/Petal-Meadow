using DG.Tweening;
using NabaGame.Core.Runtime.TickManager;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;

public  class Enemy : MonoBehaviour
{

    public EnemyState currentState;
    [FoldoutGroup("Stats")] public ParticleSystem stunEffect;
    [FoldoutGroup("Stats")][SerializeField] private float maxHealth = 100f;
    [FoldoutGroup("Stats")] public float health = 50f;
    [FoldoutGroup("Stats")] public float Range = 5f;
    [FoldoutGroup("Stats")] public float moveSpeed = 12f;
    [FoldoutGroup("Stats")] public float ChaseSpeed = 12f;
    [FoldoutGroup("Stats")] public float Damage = 50f;
    [FoldoutGroup("Stats")] public float Atkspeed = 50f;
    [FoldoutGroup("Stats")] public int Move_ID;


    [FoldoutGroup("Patrol Settings")] public float waitTime = 2f;

    [FoldoutGroup("Jump Settings")] public float jumpHeight = 2f;      // độ cao parabol
    [FoldoutGroup("Jump Settings")] public float jumpDuration = 0.5f;  // thời gian nhảy

    [FoldoutGroup("bool")] public bool _IsMoving;
    [FoldoutGroup("bool")] public bool _IsAttack;
    [FoldoutGroup("bool")] public bool _IsDie => health <= 0;
    [FoldoutGroup("bool")] public bool _IsStunned;
    [FoldoutGroup("bool")] public bool IsPlayerInAttackRange;
    [FoldoutGroup("bool")] public bool _hasDied = false;
    [FoldoutGroup("bool")] public bool stuned = false;
    [FoldoutGroup("bool")] private bool _isKnockback = false;
    [FoldoutGroup("bool")] public bool _isJumping = false;
    [FoldoutGroup("bool")] public bool canAtk;


    [FoldoutGroup("SetUp")] public Animator animator;
    [FoldoutGroup("SetUp")] public NavMeshAgent _agent;
    [FoldoutGroup("SetUp")] public Rigidbody _rb;
    [FoldoutGroup("SetUp")] public GameObject _target;

    [FoldoutGroup("Knob Back")][SerializeField] private float knockbackForce = 1f;
    [FoldoutGroup("Knob Back")][SerializeField] private float knockbackTime = 0.2f;


    protected Dictionary<Type, IState> _states = new();
    protected IState _currentState;
    public bool IsHitAttack { get; set; }
 

    void OnValidate()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (_agent == null)
        {
            _agent = GetComponent<NavMeshAgent>();
            if (_agent == null)
                _agent = gameObject.AddComponent<NavMeshAgent>();
        }

       
       

    }

    protected virtual void Awake()
    {
        health = maxHealth;
        RegisterStates();
        stunEffect.gameObject?.SetActive(false);

    }


    protected void RegisterStates()
    {


    }




    public void SetSpeed(bool Chasing)
    {
        if (Chasing)
        {
            _agent.speed = ChaseSpeed;
            _agent.acceleration = ChaseSpeed * 3;
        }
        else
        {
            _agent.speed = moveSpeed;
            _agent.acceleration = moveSpeed * 3;
        }


    }

    public void OnTickableUpdat()
    {
        SetAnimation();
        if (_IsDie)
        {
            return;
        }
        if (stuned) return;
        if (_currentState == null) return;
        _currentState?.Execute();

    }


    public void ChangeState<T>() where T : IState
    {
        if (_currentState != null)
            _currentState.OnExit();

        _currentState = _states[typeof(T)];

        if (_currentState != null)
            _currentState.OnEnter();
    }
    public virtual void SetAnimation()
    {
        if (animator != null && animator.gameObject.activeSelf)
        {
            float deltaTime = Time.deltaTime;
            animator.SetBool(AnimatorParameters.E_IS_RUN, _IsMoving && !_IsDie && !_IsStunned);
            //animator.SetBool(AnimatorParameters.E_IS_ATTACK, _IsAttack && !_IsDie && !_IsStunned);
            //animator.SetBool(AnimatorParameters.E_IS_DEAD, _IsDie);
        }

    }
    public void ReturnPool()
    {
        //GameManager.Instance.objectPool.EnemyReturnPool(enemyID, gameObject);
    }

    #region ChasingState
    private List<Vector3> _patrolPoints = new List<Vector3>();
    [SerializeField] private int _waypointCount = 10;
    float patrolRadius;
    private void GeneratePatrolPoints()
    {
        
    }
    public void GoToRandomPoint()
    {
        if (_agent == null || !_agent.isActiveAndEnabled || !_agent.isOnNavMesh)
            return;

        // Nếu danh sách trống thì generate mới
        if (_patrolPoints.Count == 0)
        {
            GeneratePatrolPoints();
            if (_patrolPoints.Count == 0) return; // vẫn không có thì bỏ qua
        }

        int index = UnityEngine.Random.Range(0, _patrolPoints.Count);
        _agent.SetDestination(_patrolPoints[index]);
    }

    public void Jump()
    {
        if (!_isJumping && _agent.isOnOffMeshLink)
        {
            StartCoroutine(HandleJump());
        }
    }

    public IEnumerator HandleJump()
    {
        _isJumping = true;

        //// Tắt auto-traverse để tránh bị warp
        //_agent.autoTraverseOffMeshLink = false;

        // Lấy dữ liệu link
        OffMeshLinkData data = _agent.currentOffMeshLinkData;

        // Sử dụng vị trí hiện tại thực tế của agent, không phải center point
        Vector3 start = _agent.transform.position;
        Vector3 end = data.endPos + Vector3.up * _agent.baseOffset;

        // Tạm dừng NavMeshAgent
        _agent.isStopped = true;
        _agent.updatePosition = false;
        _agent.updateRotation = false;

        // Xoay về hướng đích trước khi nhảy
        Vector3 direction = (end - start).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            _agent.transform.rotation = Quaternion.LookRotation(direction);
        }

        float time = 0f;
        while (time < jumpDuration)
        {
            float t = time / jumpDuration;

            // Sử dụng ease-in-out để chuyển động mượt mà hơn
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            Vector3 pos = Vector3.Lerp(start, end, smoothT);
            pos.y += Mathf.Sin(t * Mathf.PI) * jumpHeight;

            _agent.transform.position = pos;

            time += Time.deltaTime;
            yield return null;
        }

        // Đảm bảo kết thúc đúng vị trí
        _agent.transform.position = end;

        // Hoàn thành OffMeshLink
        _agent.CompleteOffMeshLink();

        // Kích hoạt lại NavMeshAgent
        _agent.isStopped = false;
        _agent.updatePosition = true;
        _agent.updateRotation = true;

        _isJumping = false;
    }
    #endregion

    #region AttackState
    public virtual void StopAttack()
    {
        StartCoroutine(ResetAtk());
    }

    public IEnumerator ResetAtk()
    {
        yield return new WaitForSeconds(1 / Atkspeed);
        canAtk = true;
    }
    #endregion


    #region Visualize
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = new Color(0f, 0f, 1f, 0.25f); // xanh trong suốt
    //    Gizmos.DrawWireSphere(transform.position, patrolRadius);
    //}
    #endregion



}

public enum EnemyState
{
    Patrol,
    Chase,
    Attack,
    Die,
    Escape
}




