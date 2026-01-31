using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeTerrain : MonoBehaviour
{
    public float delayBeforeFade = 1f;
    public float fadeDuration = 1f;

    private Material _runtimeMat;
    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
    private bool _triggered;

    private void Awake()
    {
        // 👉 Tạo instance material RIÊNG cho object này
        Renderer renderer = GetComponent<Renderer>();
        _runtimeMat = renderer.material;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered) return;
        if (!other.CompareTag("Player")) return;

        _triggered = true;
        StartCoroutine(Co_FadeAndDisable());
    }

    private IEnumerator Co_FadeAndDisable()
    {
        yield return new WaitForSeconds(delayBeforeFade);

        Color baseColor = _runtimeMat.GetColor(BaseColorID);
        float startAlpha = baseColor.a;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            baseColor.a = Mathf.Lerp(startAlpha, 0f, t / fadeDuration);
            _runtimeMat.SetColor(BaseColorID, baseColor);
            yield return null;
        }

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        // 🔥 Cleanup material instance (tránh leak trong editor)
        if (_runtimeMat != null)
        {
            Destroy(_runtimeMat);
        }
    }
}
