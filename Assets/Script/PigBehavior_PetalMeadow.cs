using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public enum PigColor
{
    Blue,
    Pink,
    Purple
}

public class PigBehavior_PetalMeadow : MonoBehaviour
{
    [Header("🎨 MÀU CỦA LỢN")]
    public PigColor pigColor = PigColor.Blue;

    // ✅ PROPERTIES CHO OUTLINE MANAGER
    public bool IsTamed { get; private set; }
    public bool IsInPen { get; private set; }
    public bool IsCorrectPen { get; private set; }

    [Header("UI Loading (Con của Lợn)")]
    public GameObject loadingCanvas;
    public Image loadingFillImage;
    public float timeToTame = 2.0f;

    [Header("✅❌ UI Đúng/Sai")]
    public GameObject correctIcon;
    public GameObject wrongIcon;

    [Header("😊 Biểu Cảm Thu Phục")]
    [Tooltip("Danh sách biểu cảm (heart, star, smiley...) sẽ random hiện 1 cái")]
    public List<GameObject> tamingEmotions = new List<GameObject>();

    [Header("🎬 Hiệu ứng Bay Lên (DOTween)")]
    public float flyUpDuration = 1.5f;
    public float flyUpHeight = 3f;
    public Ease flyUpEase = Ease.OutQuad;
    public float scalePunchAmount = 0.2f;
    public bool useRotation = true;
    public float rotationAmount = 15f;

    [Header("❌ Dấu X Không Bay (Chỉ Hiện)")]
    public float wrongIconDisplayTime = 2f;

    [Header("Di Chuyển")]
    public float moveSpeed = 5f;
    public float stopDistance = 1.5f;
    private Transform followTarget;

    [Header("🚧 Collision Detection")]
    public LayerMask obstacleLayer;
    public float collisionCheckDistance = 0.5f;
    public float collisionRadius = 0.4f;

    [Header("🎭 Animation")]
    public Animator animator;
    public string runBoolName = "Run";

    [Header("Mất Thuần Hóa")]
    public float timeOutsideToLose = 2.0f;

    [Header("Tránh Pig Khác")]
    public float pigRadius = 0.8f;

    [Header("Lang Thang (Chưa Thuần Phục)")]
    public float wanderSpeed = 2f;
    public float minWanderTime = 2f;
    public float maxWanderTime = 5f;
    public float idleTimeMin = 1f;
    public float idleTimeMax = 3f;

    [Header("🏠 Trạng Thái Trong Chuồng")]
    private int assignedPenIndex = -1;
    private bool hasReachedPenCenter = false;
    private Vector3 penCenterTarget;
    private bool justRescued = false;
    private float penEntryTimer = 0f;
    private bool hasRegisteredInPen = false;
    private bool wasInWrongPen = false;

    private Vector3 currentWanderTarget;
    private float wanderTimer = 0f;
    private float nextWanderTime = 0f;
    private bool isIdling = false;
    private float idleTimer = 0f;
    private float nextIdleTime = 0f;

    private float currentTameTimer = 0f;
    private float outsideTimer = 0f;
    private CarrotPlayer_PetalMeadow player;
    private Camera mainCamera;

    private static List<PigBehavior_PetalMeadow> allPigs = new List<PigBehavior_PetalMeadow>();

    private bool isMoving = false;

    void Start()
    {
        player = CarrotPlayer_PetalMeadow.Instance;
        mainCamera = Camera.main;

        if (!allPigs.Contains(this))
            allPigs.Add(this);

        if (player != null && player.pigFollowTarget != null)
        {
            followTarget = player.pigFollowTarget;
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }
        }

        if (loadingCanvas != null) loadingCanvas.SetActive(false);
        if (correctIcon != null) correctIcon.SetActive(false);
        if (wrongIcon != null) wrongIcon.SetActive(false);

        foreach (var emotion in tamingEmotions)
        {
            if (emotion != null) emotion.SetActive(false);
        }

        // ✅ Khởi tạo properties
        IsTamed = false;
        IsInPen = false;
        IsCorrectPen = false;

        SetNewWanderTarget();
    }

    void OnDestroy()
    {
        allPigs.Remove(this);
    }

    void Update()
    {
        if (player == null) return;

        // Billboard effects
        if (loadingCanvas != null && loadingCanvas.activeSelf && mainCamera != null)
        {
            loadingCanvas.transform.LookAt(loadingCanvas.transform.position + mainCamera.transform.forward);
        }

        if (correctIcon != null && correctIcon.activeSelf && mainCamera != null)
        {
            correctIcon.transform.LookAt(correctIcon.transform.position + mainCamera.transform.forward);
        }

        if (wrongIcon != null && wrongIcon.activeSelf && mainCamera != null)
        {
            wrongIcon.transform.LookAt(wrongIcon.transform.position + mainCamera.transform.forward);
        }

        foreach (var emotion in tamingEmotions)
        {
            if (emotion != null && emotion.activeSelf && mainCamera != null)
            {
                emotion.transform.LookAt(emotion.transform.position + mainCamera.transform.forward);
            }
        }

        CheckIfLeftWrongPen();

        if (IsInPen && !IsCorrectPen)
        {
            CheckTamingZone();
            WanderInPen();
            return;
        }

        if (IsTamed)
        {
            CheckIfStillInRange();

            if (!IsInPen)
            {
                CheckIfEnteredPen();
            }

            if (IsInPen)
            {
                WanderInPen();
            }
            else
            {
                MoveFollowPlayer();
            }
        }
        else
        {
            CheckTamingZone();
            WanderAround();
        }
    }

    void UpdateAnimation(bool moving)
    {
        if (animator == null) return;

        if (isMoving != moving)
        {
            isMoving = moving;
            animator.SetBool(runBoolName, isMoving);
        }
    }

    bool CheckForObstacle(Vector3 direction, out RaycastHit hit)
    {
        bool hasHit = Physics.SphereCast(
            transform.position + Vector3.up * 0.5f,
            collisionRadius,
            direction.normalized,
            out hit,
            collisionCheckDistance,
            obstacleLayer
        );

        return hasHit;
    }

    Vector3 MoveWithCollision(Vector3 targetPosition, float speed)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 intendedPosition = transform.position + direction * speed * Time.deltaTime;

        RaycastHit hit;
        if (CheckForObstacle(direction, out hit))
        {
            Vector3 slideDirection = Vector3.Cross(hit.normal, Vector3.up).normalized;

            Vector3 slidePos1 = transform.position + slideDirection * speed * Time.deltaTime;
            Vector3 slidePos2 = transform.position - slideDirection * speed * Time.deltaTime;

            if (Vector3.Distance(slidePos1, targetPosition) < Vector3.Distance(slidePos2, targetPosition))
            {
                intendedPosition = slidePos1;
            }
            else
            {
                intendedPosition = slidePos2;
            }

            if (CheckForObstacle((intendedPosition - transform.position).normalized, out hit))
            {
                UpdateAnimation(false);
                return transform.position;
            }
        }

        UpdateAnimation(true);
        return intendedPosition;
    }

    void CheckIfLeftWrongPen()
    {
        if (!wasInWrongPen) return;

        if (PenBoundsManager_PetalMeadow.Instance != null)
        {
            bool stillInPen = PenBoundsManager_PetalMeadow.Instance.IsInsidePenArea(transform.position);

            if (!stillInPen)
            {
                // ✅ UPDATE PROPERTIES
                IsInPen = false;
                IsCorrectPen = false;

                if (wrongIcon != null && wrongIcon.activeSelf)
                {
                    wrongIcon.SetActive(false);
                    Debug.Log("❌ Lợn đã RA KHỎI chuồng sai - TẮT dấu X");
                }
                wasInWrongPen = false;
            }
        }
    }

    void CheckIfStillInRange()
    {
        if (IsInPen) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance > player.tamingRadius)
        {
            outsideTimer += Time.deltaTime;
            if (outsideTimer >= timeOutsideToLose)
            {
                LoseTame();
            }
        }
        else
        {
            outsideTimer = 0f;
        }
    }

    void LoseTame()
    {
        if (IsInPen)
        {
            Debug.Log("🏠 Lợn đã ở trong chuồng - không thể mất thuần hóa khi ở xa player!");
            return;
        }

        // ✅ UPDATE PROPERTY
        IsTamed = false;

        outsideTimer = 0f;
        currentTameTimer = 0f;

        if (player != null && player.myPigs.Contains(this))
        {
            player.myPigs.Remove(this);
        }

        SetNewWanderTarget();
        Debug.Log("Lợn đã mất thuần hóa! Cần thu phục lại.");
    }

    void CheckTamingZone()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= player.tamingRadius)
        {
            if (IsInPen && IsCorrectPen)
            {
                currentTameTimer = 0f;
                if (loadingCanvas != null) loadingCanvas.SetActive(false);
                if (loadingFillImage != null) loadingFillImage.fillAmount = 0f;
                return;
            }

            if (loadingCanvas != null && !loadingCanvas.activeSelf)
                loadingCanvas.SetActive(true);

            currentTameTimer += Time.deltaTime;

            if (loadingFillImage != null)
                loadingFillImage.fillAmount = currentTameTimer / timeToTame;

            if (currentTameTimer >= timeToTame)
            {
                BecomeTamed();
            }
        }
        else
        {
            currentTameTimer = 0f;
            if (loadingCanvas != null) loadingCanvas.SetActive(false);
            if (loadingFillImage != null) loadingFillImage.fillAmount = 0f;
        }
    }

    void BecomeTamed()
    {
        Debug.Log($"🎯 BecomeTamed được gọi - IsInPen: {IsInPen}, IsCorrectPen: {IsCorrectPen}");

        // ✅ UPDATE PROPERTY
        IsTamed = true;

        outsideTimer = 0f;
        if (loadingCanvas != null) loadingCanvas.SetActive(false);

        ShowRandomTamingEmotion();

        if (IsInPen && !IsCorrectPen)
        {
            Debug.Log("🔄 Thu phục lại lợn ở SAI chuồng - chuẩn bị dẫn ra ngoài!");

            if (hasRegisteredInPen && GameManager_PetalMeadow.Instance != null)
            {
                GameManager_PetalMeadow.Instance.UnregisterPigFromCorrectPen(pigColor);
                hasRegisteredInPen = false;
            }

            // ✅ UPDATE PROPERTIES
            IsInPen = false;
            IsCorrectPen = false;

            assignedPenIndex = -1;
            hasReachedPenCenter = false;
            justRescued = true;
        }

        if (player != null && player.pigFollowTarget != null)
        {
            followTarget = player.pigFollowTarget;
        }

        player.AddPig(this);
        Debug.Log("Thu phục thành công!");
    }

    void CheckIfEnteredPen()
    {
        if (IsInPen) return;

        if (justRescued)
        {
            if (PenBoundsManager_PetalMeadow.Instance != null)
            {
                bool stillInsideAnyChuong = PenBoundsManager_PetalMeadow.Instance.IsInsidePenArea(transform.position);

                if (!stillInsideAnyChuong)
                {
                    justRescued = false;
                    Debug.Log("✅ Lợn đã ra khỏi chuồng hoàn toàn!");
                }
                else
                {
                    return;
                }
            }
        }

        if (PenBoundsManager_PetalMeadow.Instance == null) return;

        int penIndex = PenBoundsManager_PetalMeadow.Instance.GetPenIndexAtPosition(transform.position);

        if (penIndex != -1)
        {
            penEntryTimer += Time.deltaTime;

            if (penEntryTimer < 0.5f)
            {
                return;
            }

            // ✅ UPDATE PROPERTIES
            IsInPen = true;

            assignedPenIndex = penIndex;
            hasReachedPenCenter = false;
            penEntryTimer = 0f;

            PigColor penColor = PenBoundsManager_PetalMeadow.Instance.GetPenColor(penIndex);
            IsCorrectPen = (pigColor == penColor);

            Vector3 rawCenter = PenBoundsManager_PetalMeadow.Instance.GetPenCenter(penIndex);
            penCenterTarget = new Vector3(rawCenter.x, transform.position.y, rawCenter.z);

            Debug.Log($"🏠 Lợn {pigColor} VÀO CHUỒNG {penColor} - {(IsCorrectPen ? "✅ ĐÚNG" : "❌ SAI")}");

            if (IsCorrectPen)
            {
                // 🔊 PHÁT ÂM THANH KHI VÀO ĐÚNG CHUỒNG
                if (SoundManager_PetalMeadow.Instance != null)
                {
                    SoundManager_PetalMeadow.Instance.PlayVFXSound(3);
                    Debug.Log("🔊 Phát âm thanh lợn vào đúng chuồng!");
                }

                if (correctIcon != null)
                {
                    correctIcon.SetActive(true);
                    StartCoroutine(FlyUpAndFade(correctIcon));
                }
                if (wrongIcon != null) wrongIcon.SetActive(false);
                wasInWrongPen = false;

                if (!hasRegisteredInPen && GameManager_PetalMeadow.Instance != null)
                {
                    GameManager_PetalMeadow.Instance.RegisterPigInCorrectPen(pigColor);
                    hasRegisteredInPen = true;
                }

                if (player != null && player.myPigs.Contains(this))
                {
                    player.myPigs.Remove(this);
                    Debug.Log("✅ Đã ngắt kết nối lợn ĐÚNG với player");
                }
            }
            else
            {
                if (correctIcon != null) correctIcon.SetActive(false);
                if (wrongIcon != null)
                {
                    wrongIcon.SetActive(true);
                    wasInWrongPen = true;
                    Debug.Log("❌ Hiện dấu X - Sẽ chỉ tắt khi lợn RA KHỎI chuồng");
                }

                if (player != null && player.myPigs.Contains(this))
                {
                    player.myPigs.Remove(this);
                    Debug.Log("❌ Đã ngắt kết nối lợn SAI với player");
                }
            }

            if (loadingCanvas != null)
                loadingCanvas.SetActive(false);
        }
        else
        {
            penEntryTimer = 0f;
        }
    }

    void ShowRandomTamingEmotion()
    {
        if (tamingEmotions.Count == 0) return;

        List<GameObject> availableEmotions = new List<GameObject>();
        foreach (var emotion in tamingEmotions)
        {
            if (emotion != null) availableEmotions.Add(emotion);
        }

        if (availableEmotions.Count == 0) return;

        GameObject selectedEmotion = availableEmotions[Random.Range(0, availableEmotions.Count)];

        selectedEmotion.SetActive(true);
        StartCoroutine(FlyUpAndFade(selectedEmotion));
    }

    IEnumerator FlyUpAndFade(GameObject icon)
    {
        if (icon == null) yield break;

        icon.transform.DOKill();

        Vector3 startPos = icon.transform.localPosition;
        Vector3 startScale = icon.transform.localScale;

        CanvasGroup canvasGroup = icon.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = icon.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 1f;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            icon.transform.DOPunchScale(Vector3.one * scalePunchAmount, 0.3f, 5, 0.5f)
        );

        Vector3 targetPos = startPos + Vector3.up * flyUpHeight;
        sequence.Join(
            icon.transform.DOLocalMove(targetPos, flyUpDuration)
                .SetEase(flyUpEase)
        );

        sequence.Insert(flyUpDuration * 0.5f,
            canvasGroup.DOFade(0f, flyUpDuration * 0.5f)
        );

        if (useRotation)
        {
            sequence.Join(
                icon.transform.DOLocalRotate(
                    new Vector3(0, 0, Random.Range(-rotationAmount, rotationAmount)),
                    flyUpDuration,
                    RotateMode.LocalAxisAdd
                ).SetEase(Ease.OutQuad)
            );
        }

        yield return sequence.WaitForCompletion();

        icon.SetActive(false);
        icon.transform.localPosition = startPos;
        icon.transform.localScale = startScale;
        icon.transform.localRotation = Quaternion.identity;
        canvasGroup.alpha = 1f;
    }

    void WanderAround()
    {
        if (isIdling)
        {
            UpdateAnimation(false);
            idleTimer += Time.deltaTime;
            if (idleTimer >= nextIdleTime)
            {
                isIdling = false;
                SetNewWanderTarget();
            }
            return;
        }

        float dist = Vector3.Distance(transform.position, currentWanderTarget);

        if (dist > 0.5f)
        {
            Vector3 newPosition = MoveWithCollision(currentWanderTarget, wanderSpeed);

            if (PenBoundsManager_PetalMeadow.Instance != null)
            {
                newPosition = PenBoundsManager_PetalMeadow.Instance.ClampToMovementArea(newPosition);
            }

            newPosition = AvoidOtherPigs(newPosition);

            transform.position = newPosition;
            LookAtTarget(currentWanderTarget);
        }
        else
        {
            UpdateAnimation(false);
            isIdling = true;
            idleTimer = 0f;
            nextIdleTime = Random.Range(idleTimeMin, idleTimeMax);
        }

        wanderTimer += Time.deltaTime;
        if (wanderTimer >= nextWanderTime)
        {
            SetNewWanderTarget();
        }
    }

    void WanderInPen()
    {
        if (!hasReachedPenCenter)
        {
            float distXZ = Vector2.Distance(
                new Vector2(transform.position.x, transform.position.z),
                new Vector2(penCenterTarget.x, penCenterTarget.z)
            );

            if (distXZ > 2f)
            {
                Vector3 newPosition = MoveWithCollision(penCenterTarget, moveSpeed);
                transform.position = newPosition;
                LookAtTarget(penCenterTarget);
            }
            else
            {
                UpdateAnimation(false);
                hasReachedPenCenter = true;
                SetNewPenWanderTarget();
                Debug.Log("🎯 Lợn đã đến tâm chuồng! Bắt đầu lang thang tự do.");
            }
            return;
        }

        if (isIdling)
        {
            UpdateAnimation(false);
            idleTimer += Time.deltaTime;
            if (idleTimer >= nextIdleTime)
            {
                isIdling = false;
                SetNewPenWanderTarget();
            }
            return;
        }

        float dist = Vector3.Distance(transform.position, currentWanderTarget);

        if (dist > 0.5f)
        {
            Vector3 newPosition = MoveWithCollision(currentWanderTarget, wanderSpeed);

            if (PenBoundsManager_PetalMeadow.Instance != null && assignedPenIndex != -1)
            {
                newPosition = PenBoundsManager_PetalMeadow.Instance.ClampToSpecificPen(newPosition, assignedPenIndex);
            }

            newPosition = AvoidOtherPigs(newPosition);

            transform.position = newPosition;
            LookAtTarget(currentWanderTarget);
        }
        else
        {
            UpdateAnimation(false);
            isIdling = true;
            idleTimer = 0f;
            nextIdleTime = Random.Range(idleTimeMin, idleTimeMax);
        }

        wanderTimer += Time.deltaTime;
        if (wanderTimer >= nextWanderTime)
        {
            SetNewPenWanderTarget();
        }
    }

    void SetNewWanderTarget()
    {
        if (PenBoundsManager_PetalMeadow.Instance == null)
        {
            currentWanderTarget = transform.position;
            return;
        }

        currentWanderTarget = PenBoundsManager_PetalMeadow.Instance.GetRandomPointInMovementArea(transform.position.y);
        wanderTimer = 0f;
        nextWanderTime = Random.Range(minWanderTime, maxWanderTime);
    }

    void SetNewPenWanderTarget()
    {
        if (PenBoundsManager_PetalMeadow.Instance == null || assignedPenIndex == -1)
        {
            currentWanderTarget = transform.position;
            return;
        }

        currentWanderTarget = PenBoundsManager_PetalMeadow.Instance.GetRandomPointInSpecificPen(assignedPenIndex, transform.position.y);
        wanderTimer = 0f;
        nextWanderTime = Random.Range(minWanderTime, maxWanderTime);
    }

    void MoveFollowPlayer()
    {
        if (penEntryTimer > 0f && penEntryTimer < 0.5f)
        {
            return;
        }

        Vector3 targetPos = (followTarget != null) ? followTarget.position : player.transform.position;
        float dist = Vector3.Distance(transform.position, targetPos);

        if (dist > stopDistance)
        {
            Vector3 newPosition = MoveWithCollision(targetPos, moveSpeed);

            if (PenBoundsManager_PetalMeadow.Instance != null)
            {
                newPosition = PenBoundsManager_PetalMeadow.Instance.ClampToAllAreas(newPosition);
            }

            newPosition = AvoidOtherPigs(newPosition);

            transform.position = newPosition;
            LookAtTarget(targetPos);
        }
        else
        {
            UpdateAnimation(false);
        }
    }

    Vector3 AvoidOtherPigs(Vector3 intendedPosition)
    {
        Vector3 finalPosition = intendedPosition;

        foreach (PigBehavior_PetalMeadow otherPig in allPigs)
        {
            if (otherPig == this || otherPig == null) continue;

            Vector3 myPosFlat = new Vector3(finalPosition.x, 0, finalPosition.z);
            Vector3 otherPosFlat = new Vector3(otherPig.transform.position.x, 0, otherPig.transform.position.z);

            float distance = Vector3.Distance(myPosFlat, otherPosFlat);
            float minDistance = pigRadius + otherPig.pigRadius;

            if (distance < minDistance && distance > 0.01f)
            {
                Vector3 pushDirection = (myPosFlat - otherPosFlat).normalized;
                Vector3 newPosFlat = otherPosFlat + pushDirection * minDistance;

                finalPosition.x = newPosFlat.x;
                finalPosition.z = newPosFlat.z;
            }
        }

        return finalPosition;
    }

    void LookAtTarget(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, pigRadius);

        if (Application.isPlaying)
        {
            Vector3 dir = transform.forward;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.5f + dir * collisionCheckDistance, collisionRadius);

            if (penEntryTimer > 0f && penEntryTimer < 0.5f)
            {
                float t = penEntryTimer / 0.5f;
                Gizmos.color = Color.Lerp(Color.yellow, Color.white, Mathf.PingPong(Time.time * 3, 1));
                Gizmos.DrawWireSphere(transform.position, pigRadius + 0.3f);

                Vector3 startPos = transform.position + Vector3.up * 2;
                Vector3 endPos = startPos + Vector3.right * (t * 2);
                Gizmos.DrawLine(startPos, endPos);
            }

            if (justRescued)
            {
                Gizmos.color = new Color(1f, 0.5f, 0f);
                Gizmos.DrawWireSphere(transform.position, pigRadius + 0.5f);
                Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 3);
            }

            if (wasInWrongPen)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, pigRadius + 0.3f);
            }

            if (IsInPen)
            {
                if (!hasReachedPenCenter)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(penCenterTarget, 1f);
                    Gizmos.DrawLine(transform.position, penCenterTarget);
                }
                else
                {
                    Gizmos.color = IsCorrectPen ? Color.green : Color.red;
                    Gizmos.DrawWireSphere(currentWanderTarget, 0.3f);
                    Gizmos.DrawLine(transform.position, currentWanderTarget);
                }
            }
            else if (!IsTamed)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(currentWanderTarget, 0.3f);
                Gizmos.DrawLine(transform.position, currentWanderTarget);
            }
        }
    }
}