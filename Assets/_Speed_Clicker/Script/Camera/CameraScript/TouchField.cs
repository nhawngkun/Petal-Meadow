using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;


public class TouchField : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool Pressed;
    public Vector2 TouchDist;
    public Vector2 PointerOld;
    //private int fingerId = -1;

    private float mouseSensitivity = 100f;
    private bool mouseLocked = false;

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        // Toggle mouse lock with Alt key
        if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            ToggleMouseLock(!mouseLocked);
        }

        if (mouseLocked)
        {
            // Use mouse delta when locked
            float dx = Input.GetAxis("Mouse X") * mouseSensitivity;
            float dy = Input.GetAxis("Mouse Y") * mouseSensitivity;
            TouchDist = new Vector2(dx, -dy);
            Pressed = true;
            PointerOld = new Vector2(Screen.width / 2f, Screen.height / 2f);
            return;
        }

        if (!Pressed)
        {
            TouchDist = Vector2.zero;
            return;
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        Vector2 currentPointerPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 delta = currentPointerPosition - PointerOld;
        delta *= mouseSensitivity * 0.1f; // scale by sensitivity
        TouchDist = new Vector2(delta.x, -delta.y);
        PointerOld = currentPointerPosition;
#else
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);
                    if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary)
                    {
                        Vector2 currentPointerPosition = touch.position;
                        Vector2 delta = currentPointerPosition - PointerOld;
                        delta *= mouseSensitivity * 0.1f; // scale by sensitivity
                        TouchDist = new Vector2(delta.x, -delta.y);
                        PointerOld = currentPointerPosition;
                        break;
                    }
                }
#endif
    }

    public void ToggleMouseLock(bool lockState)
    {
        mouseLocked = lockState;
        Pressed = mouseLocked;
        TouchDist = Vector2.zero;

        if (mouseLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Allows changing sensitivity at runtime
    public void SetSensitivity(float value)
    {
        mouseSensitivity = Mathf.Clamp(value, 1f, 500f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (mouseLocked) return;

        if (eventData.position.x >= Screen.width / 2)
        {
            Pressed = true;
            PointerOld = eventData.position;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (mouseLocked) return;
        HandlePointerUp();
    }

    private void HandlePointerUp()
    {
        Pressed = false;
        TouchDist = Vector2.zero;
        //fingerId = -1;
    }
}