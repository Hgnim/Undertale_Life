using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class StartGameShowsStory : MonoBehaviour
{
    public Sprite[] storyImg;
    //public AudioClip background_music;
    public AudioClip txt_sound;

    GameObject obj_ui;
    SpriteRenderer obj_img;
    GameObject obj_text;
    //AudioSource obj_bgSound;
    AudioSource obj_txtSound;
    void Start()
    {
#if !UNITY_EDITOR
        Cursor.visible = false;
#endif
        obj_ui = gameObject.transform.Find("StartGameShowsStory_UI").gameObject;
        obj_img = obj_ui.transform.Find("StartGameShowsStory_UI_img").gameObject.GetComponent<SpriteRenderer>();
        obj_text= obj_ui.transform.Find("StartGameShowsStory_UI_text").gameObject;
        //obj_bgSound=gameObject.transform.Find("StartGameShowsStory_backgroundSound").gameObject.GetComponent<AudioSource>();
        obj_txtSound = gameObject.transform.Find("StartGameShowsStory_txtSound").gameObject.GetComponent<AudioSource>();
    }
    enum RunState{none,start,skip,end}
    RunState runState = RunState.none;
     void Update()
    {
        if (runState == RunState.none)
        {
            runState= RunState.start;
            MessageBox msgb = GameObject.Find("MainCamera").
                                   transform.Find("MessageBox").gameObject.
                                   GetComponent<MessageBox>();

            Thread t = new(() =>
            {
                const int sleepTime = 3500;//切换每一页等待的时间
                const int textSpeed = 190;//文本逐字输出的速度,ms

                {
                    bool unityEditorSkip = false;
#if UNITY_EDITOR
                    unityEditorSkip = true;
#endif
                    if(unityEditorSkip) goto UnityEditorSkip;
                }
                    for (int i = 0; i < 5; i++)
                {
                    int[] adWaitTimel = null;
                    //bool haveNext = true;
                    if (i != 0)
                    {
                        bool wait = false;
                        ThreadDelegate.QueueOnMainThread((param) =>
                        {
                            obj_text.GetComponent<Text>().text = "";
                            wait = true;
                        }, null);
                        ImgVisibility(false);
                        while (!wait) ;
                    }
                    {
                        int imgNum=-1;
                        if (i < 2)
                            imgNum = i;
                        else if(i!=2)
                            imgNum = i - 1;
                        if (imgNum != -1)
                        {
                            bool wait = false;
                            ThreadDelegate.QueueOnMainThread((param) =>
                            {
                                obj_img.sprite = storyImg[imgNum];
                                wait = true;
                            }, null);
                            while (!wait) ;
                        }
                    }
                    if (i == 2)
                        adWaitTimel = new int[] { 0, textSpeed, 2, (int)(textSpeed*3.4) };
                    /*if (i == 3) 禁用了haveNext的设置，因为用户可能需要中途跳过该剧情，使用haveNext可能造成不必要的麻烦
                        haveNext = false;*/
                    if (i != 4)
                    {
                        bool wait = false;
                        ThreadDelegate.QueueOnMainThread((param) =>
                        {
                            msgb.StartMsgBox("MainMenu_StartGameShowsStory_text" + (i + 1).ToString(),
                                audioSourceObject: obj_txtSound, audio: txt_sound,
                                gameObject_TextUI: obj_ui, gameObject_TextUI_TextObj: obj_text,
                                prefix: "",/* haveNext: haveNext,*/ keyContinue: false,banChangeTextUI:true,
                               waitTime: textSpeed, waitTime_advanced: adWaitTimel
                                );
                            wait = true;
                        }, null);
                        while (!wait) ;
                    }
                    if(i!=2)
                    ImgVisibility(true);
                    if (i != 4)
                        while (!MessageBox.msgOutputDone && !(runState == RunState.skip)) ;//双重检查，因为大部分时间都在等待逐字输出
                    if (runState == RunState.skip)
                    {
                        //sleep之前，检查一下是否跳过动画
                        MessageBox.codeContinue = true;
                        MessageBox.codeSkip = true;
                        while (MessageBox.msgboxLock) ;
                        break;
                    }
                    Thread.Sleep(sleepTime);
                    if (i != 4) { 
                        MessageBox.codeContinue = true;
                        while (MessageBox.msgboxLock) ;//等待委托启动，保证同步
                    }                    
                }
UnityEditorSkip:;//如果是开发工具调试则跳过该动画
                runState = RunState.end;
                {
                    for (int b = 0; b <= 1; b++)
                    {                        
                        switch (b)
                        {
                            case 0:
                                BlackScreen.onlyBlackScreen = BlackScreen.OnlyBlackScreen.black;
                                break;
                            case 1:
                                {
                                    bool wait = false;
                                    ThreadDelegate.QueueOnMainThread((param) =>
                                    {
                                        gameObject.SetActive(false);
                                        GameObject.Find("MainMenu").transform.Find("MainMenu_TextUI").gameObject.SetActive(true);
                                        wait = true;
                                    }, null);
                                    while (!wait) ;
                                }
                                BlackScreen.onlyBlackScreen = BlackScreen.OnlyBlackScreen.back;
                                break;
                        }

                        if (b == 0)
                        {
                            ThreadDelegate.QueueOnMainThread((param) =>
                            {
                                GameObject.Find("MainCamera").transform.Find("BlackScreen").gameObject.SetActive(true);
                            }, null);
                            while (!(BlackScreen.onlyBlackScreen == BlackScreen.OnlyBlackScreen.none)) ;
                        }                        
                    }
                    while (!(BlackScreen.onlyBlackScreen == BlackScreen.OnlyBlackScreen.none)) ;
                    MainMenu.startLock = false;//主菜单开始执行任务
                }
                
            }); t.Start();
            Thread keyCheck = new(() =>
            {
                while (runState == RunState.start)
                {
                    bool wait = false;
                    ThreadDelegate.QueueOnMainThread((param) =>
                    {
                        if (GameKey.OkKeyClick())
                            runState = RunState.skip;
                        wait = true;
                    }, null);
                    while (!wait) { Thread.Sleep(50); }
                }
            }); keyCheck.Start();
        }
    }

    /// <summary>
    /// 注意，该方法必须在子线程内执行！
    /// </summary>
    /// <param name="isShows">淡出或淡入显示图片</param>
    void ImgVisibility(bool isShows)
    {
            Color color=new();
            float prog=0;

            //设置透明度
            void SetA(float value)
            {
                {
                    bool wait = false;
                    ThreadDelegate.QueueOnMainThread((param) =>
                    {
                        color.a=value;
                        obj_img.color=color;
                        wait = true;
                    }, null);
                    while (!wait) ;
                }
            }
            
            {//设置变量初始值
                bool wait = false;
                ThreadDelegate.QueueOnMainThread((param) =>
                {
                    prog = obj_img.color.a;
                    color = obj_img.color;
                    wait = true;
                }, null);
                while (!wait) ;
            }
            while (true)
            {
                if (isShows)                
                    prog += 0.1f;                
                else
                    prog -= 0.1f;

            if (prog >= 1)
            {
                prog = 1f;
                SetA(prog);
                break;
            }else if(prog <= 0)
            {
                prog = 0f;
                SetA(prog);
                break;
            }
            SetA(prog);
                Thread.Sleep(64); 
            }
    }
}
