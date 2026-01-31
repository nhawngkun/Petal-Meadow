using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveZDir : MonoBehaviour
{
    [SerializeField] private Transform _pointA;
    [SerializeField] private Transform _pointB;
    [SerializeField] private float _speed = 3f;
    private float _direction;
    private float _zA;
    private float _zB;
    private void Start()
    {
        _zA = _pointA.position.z;
        _zB = _pointB.position.z;

        _direction = Mathf.Sign(_zB - _zA);

        Vector3 pos = transform.position;
        //pos.z = _zA;
        //transform.position = pos;
    }

    private void Update()
    {
        Vector3 pos = transform.position;
        pos.z += _direction * _speed * Time.deltaTime;

        bool reachB = _direction > 0 ? pos.z >= _zB : pos.z <= _zB;
        if (reachB)
            pos.z = _zA;

        transform.position = pos;
    }

}
