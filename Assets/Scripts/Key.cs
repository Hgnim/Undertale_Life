using UnityEngine;
using SK.Framework;

namespace SK.Framework
{
    /// <summary>
    /// XBox����
    /// </summary>
    public static class XBox
    {
        /// <summary>
        /// ���ҡ��ˮƽ��
        /// X axis
        /// </summary>
        public const string LeftStickHorizontal = "LeftStickHorizontal";
        /// <summary>
        /// ���ҡ�˴�ֱ��
        /// Y axis
        /// </summary>
        public const string LeftStickVertical = "LeftStickVertical";
        /// <summary>
        /// �Ҳ�ҡ��ˮƽ��
        /// 4th axis
        /// </summary>
        public const string RightStickHorizontal = "RightStickHorizontal";
        /// <summary>
        /// �Ҳ�ҡ�˴�ֱ��
        /// 5th axis
        /// </summary>
        public const string RightStickVertical = "RightStickVertical";
        /// <summary>
        /// ʮ�ַ�����ˮƽ��
        /// 6th axis
        /// </summary>
        public const string DPadHorizontal = "DPadHorizontal";
        /// <summary>
        /// ʮ�ַ����̴�ֱ��
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
        /// ���ҡ�˰���
        /// joystick button 8
        /// </summary>
        public const KeyCode LeftStick = KeyCode.JoystickButton8;
        /// <summary>
        /// �Ҳ�ҡ�˰���
        /// joystick button 9
        /// </summary>
        public const KeyCode RightStick = KeyCode.JoystickButton9;
        /// <summary>
        /// A��
        /// joystick button 0
        /// </summary>
        public const KeyCode A = KeyCode.JoystickButton0;
        /// <summary>
        /// B��
        /// joystick button 1
        /// </summary>
        public const KeyCode B = KeyCode.JoystickButton1;
        /// <summary>
        /// X��
        /// joystick button 2
        /// </summary>
        public const KeyCode X = KeyCode.JoystickButton2;
        /// <summary>
        /// Y��
        /// joystick button 3
        /// </summary>
        public const KeyCode Y = KeyCode.JoystickButton3;
        /// <summary>
        /// LB��
        /// joystick button 4
        /// </summary>
        public const KeyCode LB = KeyCode.JoystickButton4;
        /// <summary>
        /// RB��
        /// joystick button 5
        /// </summary>
        public const KeyCode RB = KeyCode.JoystickButton5;
        /// <summary>
        /// View��ͼ��
        /// joystick button 6
        /// </summary>
        public const KeyCode View = KeyCode.JoystickButton6;
        /// <summary>
        /// Menu�˵���
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
/// ���з���ĳЩ���ܵ���״̬������ĳЩ����������ĳ�����ã���Ҫ��ס
/// </summary>
public static class KeyState
{
    /// <summary>
    /// �������(C��)�˵���
    /// </summary>
    public static bool cMenuOpenLock = true;
    /// <summary>
    /// ���ʹ�ó�����Ʒ�����򿪲˵�ʱӦ����ס
    /// </summary>
    public static bool UseObjectLock = false;
    /// <summary>
    /// ʹ�����е���Ʒ��
    /// </summary>
    public static bool UseObjectInHandsLock= true;
}
