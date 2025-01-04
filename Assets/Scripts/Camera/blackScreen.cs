using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.SceneManagement;

public class BlackScreen : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;//������Ļ��һ��ȫ��ͼƬ����ѡ�����Camera���棬���������������ƶ��ľͺܷ���
    public AnimationCurve blackCurve; //��Inspector�ϵ����Լ�ϲ��������
    public AnimationCurve backCurve;
    [Range(0.5f, 2f)] public float speed = 1f; //���ƽ��뽥�����ٶ�

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }
    /// <summary>
    /// �������������������������������жϡ�<br/>����������ǰ���øñ�������<br/>��(none)������(black)������(back)
    /// </summary>
   public  enum OnlyBlackScreen { none,black,back}
    /// <summary>
    /// �������������������������������жϡ�<br/>����������ǰ���øñ�������<br/>
    /// �����ж��Ƿ��������������������ķ�����Ҳ���������жϷ����Ƿ�ִ����ɣ�����ִ����ɺ�ñ����ᱻ����Ϊnone
    /// </summary>
    public static OnlyBlackScreen onlyBlackScreen=OnlyBlackScreen.none;
    /// <summary>
    /// �����Զ����ź��������ֶ���������
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


    public static bool blackDone;//���ڷ��ز���ֵ����ʾ�Ƿ�����ȫ����
    /// <summary>
    /// ���ڷ��ز���ֵ����ʾ�Ƿ������������Ҳ����ִ�����������ο���ִ�е��µĴ���Ϊfalse��ʱ�򲻿��ظ�ִ��<br/>
    /// �������ж��Ƿ�ִ�����
    /// </summary>
    public static bool backDone=true;
    public static string checkScene = null;//��鳡���Ƿ��л����
    /// <summary>
    /// ��ʾ�����Ƿ��Ѿ����������ڵȴ�ִ�С�<br/>
    /// ע��: �ñ���ֻ��Ϊ�ֲ������������ṩ���ⲿ��ʹ��
    /// </summary>
    bool ColorChangeRunDone;
    Color tmpColor; //���ڴ�����ɫ�ı���
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
                    checkScene = null;//���е�����������Ѿ�������ɣ�׼������ȡ������
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

    //ͨ������ͼƬ��͸����ʵ�ֽ��뽥��
    void SetColorAlpha(float a)
    {
        tmpColor.a = a;
        spriteRenderer.color = tmpColor;
    }

}
