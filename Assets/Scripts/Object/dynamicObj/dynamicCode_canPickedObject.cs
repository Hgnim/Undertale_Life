using static CoreData.CData;
using static CoreData.CData.Objects;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;
using static CoreData.ToolClass;

public class DynamicCode_canPickedObject : MonoBehaviour
{
    bool loadSceneLock;
    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.name == "Frisk" && !KeyState.UseObjectLock)
        {
            if (!loadSceneLock && GameKey.OkKeyClick())
            {
                loadSceneLock = true;
                KeyState.UseObjectLock = true;
                string langName = "";
                if (gameObject.name == ObjTagFind(CPObj_hoe.defTag, CPObjTagHead.alias))
                {
                    langName = ObjTagFind(CPObj_hoe.defTag, CPObjTagHead.langID);
                    CPObj_hoe.Exist = false;
                    Player.BagItem.AddItem(CanPickedObject.hoe, CPObj_hoe.defTag);
                    //从该物件被添加至背包开始，该物件变成了一个单独的带TAG的个体
                }
                else if(gameObject.name== ObjTagFind(CPObj_wheatSeedBag.defTag, CPObjTagHead.alias))
                { 
                        langName = ObjTagFind(CPObj_wheatSeedBag.defTag, CPObjTagHead.langID);
                        CPObj_wheatSeedBag.Exist = false;
                        Player.BagItem.AddItem(CanPickedObject.wheatSeedBag,CPObj_wheatSeedBag.defTag);                        
                }
                GameObject.Destroy(gameObject);
                GameObject.Find("MainCamera").
                     transform.Find("MessageBox").gameObject.
                     GetComponent<MessageBox>().
                    StartMsgBox("message_object_picked,"+langName);

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
            }
        }
    }
}
