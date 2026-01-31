using UnityEngine;
using UnityEngine.AI;
using System.Collections; // Cần thư viện này để dùng Coroutine

public class bird_FleeState : IState
{
    private ChimController bird;
    
    // Lưu các chỉ số ban đầu
    private float originalSpeed;
    private float originalAcceleration;
    private float originalAngularSpeed;
    private bool originalAutoBraking;

    // Biến kiểm soát việc hồi phục tốc độ
    private Coroutine resetSpeedCoroutine; 
    private bool isRecoveringSpeed = false; // Cờ đánh dấu đang trong quá trình giảm tốc

    // Cấu hình Flee
    private float runMultiplier = 1.5f;     
    private float fleeAcceleration = 60f;   
    private float fleeAngularSpeed = 600f;  
    private float pathUpdateThreshold = 3.0f;
    private float coolDownDuration = 2.0f;  // Thời gian để giảm tốc từ chạy -> đi bộ (2 giây)

    public bird_FleeState(ChimController bird)
    {
        this.bird = bird;
    }

    public void OnEnter()
    {
        if (bird.agent != null && bird.agent.isActiveAndEnabled)
        {
            // --- LOGIC CHỐNG LỖI TỐC ĐỘ ---
            // Nếu đang trong quá trình hồi phục tốc độ (nghĩa là vừa thoát Chase chưa lâu lại bị Chase tiếp)
            // thì ta dùng luôn tốc độ gốc đã lưu trước đó, KHÔNG lưu lại tốc độ hiện tại (vì nó đang lỡ cỡ).
            if (isRecoveringSpeed && resetSpeedCoroutine != null)
            {
                bird.StopCoroutine(resetSpeedCoroutine);
                isRecoveringSpeed = false;
                // originalSpeed giữ nguyên giá trị cũ, không cập nhật
            }
            else
            {
                // Nếu đang ở trạng thái bình thường hoàn toàn, lưu lại chỉ số gốc
                originalSpeed = bird.agent.speed;
                originalAcceleration = bird.agent.acceleration;
                originalAngularSpeed = bird.agent.angularSpeed;
                originalAutoBraking = bird.agent.autoBraking;
            }

            // Thiết lập thông số hoảng loạn
            bird.agent.speed = originalSpeed * runMultiplier;
            bird.agent.acceleration = fleeAcceleration;
            bird.agent.angularSpeed = fleeAngularSpeed;
            bird.agent.autoBraking = false; 

            bird.agent.isStopped = false;
            bird.agent.ResetPath();

            SetFleeDestination();
        }
    }

    public void Execute()
    {
        if (bird.player == null || bird.agent == null) return;

        // Cập nhật đường đi liên tục
        if (!bird.agent.pathPending && bird.agent.remainingDistance < pathUpdateThreshold)
        {
            SetFleeDestination();
        }

        float distSqr = (bird.transform.position - bird.player.position).sqrMagnitude;
        float safeDistSqr = bird.fleeDistance * bird.fleeDistance;

        // Thoát trạng thái khi an toàn
        if (distSqr > safeDistSqr * 1.2f)
        {
            bird.ChangeState<bird_PatrolState>();
        }
    }

    private void SetFleeDestination()
    {
        Vector3 dirToPlayer = bird.transform.position - bird.player.position;
        Vector3 fleeDir = dirToPlayer.normalized;
        Vector3 newPos = bird.transform.position + fleeDir * bird.fleeDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(newPos, out hit, 5.0f, NavMesh.AllAreas))
        {
            bird.agent.SetDestination(hit.position);
        }
        else
        {
            Vector3 randomSide = Vector3.Cross(fleeDir, Vector3.up) * Random.Range(-1f, 1f);
            Vector3 sidePos = bird.transform.position + (fleeDir + randomSide).normalized * bird.fleeDistance;
            if (NavMesh.SamplePosition(sidePos, out hit, 5.0f, NavMesh.AllAreas))
            {
                bird.agent.SetDestination(hit.position);
            }
        }
    }

    public void OnExit()
    {
        if (bird.agent != null && bird.agent.isActiveAndEnabled)
        {
            // 1. Trả lại các chỉ số phụ ngay lập tức
            bird.agent.acceleration = originalAcceleration;
            bird.agent.angularSpeed = originalAngularSpeed;
            bird.agent.autoBraking = originalAutoBraking;
            
            bird.agent.ResetPath();

            // 2. KHÔNG trả lại Speed ngay, mà chạy Coroutine giảm từ từ
            // Gọi StartCoroutine thông qua 'bird' (vì bird là MonoBehaviour)
            resetSpeedCoroutine = bird.StartCoroutine(SmoothSpeedReset(coolDownDuration));
        }
    }

    // Coroutine giúp giảm tốc độ mượt mà
    IEnumerator SmoothSpeedReset(float duration)
    {
        isRecoveringSpeed = true;
        
        float startSpeed = bird.agent.speed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Kiểm tra null để tránh lỗi nếu bird bị destroy giữa chừng
            if (bird == null || bird.agent == null) yield break;

            elapsed += Time.deltaTime;
            // Dùng hàm Lerp để chuyển dần tốc độ về originalSpeed
            bird.agent.speed = Mathf.Lerp(startSpeed, originalSpeed, elapsed / duration);
            
            yield return null; // Chờ frame tiếp theo
        }

        // Đảm bảo kết thúc chính xác
        if (bird != null && bird.agent != null)
        {
            bird.agent.speed = originalSpeed;
        }

        isRecoveringSpeed = false;
    }
}