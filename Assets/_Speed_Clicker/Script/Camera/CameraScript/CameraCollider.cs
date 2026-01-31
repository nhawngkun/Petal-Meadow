using System.Runtime.CompilerServices;
using DG.Tweening;
using Sirenix.Utilities;
using UnityEngine;


public class CameraCollider : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _cam;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] public float dis_min;
    [SerializeField] public float dis_max;
    [SerializeField] private float offer;
    private bool _isZoom;
    private Tween _tZoom;
    public bool isCamCut;

    public void SetTarget(Transform _target)
    {
        this._target = _target;
    }

    public void SetDistancePlayerMax(float value)
    {
        DOTween.To(() => dis_max, x => dis_max = x, value, 0.4f);
    }

    private RaycastHit[] _hits = new RaycastHit[1]; // reuse để tránh alloc

    private void LateUpdate()
    {
        if (_target == null || isCamCut) return;

        Vector3 forward = _cam.TransformDirection(Vector3.forward);
        int hitCount = Physics.RaycastNonAlloc(_target.position, -forward, _hits, 100f, _layerMask);

        float targetDistance;

        if (hitCount > 0)
        {
            RaycastHit hit = _hits[0];
            targetDistance = Mathf.Max(Vector3.Distance(hit.point, _target.position) - 0.1f, dis_min);
        }
        else
        {
            targetDistance = dis_max;
        }

        SetPosition(targetDistance);
    }

    private void SetPosition(float targetDistance)
    {
        float currentZ = Mathf.Abs(_cam.transform.localPosition.z);
        float smoothZ = Mathf.Lerp(currentZ, targetDistance, 0.5f);
        smoothZ = Mathf.Clamp(smoothZ + offer, dis_min, dis_max);

        _cam.localPosition = new Vector3(0f, 0f, -smoothZ);
    }


    public void ZoomIn(bool isHold)
    {
        if (isHold)
        {
            _tZoom = DOTween.To(() => offer, x => offer = x, -1f, 5f);
        }
        else
        {
            if (_isZoom) return;
            _isZoom = true;
            DOTween.To(() => offer, x => offer = x, 0.7f, 0.08f).OnComplete(() =>
            {
                DOTween.To(() => offer, x => offer = x, 1f, 0.08f).OnComplete(() => { _isZoom = false; });
            });
        }
    }

    public void ZoomOut()
    {
        _tZoom.Pause();
        DOTween.To(() => offer, x => offer = x, 1f, 0.5f);
    }
}