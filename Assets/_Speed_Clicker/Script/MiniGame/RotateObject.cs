using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private bool _clockwise = true;
    [SerializeField] private float _rotateSpeed = 90f; // độ/giây
    private void Update()
    {
        float dir = _clockwise ? 1f : -1f;
        transform.Rotate(Vector3.up, dir * _rotateSpeed * Time.deltaTime, Space.World);
    }
}
