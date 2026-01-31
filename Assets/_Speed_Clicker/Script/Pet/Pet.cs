using NabaGame.Core.Runtime.TickManager;
using UnityEngine;

public class Pet : TickableBehaviour
{
    public Transform owner;
    public PetType type;
    public int id;
    enum PetState
    {
        Wander,
        PrepareChase,
        Chasing,
        Brake
    }

    PetState _state = PetState.Wander;

    [Header("Wander Area")]
    public float wanderRadius = 3f;
    public float moveSpeed = 3f;
    public float reachDistance = 0.3f;

    [Header("Chase Logic")]
    public float chaseDistance = 6f;
    public float catchDistance = 1.2f;
    public float chaseSpeedMultiplier = 5f;
    public float prepareTime = 0.25f;
    public float brakeTime = 0.2f;

    float _stateTimer;

    [Header("Fun Motion")]
    public float swayAmplitude = 0.25f;
    public float swayFrequency = 4f;

    Vector3 _currentTarget;
    float _timeOffset;

    [Header("Wobble / Squash")]
    public float bounceAmplitude = 0.15f;
    public float bounceFrequency = 3.5f;
    public float squashAmount = 0.12f;

    [Header("Soft Hover Y")]
    public float hoverAmplitude = 0.25f;
    public float hoverFrequency = 2.2f;
    public float hoverSpeedInfluence = 0.5f;
    public float hoverSmooth = 6f;

    [Header("Owner Safety")]
    public float minDistanceToOwner = 1.2f;

    [Header("Avoid Owner")]
    public float avoidRadius = 1.5f;
    public float avoidStrength = 2.5f;

    [Header("Ground Clamp")]
    public float minHoverHeight = 0.05f;

    Vector3 _baseScale;
    Quaternion _baseRot;

    private void Awake()
    {
        _timeOffset = Random.value * 10f;
        _baseScale = transform.localScale;
        _baseRot = transform.rotation;
        PickNewTarget();
    }

    public override void OnTickableUpdated(float dt)
    {
        if (!owner) return;

        UpdateState(dt);
        Move(dt);
        ApplyWobble(dt);
        ApplySoftHoverY(dt);
    }

    void UpdateState(float dt)
    {
        float distToOwner = Vector3.Distance(transform.position, owner.position);

        switch (_state)
        {
            case PetState.Wander:
                if (distToOwner > chaseDistance)
                {
                    _state = PetState.PrepareChase;
                    _stateTimer = 0f;
                }
                break;

            case PetState.PrepareChase:
                _stateTimer += dt;
                ApplyAnticipation(dt);

                if (_stateTimer >= prepareTime)
                {
                    _state = PetState.Chasing;
                }
                break;

            case PetState.Chasing:
                if (distToOwner <= catchDistance)
                {
                    _state = PetState.Brake;
                    _stateTimer = 0f;
                }
                break;

            case PetState.Brake:
                _stateTimer += dt;
                ApplyBrakePose(dt);

                if (_stateTimer >= brakeTime)
                {
                    _state = PetState.Wander;
                    PickNewTarget();
                }
                break;
        }
    }

    void Move(float dt)
    {
        if (_state == PetState.PrepareChase || _state == PetState.Brake)
            return;

        Vector3 pos = transform.position;

        Vector3 target =
            _state == PetState.Chasing
            ? owner.position
            : _currentTarget;

        Vector3 desiredDir = target - pos;
        desiredDir.y = 0f;

        if (_state == PetState.Wander &&
            desiredDir.sqrMagnitude < reachDistance * reachDistance)
        {
            PickNewTarget();
            return;
        }

        desiredDir.Normalize();

        Vector3 toOwner = pos - owner.position;
        toOwner.y = 0f;

        float dist = toOwner.magnitude;
        Vector3 avoidDir = Vector3.zero;

        if (_state == PetState.Wander && dist < avoidRadius)
        {
            float t = 1f - (dist / avoidRadius);
            Vector3 side = Vector3.Cross(Vector3.up, toOwner.normalized);
            avoidDir = side * t * avoidStrength;
        }

        Vector3 finalDir = (desiredDir + avoidDir).normalized;

        Vector3 swaySide = Vector3.Cross(Vector3.up, finalDir);
        float sway = Mathf.Sin(Time.time * swayFrequency + _timeOffset) * swayAmplitude;

        float speed = moveSpeed;
        if (_state == PetState.Chasing)
            speed *= chaseSpeedMultiplier;

        Vector3 velocity = finalDir * speed + swaySide * sway;
        transform.position += velocity * dt;

        if (velocity.sqrMagnitude > 0.001f)
        {
            Quaternion rot = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, dt * 10f);
        }
    }

    void ApplyAnticipation(float dt)
    {
        float t = Mathf.Clamp01(_stateTimer / prepareTime);
        float angle = Mathf.Lerp(0f, -25f, t);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.Euler(angle, transform.eulerAngles.y, 0f),
            dt * 12f
        );
    }

    void ApplyBrakePose(float dt)
    {
        float t = Mathf.Clamp01(_stateTimer / brakeTime);
        float angle = Mathf.Lerp(-15f, 20f, t);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.Euler(angle, transform.eulerAngles.y, 0f),
            dt * 14f
        );
    }

    void ApplyWobble(float dt)
    {
        if (_state != PetState.Wander) return;

        float speedFactor = Mathf.Clamp01(
            (_currentTarget - transform.position).magnitude
        );

        float t = (Time.time + _timeOffset) * bounceFrequency;
        float bounce = Mathf.Sin(t) * bounceAmplitude * speedFactor;

        float squash = 1f - bounce * squashAmount;
        float stretch = 1f + bounce * squashAmount;

        Vector3 targetScale = new Vector3(
            _baseScale.x * stretch,
            _baseScale.y * squash,
            _baseScale.z * stretch
        );

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            dt * 10f
        );
    }

    void ApplySoftHoverY(float dt)
    {
        float speedFactor = Mathf.Clamp01(
            (_currentTarget - transform.position).magnitude
        );

        float amp = hoverAmplitude *
                    Mathf.Lerp(0.4f, 1f, speedFactor * hoverSpeedInfluence);

        float t = Time.time * hoverFrequency + _timeOffset;

        float offsetY =
            Mathf.Sin(t) * amp +
            Mathf.PerlinNoise(_timeOffset, Time.time * 0.5f) * amp * 0.5f;

        float groundY = owner.position.y + minHoverHeight;
        float targetY = Mathf.Max(groundY + offsetY, groundY);

        Vector3 pos = transform.position;
        pos.y = Mathf.Lerp(pos.y, targetY, dt * hoverSmooth);
        transform.position = pos;
    }

    void PickNewTarget()
    {
        if(owner == null) return;
        for (int i = 0; i < 8; i++)
        {
            Vector2 random = Random.insideUnitCircle * wanderRadius;

            if (random.magnitude < minDistanceToOwner)
                continue;

            _currentTarget = owner.position + new Vector3(random.x, 0f, random.y);
            return;
        }

        Vector3 dir = (transform.position - owner.position).normalized;
        _currentTarget = owner.position + dir * minDistanceToOwner;
    }
}
