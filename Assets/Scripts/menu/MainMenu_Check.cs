using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu_Check : MonoBehaviour
{
    public static bool Check=true;
    void Update()
    {
        if (Check)
        {
            //�����˵��м���������Ƿ�����������ģ�����������������˵��ƶ������
            if (!Frisk_Walk.GetMoveLock())
                Frisk_Walk.MoveLock(true);
            if (!KeyState.cMenuOpenLock)
                KeyState.cMenuOpenLock = true;
            if (!KeyState.UseObjectInHandsLock)
                KeyState.UseObjectInHandsLock = true;
        }
    }
}
