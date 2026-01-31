using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimingObstacle : MonoBehaviour
{
    [SerializeField] private float _onDuration = 1.5f;
    [SerializeField] private float _offDuration = 1.5f;
    [SerializeField] private bool _startActive = false;

    private float _timer;
    private bool _isActive;

    public List<Collider> _collider;
    private Renderer _renderer;

    private void Awake()
    {
        //_collider = GetComponent<Collider>();
        _renderer = GetComponent<Renderer>();

        _isActive = _startActive;
        SetState(_isActive);
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_isActive && _timer >= _onDuration)
        {
            _isActive = false;
            _timer = 0f;
            SetState(false);
        }
        else if (!_isActive && _timer >= _offDuration)
        {
            _isActive = true;
            _timer = 0f;
            SetState(true);
        }
    }

    private void SetState(bool active)
    {
        foreach (var collider in _collider) 
        {
            collider.enabled = active;
        }
        //if (_collider != null)
        //    _collider.enabled = active;

        if (_renderer != null)
            _renderer.enabled = active;
    }
}
