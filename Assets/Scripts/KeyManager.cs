using static CoreData.CData.Player;
using static CoreData.CData;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static PlayerChange_yard;
using UnityEngine.SceneManagement;

public class KeyManager : MonoBehaviour
{
    Thread keyListenThread;
    void Start()
    {
        keyListenThread = new(KeyListenThread);
        keyListenThread.Start();
    }
    public static class WaitLock
    {
        internal static bool exitGameWaitLock = false;
        static public bool cMenuOpenWaitLock = false;
        internal static bool UseObjectInHandsWaitLock = false;
    }
    enum KeyName
    {
        MenuKey,EscKey, UseHandsKey
    }
    bool GetKeyClickBool(KeyName key)
    {
        bool wait = false;
        bool tmp = false;
        ThreadDelegate.QueueOnMainThread((param) =>
        {
            switch (key)
            {
                case KeyName.MenuKey:
                    tmp = GameKey.MenuKeyClick(); break;
                case KeyName.EscKey:
                    tmp = GameKey.EscClick();break;
                case KeyName.UseHandsKey:
                    tmp = GameKey.UseHandsKeyClick(); break;
            }
            
            wait = true;
        }, null);
        while (!wait) ;
        return tmp;
    }
    void KeyListenThread()
    {
        while (keyListenThread.IsAlive)
        {
            if (!WaitLock.exitGameWaitLock &&
                GetKeyClickBool(KeyName.EscKey))
            {
                WaitLock.exitGameWaitLock = true;
                    ExitGameWait();
                }
            if (!KeyState.cMenuOpenLock && 
                !WaitLock.cMenuOpenWaitLock &&
                GetKeyClickBool(KeyName.MenuKey))
            {
                    WaitLock.cMenuOpenWaitLock = true;
                    OpenCMenu();
                }
            if (!KeyState.UseObjectInHandsLock && 
                !WaitLock.UseObjectInHandsWaitLock &&
                GetKeyClickBool(KeyName.UseHandsKey))
            {
                WaitLock.UseObjectInHandsWaitLock = true;
                UseObjectInHands();
            }
            Thread.Sleep(10);
        }
    }
    /// <summary>
    /// 长按esc退出游戏
    /// </summary>
    void ExitGameWait()
    {
        ThreadDelegate.QueueOnMainThread((param) =>
        {
            GameObject quitText = GameObject.Find("MainCamera").transform.Find("Quit_Text").gameObject;
            quitText.SetActive(true);
            Text quitText_text = quitText.transform.Find("Quit_Text_text").gameObject.GetComponent<Text>();
            const string mTxt = "quit game";
            const int maxTime = 5;

            Thread runThread = new(() =>
            {
                int i = 0;
                bool keyCheck()
                {
                    bool wait = false;
                    bool tmp = false;
                    ThreadDelegate.QueueOnMainThread((param) =>
                    {
                        quitText_text.text = mTxt.PadRight(mTxt.Length + i, '.');
                        if (!GameKey.EscClick())
                            tmp = false;
                        else
                            tmp = true;
                        wait = true;
                    }, null);
                    while (!wait) ;
                    return tmp;
                }
                for (; i < maxTime; i++)
                {
                    if (!keyCheck())
                    {
                        break;
                    }
                    Thread.Sleep(240);
                }
                if (!keyCheck())//最后一次检查，避免滞后
                {
                    goto exit;
                }
                {
                    bool wait = false;
                    ThreadDelegate.QueueOnMainThread((param) =>
                        {
#if UNITY_EDITOR
                            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
                            wait = true;
                        }, null);
                    while (!wait) ;
                }
                goto over;
    exit:;//停止退出程序
                {
                    bool wait = false;
                    ThreadDelegate.QueueOnMainThread((param) =>
                    {
                        quitText_text.text = "";
                        quitText.SetActive(false);
                        wait = true;
                    }, null);
                    while (!wait) ;
                }
    over:;
                WaitLock.exitGameWaitLock = false;
            });
            runThread.Start();
        }, null);
    }
    /// <summary>
    /// 打开C键菜单
    /// </summary>
    void OpenCMenu()
    {
        ThreadDelegate.QueueOnMainThread((param) =>
        {
            GameObject.Find("MainCamera").transform.Find("cMenu").gameObject.SetActive(true);
        }, null);
    }
    /// <summary>
    /// 使用手中的物品
    /// </summary>
    void UseObjectInHands()
    {
        bool keyVup()
        {
            bool theBool = false;
            {
                bool wait = false;
                ThreadDelegate.QueueOnMainThread((param) =>
                {
                    theBool = (!GameKey.UseHandsKeyClick());
                    wait = true;
                }, null);
                while (!wait) ;
            }
            return theBool;
        }

        {
            bool wait= false;
            ThreadDelegate.QueueOnMainThread((param) =>
            {
                GameObject frisk = GameObject.Find("Frisk");
                PlayerChange_yard.TileList tl = PlayerChange_yard.TileList.none;
                switch (MainItem.GetItem(MainItem.PickedLoc.hand))
                {
                    case Objects.CanPickedObject.hoe:
                        if (SceneManager.GetActiveScene().name == "yard")
                        {
                            tl = PlayerChange_yard.TileList.dirt;
                            PlayerChange_yard.Instance.TilemapDraw(tl, frisk.transform.position.x, frisk.transform.position.y);
                        }
                        break;
                    case Objects.CanPickedObject.wheatSeedBag:
                        if (SceneManager.GetActiveScene().name == "yard")
                        {
                            tl = PlayerChange_yard.TileList.wheat;
                            PlayerChange_yard.Instance.TilemapDraw(tl, frisk.transform.position.x, frisk.transform.position.y);
                        }
                        break;
                }
                //注意，不能提前执行到此处(例如调用的方法中包含线程结尾执行)，必须等到所有任务执行完毕后才能到这。
                //因为到此处后就直接结束任务了，很可能出现意想不到的bug
                wait = true;
            }, null);
            while (wait) ;
        }

        while (!keyVup()) Thread.Sleep(50);//等待V键松开
        WaitLock.UseObjectInHandsWaitLock = false;
    }
}
