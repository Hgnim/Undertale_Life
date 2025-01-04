using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.SceneManagement;

public class BlackScreen : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;//覆盖屏幕的一张全黑图片，我选择挂在Camera下面，这样做如果相机是移动的就很方便
    public AnimationCurve blackCurve; //在Inspector上调整自己喜欢的曲线
    public AnimationCurve backCurve;
    [Range(0.5f, 2f)] public float speed = 1f; //控制渐入渐出的速度

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }
    /// <summary>
    /// 仅黑屏或亮屏方法启动，不做其它判断。<br/>在启动对象前设置该变量即可<br/>无(none)、黑屏(black)、亮屏(back)
    /// </summary>
   public  enum OnlyBlackScreen { none,black,back}
    /// <summary>
    /// 仅黑屏或亮屏方法启动，不做其它判断。<br/>在启动对象前设置该变量即可<br/>
    /// 用于判断是否启动单独黑屏或亮屏的方法。也可以用于判断方法是否执行完成，方法执行完成后该变量会被设置为none
    /// </summary>
    public static OnlyBlackScreen onlyBlackScreen=OnlyBlackScreen.none;
    /// <summary>
    /// 开启自动播放黑屏，或手动仅黑亮屏
    /// </summary>
    private void OnEnable()
    {
        ColorChangeRunDone = false;
        if (backDone && !(onlyBlackScreen != OnlyBlackScreen.none))
        {
            onlyBlackScreenWaitLock = true;
            blackDone = false;
            backDone = false;
            StartAutoChange();
        }
        else
            onlyBlackScreenWaitLock=false;
    }
    bool onlyBlackScreenWaitLock=false;
    private void Update()
    {
        if (onlyBlackScreen != OnlyBlackScreen.none && !onlyBlackScreenWaitLock)
        {
            onlyBlackScreenWaitLock=true;
            ColorChangeRunDone = false;
            StartOnlyChange(onlyBlackScreen);
        }
    }


    public static bool blackDone;//用于返回布尔值，表示是否已完全黑屏
    /// <summary>
    /// 用于返回布尔值，表示是否已完成亮屏。也代表执行锁，避免多次快速执行导致的错误。为false的时候不可重复执行<br/>
    /// 可用于判断是否执行完成
    /// </summary>
    public static bool backDone=true;
    public static string checkScene = null;//检查场景是否切换完成
    /// <summary>
    /// 表示更改是否已经结束，用于等待执行。<br/>
    /// 注意: 该变量只能为局部变量，不可提供给外部类使用
    /// </summary>
    bool ColorChangeRunDone;
    Color tmpColor; //用于传递颜色的变量
    AnimationCurve ac;
    void StartAutoChange()
    {
        Thread t = new(() =>
        {
            ac = blackCurve;
            goto blackRun;
backRun:;
            ac = backCurve;
blackRun:;
            {
                bool wait=false;
                ColorChangeRunDone = false;
                ThreadDelegate.QueueOnMainThread((param) =>
                {
                    StartCoroutine(ColorChange());
                    wait = true;
                }, null);
                while (!wait) ;
                while(!ColorChangeRunDone) ;
            }
            if (ac == blackCurve)
            {
                blackDone = true;
                if (checkScene != null)
                {
                    bool SceneCheck()
                    {
                        bool tmp=false;
                        bool wait = false;
                        ThreadDelegate.QueueOnMainThread((param) =>
                        {
                           tmp= SceneManager.GetActiveScene().name == checkScene;
                            wait= true;
                        }, null);
                        while(!wait) ;
                        return tmp;
                    }
                    while (!(SceneCheck())) Thread.Sleep(20);
                    checkScene = null;//运行到这里，代表场景已经加载完成，准备调用取消黑屏
                    {
                        bool wait=false;
                        ThreadDelegate.QueueOnMainThread((param) =>
                        {
                            Frisk_Walk.MoveLock(false);
                            wait= true;
                        }, null);
                        while(!wait) ;
                    }
                    KeyState.cMenuOpenLock = false;
                    KeyState.UseObjectInHandsLock = false;
                    goto backRun;
                }
                else
                {
                    goto backRun;
                }
            }
            else
            {
                blackDone = false;
                {
                    bool wait = false;
                    ThreadDelegate.QueueOnMainThread((param) =>
                    {
                        gameObject.SetActive(false);
                        wait = true;
                    }, null);
                    while (!wait) ;
                }
                backDone = true;
            }
        });t.Start();
    }
    void StartOnlyChange(OnlyBlackScreen obs)
    {
        Thread t = new(() =>
        {
            switch (obs)
        {
            case OnlyBlackScreen.black:
                ac=blackCurve; break;
            case OnlyBlackScreen.back:
                ac=backCurve; break;
        }
            ThreadDelegate.QueueOnMainThread((param) =>
            {
                StartCoroutine(ColorChange());
            }, null);
            while (!ColorChangeRunDone) ;            
            if (obs == OnlyBlackScreen.back)
            {
                ThreadDelegate.QueueOnMainThread((param) =>
                {
                    gameObject.SetActive(false);
                }, null);
            }
            onlyBlackScreen = OnlyBlackScreen.none;
            onlyBlackScreenWaitLock = false;
        });t.Start();
    }
    public IEnumerator ColorChange()
    {
        float timer = 0f;
        tmpColor = spriteRenderer.color;
        do
        {
            timer += Time.deltaTime;
            SetColorAlpha(ac.Evaluate(timer * speed));
            yield return null;
        } while (tmpColor.a > 0 && tmpColor.a<1);
        //timer = 0f;        
        ColorChangeRunDone = true;
    }

    //通过调整图片的透明度实现渐入渐出
    void SetColorAlpha(float a)
    {
        tmpColor.a = a;
        spriteRenderer.color = tmpColor;
    }

}
