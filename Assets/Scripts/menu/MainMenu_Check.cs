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
            //在主菜单中检查下列项是否被其它代码更改，避免出现主角在主菜单移动的情况
            if (!Frisk_Walk.GetMoveLock())
                Frisk_Walk.MoveLock(true);
            if (!KeyState.cMenuOpenLock)
                KeyState.cMenuOpenLock = true;
            if (!KeyState.UseObjectInHandsLock)
                KeyState.UseObjectInHandsLock = true;
        }
    }
}
