using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Author_s_paper : MonoBehaviour
{
    bool loadSceneLock;//�����ε���
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
                            while (!MessageBox.msgboxLock) ;//�ȴ�ί��������ɺ��������֤ͬ��
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
                        while (MessageBox.msgboxLock) ;//�ȴ�ί�з�����ɺ����ѭ������֤ͬ��
                    }


                    //ֻ�������߳��ڵ���
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
                        while (WaitKeyUp()) { Thread.Sleep(5); }//�ȴ������ɿ�
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
