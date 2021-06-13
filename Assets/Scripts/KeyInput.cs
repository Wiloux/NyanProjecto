using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KeyInput
{
    #region Zoom keys
    static KeyCode[] zoomKeyCodes = new KeyCode[2] { KeyCode.Mouse1, KeyCode.JoystickButton6 };

    public static bool GetZoomKeyDown()
    {
        return Input.GetKeyDown(zoomKeyCodes[0]) || Input.GetKeyDown(zoomKeyCodes[1]);
    }
    public static bool GetZoomKey()
    {
        return Input.GetKey(zoomKeyCodes[0]) || Input.GetKey(zoomKeyCodes[1]);
    }
    public static bool GetZoomKeyUp()
    {
        return Input.GetKeyUp(zoomKeyCodes[0]) || Input.GetKeyUp(zoomKeyCodes[1]);
    }
    #endregion

    #region Escape
    static KeyCode[] pauseKeyCodes = new KeyCode[2] { KeyCode.Escape, KeyCode.JoystickButton9 };

    public static bool GetPauseKeyDown()
    {
        return Input.GetKeyDown(pauseKeyCodes[0]) || Input.GetKeyDown(pauseKeyCodes[1]);
    }
    #endregion

    #region Dash to spear keys
    static KeyCode[] dashKeys = new KeyCode[2] { KeyCode.E, KeyCode.JoystickButton5 };

    public static bool GetDashKeyDown()
    {
        return Input.GetKeyDown(dashKeys[0]) || Input.GetKeyDown(dashKeys[1]);
    }
    #endregion

    #region Fire/Launch
    static KeyCode[] fireKeys = new KeyCode[2] { KeyCode.Mouse0, KeyCode.JoystickButton7 };

    public static bool GetFireKeyDown()
    {
        return Input.GetKeyDown(fireKeys[0]) || Input.GetKeyDown(fireKeys[1]);
    }
    public static bool GetFireKey()
    {
        return Input.GetKey(fireKeys[0]) || Input.GetKey(fireKeys[1]);
    }
    public static bool GetFireKeyUp()
    {
        return Input.GetKeyUp(fireKeys[0]) || Input.GetKeyUp(fireKeys[1]);
    }
    #endregion

    #region Sprint keys
    static KeyCode[] sprintKeys = new KeyCode[2] { KeyCode.LeftShift, KeyCode.JoystickButton10};

    public static bool GetSprintKeyDown()
    {
        return Input.GetKeyDown(sprintKeys[0]) || Input.GetKeyDown(sprintKeys[1]);
    }
    public static bool GetSprintKey()
    {
        return Input.GetKey(sprintKeys[0]) || Input.GetKey(sprintKeys[1]);
    }
    public static bool GetSprintKeyUp()
    {
        return Input.GetKeyUp(sprintKeys[0]) || Input.GetKeyUp(sprintKeys[1]);
    }
    #endregion   
    
    #region Horizontal cam keys
    static string[] horizontalCamAxis = new string[2] { "Mouse X", "RightJoystickHorizontal"};

    public static float GetHorizontalCamAxis()
    {
        return Input.GetAxis(horizontalCamAxis[0]) != 0 ? Input.GetAxis(horizontalCamAxis[0]) : Input.GetAxis(horizontalCamAxis[1]);
    }
    #endregion    
    #region Vertical cam keys
    static string[] verticalCamAxis = new string[2] { "Mouse Y", "RightJoystickVertical" };

    public static float GetVerticalCamAxis()
    {
        //Debug.Log($"Mouse:{Input.GetAxis(verticalCamAxis[0])}\t Controller:{Input.GetAxis(verticalCamAxis[1])}");
        return Input.GetAxis(verticalCamAxis[0]) != 0 ? Input.GetAxis(verticalCamAxis[0]) : Input.GetAxis(verticalCamAxis[1]);
    }
    #endregion
}
