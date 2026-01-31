using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera _MainCamera;
    public Camera _ShopCamera;
    public FollowPlayer _FollowPlayer;
    public CameraCollider _CameraCollider;
    public ShakeCamera _ShakeCamera;
    //[SerializeField] private float defaultFOV = 60f;

    private Coroutine _fovRoutine;

    private bool _isAiming;
    private Coroutine _switchRoutine;
    private void OnValidate()
    {
        if (_MainCamera == null)
            _MainCamera = Camera.main;

        if (_FollowPlayer == null)
            _FollowPlayer = GetComponent<FollowPlayer>();

        if (_CameraCollider == null)
            _CameraCollider = GetComponent<CameraCollider>();

        if (_ShakeCamera == null)
            _ShakeCamera = GetComponent<ShakeCamera>();
    }

    public Vector3 _ThirdPersonPos;
    public Quaternion _ThirdPersonRotation;
    public Quaternion _P0Rotation;

    //[Button]
    public void SwitchFirstPersionCamera(Action onComplete)
    {
        StartCoroutine(SmoothTransitionToFirstPerson(onComplete));
    }

    public void SwitchThirdPersionCamera()
    {
        StartCoroutine(SmoothTransitionToThirdPerson());
    }

    public void ChangeMainCameraStatus(bool status)
    {
        _ShopCamera.enabled = false;
        _MainCamera.enabled = status;
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            SwitchFirstPersionCamera(SwitchShopCameraCallback);
        }

        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    SwitchThirdPersionCamera();
        //}
    }
#endif


    public void SetFollow(bool value)
    {
        //_FollowPlayer.SetFollow(value);
        //_CameraCollider.SetState(value);
    }

    public void SwitchCameraShop()
    {
        SwitchFirstPersionCamera(SwitchShopCameraCallback);
    }

    private void SwitchShopCameraCallback()
    {
        Debug.Log("Switched to Shop Camera");
    }

    private IEnumerator SmoothTransitionToThirdPerson()
    {
        _MainCamera.enabled = true;
        _ShopCamera.enabled = false;
        //GameController.Instance._weatherEffectController.ChangeCamera(_MainCamera);
        _CameraCollider.dis_max = 5f;
        _CameraCollider.dis_min = 0.5f;
        //GameController.Instance.frustumCullingManager.ResetMaincam();
        float duration = 0.3f;
        float elapsed = 0f;
        Quaternion startP0Rot = Quaternion.Euler(0, 0, 0);
        Quaternion targetP0Rot = _P0Rotation;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 targetPos = _ThirdPersonPos;
        Quaternion targetRot = _ThirdPersonRotation;

        Vector3 startLocalPos = _MainCamera.transform.localPosition;
        Vector3 targetLocalPos = new Vector3(0, 0, -8f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            _FollowPlayer.po.localRotation = Quaternion.Slerp(startP0Rot, targetP0Rot, t);
            _MainCamera.transform.localPosition = Vector3.Lerp(startLocalPos, targetLocalPos, t);

            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
        _MainCamera.transform.localPosition = targetLocalPos;

        _FollowPlayer.enabled = true;
        _CameraCollider.enabled = true;
        //GameController.Instance.SetCamDistance();
        //if (!UIMainManager.Instance.cutsceneIsRunning)
        //    UIMainManager.Instance.JoyStick(true);
    }

    private IEnumerator SmoothTransitionToFirstPerson(Action onComplete)
    {
        //_ShopCamera = GameController.Instance._pThirdPersonController._shopCamera;

        _FollowPlayer.enabled = false;
        _CameraCollider.dis_max = 0.0f;
        _CameraCollider.dis_min = 0.0f;
        _CameraCollider.enabled = false;

        float duration = 0.3f;
        float elapsed = 0f;
        Quaternion startP0Rot = _P0Rotation = _FollowPlayer.po.localRotation;
        Quaternion targetP0Rot = Quaternion.Euler(0, 0, 0);

        Vector3 startPos = _ThirdPersonPos = transform.position;
        Quaternion startRot = _ThirdPersonRotation = transform.rotation;
        Vector3 targetPos = _ShopCamera.transform.position;
        Quaternion targetRot = _ShopCamera.transform.localRotation;
        targetRot = Quaternion.Euler(0, 180f, 0);
        Vector3 startLocalPos = _MainCamera.transform.localPosition;
        Vector3 targetLocalPos = Vector3.zero;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;

            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            _FollowPlayer.po.localRotation = Quaternion.Slerp(startP0Rot, targetP0Rot, t);
            _MainCamera.transform.localPosition = Vector3.Lerp(startLocalPos, targetLocalPos, t);

            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
        _MainCamera.transform.localPosition = targetLocalPos;

        _ShopCamera.enabled = true;
        _MainCamera.enabled = false;
        //GameController.Instance.frustumCullingManager.SetCameraShop();
        //GameController.Instance._weatherEffectController.ChangeCamera(_ShopCamera);
        //UIMainManager.Instance.JoyStick(false);
        onComplete?.Invoke();
    }

    public void SmoothChangeFOV(float targetFOV, float duration)
    {
        if (_MainCamera == null)
        {
            Debug.LogWarning("CameraFOVController: mainCam chưa được gán!");
            return;
        }

        if (_fovRoutine != null)
            StopCoroutine(_fovRoutine);

        _fovRoutine = StartCoroutine(SmoothFOVRoutine(targetFOV, duration));
    }

    private IEnumerator SmoothFOVRoutine(float targetFOV, float duration)
    {
        float startFOV = _MainCamera.fieldOfView;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = Mathf.SmoothStep(0, 1, t); // easing mượt tự nhiên

            _MainCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);

            yield return null;
        }

        _MainCamera.fieldOfView = targetFOV;
        _fovRoutine = null;
    }

}
