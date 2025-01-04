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
    int imgId=0;//���ڲ��Ŷ�����ͼƬID

    Thread walkImgThread;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        walkImgThread = new(WalkImgThread);
        walkImgThread.Start();
    }
    void WalkImgThread()
    {
        bool imgLock = false;//�������ֹͣ�����̿�����������������ֹͣ������·����
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
            bool waitLock=false;//�ȴ�ί��ִ�У���֤ͬ��
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
                            if (!imgLock)//���ջ���
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
                Thread.Sleep(200);//�����ٶ�
                while (waitLock) { }//��ס���ȣ���֤�߳�ͬ��
                while(imgLock) { }//�ݹ��̣߳���֤������
            }
        }
    }
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        transform.Translate(Vector3.right * x * moveSpeed * Time.deltaTime, Space.World);//ˮƽ�ƶ���xΪ�����ң�xΪ������
        transform.Translate(Vector3.up * y * moveSpeed * Time.deltaTime, Space.World);//��ֱ�ƶ���yΪ�����ϣ�yΪ������
        if ((x != 0 || y != 0) && moveLock==false)
        {
            if(nowImgId!='n')
            lastImgId = nowImgId;
            if (y > 0 && Math.Abs(y)>Math.Abs(x))//�����ƶ�
            {                
                    nowImgId = 'u';
            }
            else if (y < 0 && Math.Abs(y) > Math.Abs(x) )//�����ƶ�
            {
                    nowImgId = 'd';
            }
            else if (x < 0 && Math.Abs(y) < Math.Abs(x))//�����ƶ�
            {
                    nowImgId = 'l';
            }
            else if (x > 0 && Math.Abs(y) < Math.Abs(x))//�����ƶ�
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
    /// �����ƶ���״̬
    /// </summary>
    /// <param name="enable">�Ƿ���ס�ƶ�</param>
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
    /// ��ȡ��ǰ�ƶ�����״̬
    /// </summary>
    /// <returns></returns>
    public static bool GetMoveLock()
    {
        return moveLock;
    }

    static char nowImgId = 'n';//��ǰ�ƶ�ʱ�ĳ���n(null) u d l r
    static char lastImgId = 'd';//ֹͣ�ƶ���ĳ���u d l r
    /// <summary>
    /// ������ﳯ���Ƿ����Ŀ�ꡣ
    /// <br/>
    /// ��checkNow��checkLast��Ϊtrueʱ�����ô��жϷ���������ǰ�ƶ�����Ϊn��ʱ����ֻ�жϵ�ǰ�ƶ��ĳ��򣬷�֮�����߼������жϡ�
    /// </summary>
    /// <param name="target">��Ҫ�����ϵ�Ŀ�곯��
    /// <br/>
    /// ���õ�ֵ��: u d l r</param>
    /// <param name="checkNow">�Ƿ��鵱ǰ�ƶ�ʱ�ĳ���Ĭ��true</param>
    /// <param name="checkLast">�Ƿ���ֹͣ��ĳ���Ĭ��Ϊtrue</param>
    /// <returns>���ɹ�����true�����򷵻�false</returns>
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
     // ��ײ��⺯��,�����ʱ��ִ��
     void OnTriggerEnter2D(Collider2D collider)
     {
         //moveSpeed = 0;
        lockPoint= sr.transform.position;
     }

     //
     // ͣ����ʱ��ִ��
     void OnTriggerStay2D(Collider2D collider)
     {
         Debug.Log("һֱͣ������ײ״̬...");
         sr.transform.position= lockPoint;
     }

     // �˳���ʱ��ִ��
     void OnTriggerExit2D(Collider2D collider)
     {
        // moveSpeed = defMoveSpeed;
        moveLock = new bool[4];
     }*/
    /*bool[] moveLock = new bool[4];//��ײ���ж��Լ��ƶ��������˳�� u d l r
    private void OnCollisionEnter2D(Collision2D coll)
    {

        if (coll.contacts[0].normal.y == -1)//���Ϸ���ײ
        {
            moveLock[0]= true;
        }
        if (coll.contacts[0].normal.y == 1)//���·���ײ
        {
            moveLock[1] = true;
        }
        if (coll.contacts[0].normal.x == -1)//�����ײ
        {
            moveLock[2]=true;
        }
        if (coll.contacts[0].normal.x == 1)//�ұ���ײ
        {
            moveLock[3] = true;
        }

    }*/
}
