using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Define;

public class InputManager
{
    #region [ Field Data ]
    public Action KeyAction = null;
    public Action<MouseEvent> MouseAction = null;
    bool isPress = false;
    float PressTime = 0;
    #endregion [ Field Data ]
    
    public void OnUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.anyKey && KeyAction != null)
            KeyAction.Invoke();

        if(MouseAction != null)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if(isPress == false)
                {
                    MouseAction.Invoke(MouseEvent.PointerDown);
                    PressTime = Time.time;
                }

                MouseAction.Invoke(MouseEvent.Press);
                isPress = true;
            }
            else
            {
                if (isPress)
                {
                    if (Time.time > PressTime + 0.25f)
                        MouseAction.Invoke(MouseEvent.Click);
                    MouseAction.Invoke(MouseEvent.PointerUp);
                }
                isPress = false;
                PressTime = 0;
            }
        }
    }

    public void Clear()
    {
        KeyAction = null;
        MouseAction = null;
    }
    
}
