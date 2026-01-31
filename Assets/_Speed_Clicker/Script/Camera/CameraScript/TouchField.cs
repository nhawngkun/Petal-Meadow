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

    private void Update()
    {
        if (!Pressed) return;

#if UNITY_EDITOR
        Vector2 currentPointerPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        TouchDist = currentPointerPosition - PointerOld;
        TouchDist = new Vector2(TouchDist.x, -TouchDist.y);
        PointerOld = currentPointerPosition;
#else
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);
                    if (touch.fingerId == fingerId)
                    {
                        Vector2 currentPointerPosition = touch.position;
                        TouchDist = currentPointerPosition - PointerOld;
                        TouchDist = new Vector2(TouchDist.x, -TouchDist.y);
                        PointerOld = currentPointerPosition;
                        break;
                    }
                }
#endif
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //GameController.Instance.PlayerController.AddBonus(1f);


        if (eventData.position.x >= Screen.width / 2)
        {
#if UNITY_EDITOR
            Pressed = true;
            PointerOld = eventData.position;
#else
                    // Tìm touch nào trùng vị trí eventData
                    foreach (Touch touch in Input.touches)
                    {
                        if (Vector2.Distance(touch.position, eventData.position) < 50f) // khoảng cách sai số nhỏ
                        {
                            fingerId = touch.fingerId;
                            Pressed = true;
                            PointerOld = touch.position;
                            break;
                        }
                    }
#endif
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
#if UNITY_EDITOR
        HandlePointerUp();
#else
                if (Pressed)
                {
                    foreach (Touch touch in Input.touches)
                    {
                        if (touch.fingerId == fingerId && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
                        {
                            HandlePointerUp();
                            break;
                        }
                    }
                }
#endif
    }

    private void HandlePointerUp()
    {
        Pressed = false;
        TouchDist = Vector2.zero;
        //fingerId = -1;
    }
}