using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingBall : MonoBehaviour
{
    [Header("Rolling")]
    [SerializeField] private float _torqueForce = 15f;
    [SerializeField] private float _maxAngularSpeed = 50f;

    [Header("Reset")]
    [SerializeField] private Transform _startPoint;
    [SerializeField] private float _resetDelay = 0.1f;

    private Rigidbody _rb;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotationY
                        | RigidbodyConstraints.FreezeRotationZ;

        _rb.maxAngularVelocity = 200f; // cho phép xoay nhanh
    }

   
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ResetZone"))
        {
            ResetBall();
        }
    }
    [SerializeField] private float _downhillAccel = 2.5f;

    private void FixedUpdate()
    {
        _rb.AddForce(Vector3.down * _downhillAccel, ForceMode.Acceleration);
    }
    private void ResetBall()
    {

        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        transform.position = _startPoint.position;
        transform.rotation = Quaternion.identity;
    }


}
