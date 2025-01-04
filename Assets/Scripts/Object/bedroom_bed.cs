using System.Threading;
using UnityEngine;

public class Bedroom_bed : MonoBehaviour
{
    bool loadSceneLock;//�����ε���
    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.name == "Frisk" && !KeyState.UseObjectLock)
        {
            if (!loadSceneLock && GameKey.OkKeyClick() && Frisk_Walk.ImgFacingCheck('r'))
            {               
                loadSceneLock = true;
                KeyState.UseObjectLock = true;
                GameObject.Find("MainCamera").
                     transform.Find("MessageBox").gameObject.
                     GetComponent<MessageBox>().
                     StartMsgBox("message_bedroom_bed");

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
