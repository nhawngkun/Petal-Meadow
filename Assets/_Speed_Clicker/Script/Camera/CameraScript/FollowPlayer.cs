using NabaGame.Core.Runtime.Singleton;
using NabaGame.Core.Runtime.TickManager;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;


public class FollowPlayer : MonoBehaviour
{
    public TouchField _touchField;
    [SerializeField] private Transform cam;
    [SerializeField] private CameraCollider _cameraCollider;
    [SerializeField] private Transform target;
    [SerializeField] public Transform po;
    [SerializeField] private Transform p1;
    [SerializeField] private float smooth_speed;
    [SerializeField] private float speedRotate;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;
    //private Transform trans;
    private float h, v;
    private Vector3 delta, delta_p0;

    private void Start()
    {
        //trans = this.transform;
        if (_touchField == null)
        {
            //_touchField = UIMainManager.Instance._TouchField;
        }
    }

    void LateUpdate()
    {
        //if (frustumCulling != null)
        //    frustumCulling.Culling();
        if (target == null) return;

        float dt = Time.deltaTime;

        // Di chuyển mượt tới vị trí target (mượt theo thời gian, không phụ thuộc FPS)
        //float t = 1f - Mathf.Exp(-smooth_speed * dt);
        // transform.position = Vector3.Lerp(transform.position, target.position, t);
        transform.position = Vector3.Lerp(transform.position, target.position, smooth_speed);

        // Lấy đầu vào từ touch
        float h = _touchField.TouchDist.x;   // yaw
        float v = _touchField.TouchDist.y;   // pitch

        // Tính góc xoay theo tốc độ/giây
        float yawDelta = h * speedRotate * dt; // quanh Y (thế giới)
        float pitchDelta = v * speedRotate * dt; // quanh X (local của po)

        // Xoay theo trục thế giới và local
        transform.Rotate(0f, yawDelta, 0f, Space.World);
        po.Rotate(pitchDelta, 0f, 0f, Space.Self);

        // Clamp trục X của po (pitch)
        Vector3 euler = transform.localEulerAngles;
        Vector3 eulerPo = po.localEulerAngles;

        eulerPo.x = ClampSigned(eulerPo.x, minY, maxY); // dùng minY/maxY sẵn có

        // Gán lại giá trị sau clamp; giữ transform sạch X/Z như logic cũ
        transform.localEulerAngles = new Vector3(0f, euler.y, 0f);
        po.localEulerAngles = new Vector3(eulerPo.x, 0f, 0f);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static float ClampSigned(float angle, float min, float max)
    {
        // 0..360 -> -180..180 rồi mới clamp để tránh nhảy góc
        angle = (angle > 180f) ? angle - 360f : angle;
        return Mathf.Clamp(angle, min, max);
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
        _cameraCollider.SetTarget(_target);
    }
}