using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CMenu : MonoBehaviour
{
    GameObject cMenu_selectBox_TextUI_Heart;
    Vector2[] cMenu_selectBox_TextUI_Heart_loc;

    AudioSource cMenu_Sound;
    public static AudioClip sound_move ;
    public static AudioClip sound_enter;
    private void OnEnable()
    {
        KeyState.UseObjectLock = true;
        Frisk_Walk.MoveLock(true);
        KeyState.UseObjectInHandsLock = true;

        upWaitLock = true;
        downWaitLock = true;
        exitWaitLock = true;
        {//等待方向键和菜单键松开
            Thread arrowUnlock = new(() =>
            {
                if (KeyClick_ThreadCall(KeyClick_ThreadCall_ID.UpKey) || KeyClick_ThreadCall(KeyClick_ThreadCall_ID.DownKey)
                || KeyClick_ThreadCall(KeyClick_ThreadCall_ID.cMenuKey))
                {
                    while (KeyClick_ThreadCall(KeyClick_ThreadCall_ID.UpKey)
                        || KeyClick_ThreadCall(KeyClick_ThreadCall_ID.DownKey)
                            || KeyClick_ThreadCall(KeyClick_ThreadCall_ID.cMenuKey))
                        Thread.Sleep(20);
                }
                upWaitLock = false;
                downWaitLock = false;
                exitWaitLock = false;
            });
            arrowUnlock.Start();
        }

        closeAllMenuSignal = false;
        cMenu_selectBox_TextUI_Heart = gameObject.transform
            .Find("cMenu_selectBox")
            .Find("cMenu_selectBox_TextUI")
            .Find("cMenu_selectBox_TextUI_Heart").gameObject;
        cMenu_Sound=gameObject.transform.Find("cMenu_Sound").gameObject.GetComponent<AudioSource>();        
        {
            Transform uiGO = gameObject.transform
            .Find("cMenu_selectBox")
            .Find("cMenu_selectBox_TextUI");
            float normalX = cMenu_selectBox_TextUI_Heart.transform.localPosition.x;
            float[] yLoc = new float[3] {
                uiGO.Find("cMenu_selectBox_TextUI_object").localPosition.y,
                uiGO.Find("cMenu_selectBox_TextUI_state").localPosition.y,
                uiGO.Find("cMenu_selectBox_TextUI_phone").localPosition.y
            };
            cMenu_selectBox_TextUI_Heart_loc = new Vector2[3] {
                new(normalX, yLoc[0]),
                new(normalX, yLoc[1]),
                new(normalX, yLoc[2]),
            };
        }
        cMenu_selectBox_TextUI_Heart.SetActive(true);
        cMenu_selectBox_TextUI_Heart.transform.localPosition = cMenu_selectBox_TextUI_Heart_loc[0];
        LocID = 0;        
    }
    bool allWaitLock = false;
    bool upWaitLock = false;
    bool downWaitLock = false;
    bool enterWaitLock = false;
    bool exitWaitLock = false;

    private int locID;
    /// <summary>
    /// 当前选中的按钮位置ID
    /// </summary>
    int LocID
    {
        get { return locID; }
        set
        {
            if (value >= 3)
                locID = 0;
            else if (value < 0)
                locID = 3-1;
            else
                locID = value;
        }
    }

    /// <summary>
    /// 仅供在子线程调用，不要在主线程内调用
    /// </summary>
    /// <param name="keyID">使用枚举提供箭头指向参数</param>
    /// <returns></returns>
    bool KeyClick_ThreadCall(KeyClick_ThreadCall_ID keyID)
    {
        bool tmp = false;
        bool wait = false;
        ThreadDelegate.QueueOnMainThread((param) =>
        {
            switch (keyID)
            {
                case KeyClick_ThreadCall_ID.UpKey:
                    tmp = GameKey.UpKeyClick();
                    break;
                    case KeyClick_ThreadCall_ID.DownKey: 
                    tmp = GameKey.DownKeyClick();
                    break;
                case KeyClick_ThreadCall_ID.cMenuKey:
                    tmp = GameKey.MenuKeyClick();
                    break;
            }            
            wait = true;
        }, null);
        while (!wait) ;
        return tmp;
    }
    enum KeyClick_ThreadCall_ID
    {
        UpKey, DownKey,cMenuKey
    }
    /// <summary>
    /// 子菜单通知主菜单关闭的信号
    /// </summary>
    public static bool closeAllMenuSignal;
    private void Update()
    {
        if (!allWaitLock)
        {
            if (!upWaitLock && GameKey.UpKeyClick())
            {                
                upWaitLock = true;
                LocID--;
                cMenu_selectBox_TextUI_Heart.transform.localPosition = cMenu_selectBox_TextUI_Heart_loc[LocID];
                cMenu_Sound.PlayOneShot(sound_move);
                Thread thread = new(() =>
                {
                    while (KeyClick_ThreadCall(KeyClick_ThreadCall_ID.UpKey))
                                Thread.Sleep(20);
                    upWaitLock = false;
                });
                thread.Start();
            }
            else if (!downWaitLock && GameKey.DownKeyClick())
            {
                downWaitLock = true;
                LocID++;
                cMenu_selectBox_TextUI_Heart.transform.localPosition = cMenu_selectBox_TextUI_Heart_loc[LocID];
                cMenu_Sound.PlayOneShot(sound_move);
                Thread thread = new(() =>
                {
                    while (KeyClick_ThreadCall(KeyClick_ThreadCall_ID.DownKey))
                        Thread.Sleep(20);
                    downWaitLock = false;
                });
                thread.Start();
            }
            else if (!enterWaitLock && GameKey.OkKeyClick())
            {
                enterWaitLock = true;
                allWaitLock = true;
                cMenu_Sound.PlayOneShot(sound_enter);
                GameObject InfoBox = gameObject.transform.Find("cMenu_InfoBox").gameObject;
                InfoBox.SetActive(true);
                string subName = "null";
                switch (LocID)
                {
                    case 0:
                        subName = "cMenu_InfoBox_objectUI";
                        break;
                    case 1:
                        subName = "cMenu_InfoBox_stateUI";
                        break;
                    case 2:
                        subName = "cMenu_InfoBox_phoneUI";
                        break;
                }
                GameObject subUIObj = InfoBox.transform.Find(subName).gameObject;
                cMenu_selectBox_TextUI_Heart.SetActive(false);
                subUIObj.SetActive(true);
                Thread thread = new(() =>
                {
                    bool Sub_activeSelf()
                    {
                        bool theBool = false;
                        {
                            bool wait = false;
                            ThreadDelegate.QueueOnMainThread((param) =>
                            {
                                theBool = subUIObj.activeSelf;
                                wait = true;
                            }, null);
                            while (!wait);
                        }
                        return theBool;
                    }
                    bool keyXandZup()
                    {
                        bool theBool = false;
                        {
                            bool wait = false;
                            ThreadDelegate.QueueOnMainThread((param) =>
                            {
                                theBool = (!GameKey.CancelKeyClick() && !GameKey.OkKeyClick());
                                wait = true;
                            }, null);
                            while (!wait) ;
                        }
                        return theBool;
                    }

                    while (Sub_activeSelf())//持续判断子对象是否存活，等待其死亡后继续
                        Thread.Sleep(50);
                    {
                        bool wait = false;
                        ThreadDelegate.QueueOnMainThread((param) =>
                    {
                                InfoBox.SetActive(false);
                        cMenu_selectBox_TextUI_Heart.SetActive(true);
                        wait = true;
                            }, null);
                        while (!wait);
                    }

                    while(!keyXandZup()) Thread.Sleep(50);//等待X键(取消键)和Z键(确定键)松开
                    
                    enterWaitLock = false;
                    allWaitLock = false;
                });
                thread.Start();
            }
            else if (!exitWaitLock && ((GameKey.CancelKeyClick()||GameKey.MenuKeyClick())|| closeAllMenuSignal))
            {                
                exitWaitLock = true;
                        KeyState.UseObjectLock = false;
                        Frisk_Walk.MoveLock(false);
                KeyState.UseObjectInHandsLock = false;
                        gameObject.SetActive(false);
                Thread keyUpWait = new(() =>
                {//等待菜单键松开
                    if (KeyClick_ThreadCall(KeyClick_ThreadCall_ID.cMenuKey))
                    {
                        while (KeyClick_ThreadCall(KeyClick_ThreadCall_ID.cMenuKey)) 
                            Thread.Sleep(20);
                    }
                    KeyManager.WaitLock.cMenuOpenWaitLock = false;
                    exitWaitLock = false;
                });               
                keyUpWait.Start();
            }
        }
    }
}