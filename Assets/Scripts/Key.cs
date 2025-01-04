using UnityEngine;
using SK.Framework;

namespace SK.Framework
{
    /// <summary>
    /// XBox按键
    /// </summary>
    public static class XBox
    {
        /// <summary>
        /// 左侧摇杆水平轴
        /// X axis
        /// </summary>
        public const string LeftStickHorizontal = "LeftStickHorizontal";
        /// <summary>
        /// 左侧摇杆垂直轴
        /// Y axis
        /// </summary>
        public const string LeftStickVertical = "LeftStickVertical";
        /// <summary>
        /// 右侧摇杆水平轴
        /// 4th axis
        /// </summary>
        public const string RightStickHorizontal = "RightStickHorizontal";
        /// <summary>
        /// 右侧摇杆垂直轴
        /// 5th axis
        /// </summary>
        public const string RightStickVertical = "RightStickVertical";
        /// <summary>
        /// 十字方向盘水平轴
        /// 6th axis
        /// </summary>
        public const string DPadHorizontal = "DPadHorizontal";
        /// <summary>
        /// 十字方向盘垂直轴
        /// 7th axis
        /// </summary>
        public const string DPadVertical = "DPadVertical";
        /// <summary>
        /// LT
        /// 9th axis
        /// </summary>
        public const string LT = "LT";
        /// <summary>
        /// RT
        /// 10th axis
        /// </summary>
        public const string RT = "RT";
        /// <summary>
        /// 左侧摇杆按键
        /// joystick button 8
        /// </summary>
        public const KeyCode LeftStick = KeyCode.JoystickButton8;
        /// <summary>
        /// 右侧摇杆按键
        /// joystick button 9
        /// </summary>
        public const KeyCode RightStick = KeyCode.JoystickButton9;
        /// <summary>
        /// A键
        /// joystick button 0
        /// </summary>
        public const KeyCode A = KeyCode.JoystickButton0;
        /// <summary>
        /// B键
        /// joystick button 1
        /// </summary>
        public const KeyCode B = KeyCode.JoystickButton1;
        /// <summary>
        /// X键
        /// joystick button 2
        /// </summary>
        public const KeyCode X = KeyCode.JoystickButton2;
        /// <summary>
        /// Y键
        /// joystick button 3
        /// </summary>
        public const KeyCode Y = KeyCode.JoystickButton3;
        /// <summary>
        /// LB键
        /// joystick button 4
        /// </summary>
        public const KeyCode LB = KeyCode.JoystickButton4;
        /// <summary>
        /// RB键
        /// joystick button 5
        /// </summary>
        public const KeyCode RB = KeyCode.JoystickButton5;
        /// <summary>
        /// View视图键
        /// joystick button 6
        /// </summary>
        public const KeyCode View = KeyCode.JoystickButton6;
        /// <summary>
        /// Menu菜单键
        /// joystick button 7
        /// </summary>
        public const KeyCode Menu = KeyCode.JoystickButton7;
    }
}

public static class GameKey 
{
    readonly static KeyCode[] okKey = new KeyCode[] { XBox.X,KeyCode.Z, KeyCode.Return };
    readonly static KeyCode[] cancelKey =new KeyCode[] { XBox.B,KeyCode.X,KeyCode.RightShift};
    readonly static KeyCode[] menuKey=new KeyCode[] { XBox.Y,KeyCode.C,KeyCode.RightControl};
    readonly static KeyCode[] useHandsKey = new KeyCode[] { XBox.A, KeyCode.V,KeyCode.Backslash};
    readonly static KeyCode[] escKey = new KeyCode[] {XBox.Menu, KeyCode.Escape };
    readonly static KeyCode upKey =  KeyCode.UpArrow ;
    readonly static KeyCode downKey = KeyCode.DownArrow;
    readonly static KeyCode leftKey=KeyCode.LeftArrow;
    readonly static KeyCode rightKey=KeyCode  .RightArrow;
    static float DPadH() {return Input.GetAxis(XBox.DPadHorizontal); }
    static float DPadV() {return Input.GetAxis(XBox.DPadVertical); }

    public static bool EscClick()
    {
        foreach (KeyCode k in escKey)
        {
            if (Input.GetKey(k))
                return true;
        }
        return false;
    }
    public static bool OkKeyClick()
    {
        foreach(KeyCode k in okKey)
        {           
           if( Input.GetKey(k))
                return true;
        }
       return false;        
    }
    /*public static bool OkKeyUp()
    {
        foreach (KeyCode k in okKey)
        {
            if (Input.GetKeyUp(k))
                return true;
        }
        return false;
    }*/
    public static bool CancelKeyClick()
    {
        foreach(KeyCode k in cancelKey)
        {
            if(Input.GetKey(k))return true;
        }
        return false;
    }
    public static bool MenuKeyClick()
    {
        foreach(KeyCode k in menuKey)
        {
            if ( Input.GetKey(k))return true;
        }
        return false;
    }
    public static bool UseHandsKeyClick()
    {
        foreach (KeyCode k in useHandsKey)
        {
            if (Input.GetKey(k)) return true;
        }
        return false;
    }

    public static bool UpKeyClick()
    {
        if (Input.GetKey(upKey)) return true;
        else if (DPadV() < 0) return true;
        else return false;
    }
    public static bool DownKeyClick()
    {
        if (Input.GetKey(downKey)) return true;
        else if (DPadV() > 0) return true;
        else return false;
    }
    public static bool LeftKeyClick()
    {
        if (Input.GetKey(leftKey)) return true;
        else if (DPadH() < 0) return true;
        else return false;
    }
    public static bool RightKeyClick()
    {
        if (Input.GetKey(rightKey)) return true;
        else if (DPadH() > 0) return true;
        else return false;
    }
}

/// <summary>
/// 集中放置某些功能的锁状态，例如某些东西不能在某处调用，需要锁住
/// </summary>
public static class KeyState
{
    /// <summary>
    /// 玩家属性(C键)菜单锁
    /// </summary>
    public static bool cMenuOpenLock = true;
    /// <summary>
    /// 玩家使用场景物品锁，打开菜单时应当锁住
    /// </summary>
    public static bool UseObjectLock = false;
    /// <summary>
    /// 使用手中的物品锁
    /// </summary>
    public static bool UseObjectInHandsLock= true;
}
