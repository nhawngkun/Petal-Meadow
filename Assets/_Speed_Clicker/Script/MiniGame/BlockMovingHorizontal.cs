using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMovingHorizontal : MonoBehaviour
{
    [Header("Move Points")]
    [SerializeField] private Transform _leftPoint;
    [SerializeField] private Transform _rightPoint;

    [Header("Move Data")]
    [SerializeField] private float _speed = 3f;

    [SerializeField] int _direction = 1; // 1 = to right, -1 = to left
    
    private void Start()
    {
        //// đảm bảo start trong khoảng
        //Vector3 pos = transform.position;
        //pos.x = _leftPoint.position.x;
        //transform.position = pos;
    }

    private void Update()
    {
        Vector3 pos = transform.position;

        pos.x += _direction * _speed * Time.deltaTime ;

        if (pos.x >= _rightPoint.position.x)
        {
            pos.x = _rightPoint.position.x;
            _direction = -1;
        }
        else if (pos.x <= _leftPoint.position.x)
        {
            pos.x = _leftPoint.position.x;
            _direction = 1;
        }

        transform.position = pos;
    }
}
