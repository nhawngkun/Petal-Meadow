using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections.Generic;
using DG.Tweening;

public class ChimController : MonoBehaviour
{
    [Header("Settings")]
    public NavMeshAgent agent;
    public Transform player;
    public float detectionRange = 25f; // Tầm nhìn xa để bắt đầu cảnh giác
    public float fleeDistance = 15f;    // Khoảng cách an toàn để ngừng chạy
    public float patrolRadius = 10f;

    protected Dictionary<Type, IState> _states = new();
    protected IState _currentState;

    // Cache giá trị bình phương để tối ưu hiệu năng (tránh tính căn bậc 2 mỗi frame)
    private float _sqrDetectionRange;

    protected virtual void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        
        // Tự động tìm player nếu quên gán trong Inspector
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        _sqrDetectionRange = detectionRange * detectionRange;
        RegisterStates();
    }

    protected void RegisterStates()
    {
        _states.Add(typeof(bird_PatrolState), new bird_PatrolState(this));
        _states.Add(typeof(bird_ChaseState), new bird_ChaseState(this));
        _states.Add(typeof(bird_FleeState), new bird_FleeState(this));
    }

    void Start()
    {
        // Bắt đầu bằng trạng thái đi tuần
        ChangeState<bird_PatrolState>();
    }

    public void Update()
    {
        if (_currentState == null || player == null) return;

        // TỐI ƯU: Sử dụng sqrMagnitude thay vì Distance
        float sqrDist = (transform.position - player.position).sqrMagnitude;

        // Nếu Player đi vào vùng nhận diện và Chim đang không chạy trốn
        if (sqrDist < _sqrDetectionRange && !(_currentState is bird_FleeState))
        {
            ChangeState<bird_FleeState>();
        }

        _currentState.Execute();
    }

    public void ChangeState<T>() where T : IState
    {
        Type type = typeof(T);
        
        // Nếu đã ở state này rồi thì không đổi nữa
        if (_currentState != null && _currentState.GetType() == type) return;

        _currentState?.OnExit();
        _currentState = _states[type];
        _currentState?.OnEnter();
    }

    public Vector3 GetPatrolPoints()
    {
        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * patrolRadius;
        Vector3 randomPoint = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return transform.position;
    }
}
