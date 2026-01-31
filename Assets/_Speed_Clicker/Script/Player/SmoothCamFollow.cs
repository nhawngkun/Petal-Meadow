using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCamFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0, 5, -8);   // chiều cao, góc nhìn, khoảng cách
    public float smoothTime = 0.25f;                 // độ trễ follow

    private Vector3 _velocity = Vector3.zero;

    void LateUpdate()
    {
        if (!target) return;

        // --- Tính vị trí mong muốn ---
        Vector3 desiredPos = new Vector3(
            target.position.x + offset.x,    // follow X + offset X (nếu muốn lệch trái/phải)
            target.position.y + offset.y,    // chiều cao
            target.position.z + offset.z     // khoảng cách / góc nhìn
        );

        // --- Smooth follow ---
        Vector3 smoothedPos = Vector3.SmoothDamp(
            transform.position,
            desiredPos,
            ref _velocity,
            smoothTime
        );

        transform.position = smoothedPos;
        // --- CHỈ XOAY TRỤC X ĐỂ NHÌN XUỐNG ---
        Vector3 lookDir = target.position - transform.position;
        float angleX = Quaternion.LookRotation(lookDir).eulerAngles.x;

        transform.rotation = Quaternion.Euler(
            angleX,              // chỉ xoay pitch (nhìn xuống)
            transform.rotation.eulerAngles.y, // giữ nguyên yaw
            0f                   // roll luôn 0 để camera không nghiêng lệch
        );

        //// --- LookAt Target (ổn định, không xoay lung tung) ---
        //transform.LookAt(target.position + Vector3.up * 0.5f);
        //// thêm 0.5f để nhìn vào thân nhân vật, không nhìn chân
    }
}
