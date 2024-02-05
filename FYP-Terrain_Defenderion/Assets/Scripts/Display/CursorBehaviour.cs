using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorBehaviour : MonoBehaviour
{
    public void ControlCursor(bool state)
    {
        Cursor.visible = state;
        if (!state)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }
    public static void StaticControlCursor(bool state)
    {
        Cursor.visible = state;
        if (!state)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }
    public static bool GetCursorState()
    {
        switch (Cursor.lockState)
        {
            case CursorLockMode.Locked:
                return true;
                break;
            case CursorLockMode.None:
                return false;
                break;
        }
        return false;
    }
}
