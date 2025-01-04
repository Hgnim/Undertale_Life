using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Author_s_paper : MonoBehaviour
{
    bool loadSceneLock;//避免多次调用
    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.name == "Frisk" && !KeyState.UseObjectLock)
        {
            if (!loadSceneLock && GameKey.OkKeyClick() && (Frisk_Walk.ImgFacingCheck('l')|| Frisk_Walk.ImgFacingCheck('u')))
            {
                loadSceneLock = true;
                KeyState.UseObjectLock = true;
                Thread runThread = new(() =>
                {
                    for (int num = 1; num <= 7; num++)
                    {
                        if (num == 1)
                        {
                            ThreadDelegate.QueueOnMainThread((param) =>
                            {
                                GameObject.Find("MainCamera").
                                  transform.Find("MessageBox").gameObject.
                                  GetComponent<MessageBox>().
                                  StartMsgBox("message_author_s_paper_" + num.ToString(), prefix: "", haveNext: true,waitTime:35,soundSpeedDown:2);
                            }, null);
                            while (!MessageBox.msgboxLock) ;//等待委托启动完成后继续，保证同步
                        }
                        else if (num == 7)
                        {
                            ThreadDelegate.QueueOnMainThread((param) =>
                            {
                                GameObject.Find("MainCamera").
                                transform.Find("MessageBox").gameObject.
                                GetComponent<MessageBox>().
                                StartMsgBox("message_author_s_paper_" + num.ToString(), waitTime: 35, soundSpeedDown: 2);
                            }, null);
                            while (!MessageBox.msgboxLock) ;
                        }
                        else
                        {
                            ThreadDelegate.QueueOnMainThread((param) =>
                            {
                                GameObject.Find("MainCamera").
                                transform.Find("MessageBox").gameObject.
                                GetComponent<MessageBox>().
                                StartMsgBox("message_author_s_paper_" + num.ToString(), haveNext: true, waitTime: 35, soundSpeedDown: 2);
                            }, null);
                            while (!MessageBox.msgboxLock) ;
                        }
                        while (MessageBox.msgboxLock) ;//等待委托方法完成后继续循环，保证同步
                    }


                    //只能在子线程内调用
                    bool WaitKeyUp()
                    {
                        bool wait = false;
                        bool tmp = false;
                        ThreadDelegate.QueueOnMainThread((param) =>
                        {
                            tmp = GameKey.OkKeyClick();
                            wait = true;
                        }, null);
                        while (!wait) ;
                        return tmp;
                    }
                    Thread waitEnd = new(() => {
                        while (MessageBox.msgboxLock) ;
                        while (WaitKeyUp()) { Thread.Sleep(5); }//等待按键松开
                        KeyState.UseObjectLock = false;
                        loadSceneLock = false;
                    });
                    waitEnd.Start();

                });
                runThread.Start();                
            }
        }
    }
}
