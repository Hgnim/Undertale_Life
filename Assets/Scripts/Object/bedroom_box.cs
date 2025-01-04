using System.Threading;
using UnityEngine;

public class bedroom_box : MonoBehaviour
{
    bool loadSceneLock;//�����ε���
    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.name == "Frisk" && !KeyState.UseObjectLock)
        {
            if (!loadSceneLock && GameKey.OkKeyClick() && Frisk_Walk.ImgFacingCheck('u'))
            {
                loadSceneLock = true;
                KeyState.UseObjectLock = true;
                GameObject.Find("MainCamera").
                     transform.Find("MessageBox").gameObject.
                     GetComponent<MessageBox>().
                    StartMsgBox("message_bedroom_box");

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
            }
        }
    }
}
