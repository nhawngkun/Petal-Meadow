using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunMachine : MonoBehaviour
{
    public bool _triggered = false;
    public float pushSpeed = 8;


    [Header("Material Scroll")]
    [SerializeField] private Renderer _renderer;
    [SerializeField] private float _scrollSpeedY = 1.5f;

    private Material _runtimeMat;
    private static readonly int BaseMapID = Shader.PropertyToID("_BaseMap");

    private float _offsetY;
    private void OnValidate()
    {
        _renderer = GetComponent<Renderer>();
    }
    private void Awake()
    {
        // 🔥 Clone material để không ảnh hưởng object khác
        _runtimeMat = _renderer.material;
    }

    private void Update()
    {
        //if (!_triggered) return;

        _offsetY += Time.deltaTime * _scrollSpeedY;
        _runtimeMat.SetTextureOffset(BaseMapID, new Vector2(0f, _offsetY));
    }

    private void OnCollisionEnter(Collision other )
    {
        if (_triggered) return;
        if (!other.gameObject.CompareTag("Player")) return;

        _triggered = true;
        GameController.Instance.PlayerController._playerMove.isPushed = true;
        GameController.Instance.PlayerController._playerMove._tilePushSpeed = pushSpeed;
    }
    private void OnCollisionExit(Collision other)
    {
        if (!_triggered) return;
        if (!other.gameObject.CompareTag("Player")) return;

        _triggered = false;
        GameController.Instance.PlayerController._playerMove.isPushed = false;
        GameController.Instance.PlayerController._playerMove._tilePushSpeed = 0;
    }
}
