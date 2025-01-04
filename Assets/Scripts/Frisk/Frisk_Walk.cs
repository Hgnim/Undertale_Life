using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Resources;
using System.Threading;
using UnityEngine;
using UnityEngine.Playables;

public class Frisk_Walk : MonoBehaviour
{
    public SpriteRenderer sr;
    public Sprite[] walkImg_u;
    public Sprite[] walkImg_d;
    public Sprite[] walkImg_l;
    public Sprite[] walkImg_r;

    const float defMoveSpeed = 0.7f; static float moveSpeed = defMoveSpeed;
    int imgId=0;//用于播放动画的图片ID

    Thread walkImgThread;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        walkImgThread = new(WalkImgThread);
        walkImgThread.Start();
    }
    void WalkImgThread()
    {
        bool imgLock = false;//如果人物停止，立刻开启动画锁，让人物停止播放走路动画
        Thread stopImgThread = new(() =>
        {
            while (walkImgThread.IsAlive)
            {
                if (nowImgId == 'n' && !imgLock)
                {
                    imgLock = true;
                    imgId = -1;
                    switch (lastImgId)
                    {
                        case 'd':
                            ThreadDelegate.QueueOnMainThread((param) =>
                            {
                                sr.sprite = walkImg_d[0];
                            }, null);
                            break;
                        case 'u':
                            ThreadDelegate.QueueOnMainThread((param) =>
                            {
                                sr.sprite = walkImg_u[0];
                            }, null);
                            break;
                        case 'l':
                            ThreadDelegate.QueueOnMainThread((param) =>
                            {
                                sr.sprite = walkImg_l[0];
                            }, null);
                            break;
                        case 'r':
                            ThreadDelegate.QueueOnMainThread((param) =>
                            {
                                sr.sprite = walkImg_r[0];
                            }, null);
                            break;
                    }
                }
                else if(nowImgId!='n')
                {
                    imgLock = false;
                }
            }
        });
        stopImgThread.Start();
        void WalkGo()
        {
            imgId++;
        }
        {
            bool waitLock=false;//等待委托执行，保证同步
            while (true)
            {
                switch (nowImgId)
                {
                    case 'u':
                        waitLock = true;
                        ThreadDelegate.QueueOnMainThread((param) =>
                        {
                            if (lastImgId != 'u')
                                imgId = 1;
                            if (!(imgId < walkImg_u.Length) || imgId == -1)
                                imgId = 0;
                            if (!imgLock)//保险机制
                                sr.sprite = walkImg_u[imgId];
                            WalkGo();
                            waitLock = false;
                        }, null);
                        break;
                    case 'd':
                        waitLock = true;
                        ThreadDelegate.QueueOnMainThread((param) =>
                        {
                            if (lastImgId != 'd')
                                imgId = 1;
                            if (!(imgId < walkImg_d.Length) || imgId == -1)
                                imgId = 0;
                            if (!imgLock)
                                sr.sprite = walkImg_d[imgId];
                            WalkGo();
                            waitLock = false;
                        }, null);
                        break;
                    case 'l':
                        waitLock = true;
                        ThreadDelegate.QueueOnMainThread((param) =>
                        {
                            if (lastImgId != 'l')
                                imgId = 1;
                            if (!(imgId < walkImg_l.Length) || imgId == -1)
                                imgId = 0;
                            if (!imgLock)
                                sr.sprite = walkImg_l[imgId];
                            WalkGo();
                            waitLock = false;
                        }, null);
                        break;
                    case 'r':
                        waitLock = true;
                        ThreadDelegate.QueueOnMainThread((param) =>
                        {
                            if (lastImgId != 'r')
                                imgId = 1;
                            if (!(imgId < walkImg_r.Length) || imgId == -1)
                                imgId = 0;
                            if (!imgLock)
                                sr.sprite = walkImg_r[imgId];
                            WalkGo();
                            waitLock = false;
                        }, null);
                        break;
                    default:
                        if(waitLock)waitLock = false;
                        break;
                }
                Thread.Sleep(200);//动画速度
                while (waitLock) { }//锁住进度，保证线程同步
                while(imgLock) { }//暂挂线程，保证快速起步
            }
        }
    }
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        transform.Translate(Vector3.right * x * moveSpeed * Time.deltaTime, Space.World);//水平移动，x为正向右，x为负向左
        transform.Translate(Vector3.up * y * moveSpeed * Time.deltaTime, Space.World);//垂直移动，y为正向上，y为负向下
        if ((x != 0 || y != 0) && moveLock==false)
        {
            if(nowImgId!='n')
            lastImgId = nowImgId;
            if (y > 0 && Math.Abs(y)>Math.Abs(x))//向上移动
            {                
                    nowImgId = 'u';
            }
            else if (y < 0 && Math.Abs(y) > Math.Abs(x) )//向下移动
            {
                    nowImgId = 'd';
            }
            else if (x < 0 && Math.Abs(y) < Math.Abs(x))//向左移动
            {
                    nowImgId = 'l';
            }
            else if (x > 0 && Math.Abs(y) < Math.Abs(x))//向右移动
            {
                    nowImgId = 'r';
            }
        }
        else if (nowImgId != 'n')
        {
            nowImgId = 'n';            
        }
    }

  static  bool moveLock;
    /// <summary>
    /// 设置移动锁状态
    /// </summary>
    /// <param name="enable">是否锁住移动</param>
    public static void MoveLock(bool enable)
    {
        switch (enable)
        {
            case true:
                moveSpeed = 0; 
                moveLock = true;
                break;
            case false:
                moveSpeed = defMoveSpeed;
                moveLock = false;
                break;
        }
    }
    /// <summary>
    /// 获取当前移动锁的状态
    /// </summary>
    /// <returns></returns>
    public static bool GetMoveLock()
    {
        return moveLock;
    }

    static char nowImgId = 'n';//当前移动时的朝向，n(null) u d l r
    static char lastImgId = 'd';//停止移动后的朝向，u d l r
    /// <summary>
    /// 检查人物朝向是否符合目标。
    /// <br/>
    /// 当checkNow和checkLast都为true时，则用此判断方法：当当前移动朝向不为n的时候，则只判断当前移动的朝向，反之则以逻辑或来判断。
    /// </summary>
    /// <param name="target">需要检查符合的目标朝向。
    /// <br/>
    /// 可用的值有: u d l r</param>
    /// <param name="checkNow">是否检查当前移动时的朝向，默认true</param>
    /// <param name="checkLast">是否检查停止后的朝向，默认为true</param>
    /// <returns>检查成功返回true，否则返回false</returns>
    public static bool ImgFacingCheck(char target, bool checkNow = true, bool checkLast = true)
    {
        if (checkNow && checkLast)
        {
            if (nowImgId == 'n')
            {
                if (nowImgId == target || lastImgId == target)
                    return true;
            }
            else
            {
                if(nowImgId==target)
                    return true;
            }            
        }
        else if (!checkNow && checkLast)
        {
            if (lastImgId == target)
                return true;
        }
        else if (checkNow && !checkLast)
        {
            if (nowImgId == target)
                return true;
        }
        return false;
    }

    /* Vector2 lockPoint= new();
     // 碰撞检测函数,进入的时候执行
     void OnTriggerEnter2D(Collider2D collider)
     {
         //moveSpeed = 0;
        lockPoint= sr.transform.position;
     }

     //
     // 停留的时候执行
     void OnTriggerStay2D(Collider2D collider)
     {
         Debug.Log("一直停留在碰撞状态...");
         sr.transform.position= lockPoint;
     }

     // 退出的时候执行
     void OnTriggerExit2D(Collider2D collider)
     {
        // moveSpeed = defMoveSpeed;
        moveLock = new bool[4];
     }*/
    /*bool[] moveLock = new bool[4];//碰撞箱判断以及移动锁定编号顺序 u d l r
    private void OnCollisionEnter2D(Collision2D coll)
    {

        if (coll.contacts[0].normal.y == -1)//从上方碰撞
        {
            moveLock[0]= true;
        }
        if (coll.contacts[0].normal.y == 1)//从下方碰撞
        {
            moveLock[1] = true;
        }
        if (coll.contacts[0].normal.x == -1)//左边碰撞
        {
            moveLock[2]=true;
        }
        if (coll.contacts[0].normal.x == 1)//右边碰撞
        {
            moveLock[3] = true;
        }

    }*/
}
