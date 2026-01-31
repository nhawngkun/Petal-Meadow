using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeTerrain_Loop : MonoBehaviour
{
    [Header("Timing")]
    public float visibleDuration = 2f;
    public float hiddenDuration = 2f;
    public float fadeDuration = 1f;

    private Material _runtimeMat;
    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");

    private Collider _collider;

    private float _timer;
    private float _fadeTimer;

    private enum State
    {
        Visible,
        FadingOut,
        Hidden,
        FadingIn
    }

    private State _state;

    private void Awake()
    {
        Renderer renderer = GetComponent<Renderer>();
        _runtimeMat = renderer.material;

        _collider = GetComponent<Collider>();

        SetAlphaAndCollider(1f);
        _state = State.Visible;
    }

    private void Update()
    {
        switch (_state)
        {
            case State.Visible:
                _timer += Time.deltaTime;
                if (_timer >= visibleDuration)
                {
                    _timer = 0f;
                    _fadeTimer = 0f;
                    _state = State.FadingOut;
                }
                break;

            case State.FadingOut:
                _fadeTimer += Time.deltaTime;
                SetAlphaAndCollider(Mathf.Lerp(1f, 0f, _fadeTimer / fadeDuration));

                if (_fadeTimer >= fadeDuration)
                {
                    SetAlphaAndCollider(0f);
                    _fadeTimer = 0f;
                    _timer = 0f;
                    _state = State.Hidden;
                }
                break;

            case State.Hidden:
                _timer += Time.deltaTime;
                if (_timer >= hiddenDuration)
                {
                    _timer = 0f;
                    _fadeTimer = 0f;
                    _state = State.FadingIn;
                }
                break;

            case State.FadingIn:
                _fadeTimer += Time.deltaTime;
                SetAlphaAndCollider(Mathf.Lerp(0f, 1f, _fadeTimer / fadeDuration));

                if (_fadeTimer >= fadeDuration)
                {
                    SetAlphaAndCollider(1f);
                    _fadeTimer = 0f;
                    _timer = 0f;
                    _state = State.Visible;
                }
                break;
        }
    }

    private void SetAlphaAndCollider(float alpha)
    {
        // Set alpha
        Color c = _runtimeMat.GetColor(BaseColorID);
        c.a = alpha;
        _runtimeMat.SetColor(BaseColorID, c);

        // Alpha > 0 => collider ON
        if (_collider != null)
            _collider.enabled = alpha > 0f;
    }

    private void OnDestroy()
    {
        if (_runtimeMat != null)
            Destroy(_runtimeMat);
    }
}
