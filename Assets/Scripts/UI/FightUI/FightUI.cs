using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static KeyManager;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using System;
using UnityEngine.UI;
using static CoreData.CData.People;
using static CoreData.ToolClass;
using UnityEditor.Rendering;
using static CoreData.CData;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine.Events;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.Localization.PropertyVariants.TrackedProperties;

public class FightUI : MonoBehaviour
{
#pragma warning disable CS8632 // ֻ���� "#nullable" ע���������ڵĴ�����ʹ�ÿ�Ϊ null ���������͵�ע�͡�
    /// <summary>
    /// ս�������ݼ�����
    /// </summary>
    static class FightBoxData
    {
        /// <summary>
        /// Ԥ��ս����λ��1��ƫ�����
        /// </summary>
        public const string loc1FightBoxLocSize = "-0.0252,0.2,0.74,0.64";
        /// <summary>
        /// Ԥ��ս����λ��1��ƫ���Լ�
        /// </summary>
        public const string loc2FightBoxLocSize = "-0.0252,-0.32,0.74,0.64";
        /// <summary>
        /// ս����δ�ı�ʱ�ĳ�ʼλ��
        /// </summary>
        public const string normalFightBoxLocSize = "-0.0252,-0.427,2,0.46";
        /// <summary>
        /// ����ս�����������ȡ��������
        /// </summary>
        /// <param name="dataStr">��','��ɵ�������<br/>���Ϊnull��ʹ��Ĭ�ϵ����������</param>
        /// <returns></returns>
        internal static Vector2 GetLoc(string? dataStr)
        {
            string[] cache;
            if (dataStr != null)
                cache=dataStr.Split(',');
            else
               cache= normalFightBoxLocSize.Split(',');
            return new Vector2(float.Parse( cache[0]), float.Parse(cache[1]));
        }
        /// <summary>
        /// ����ս�����������ȡ��С����
        /// </summary>
        /// <param name="dataStr">��','��ɵ�������<br/>���Ϊnull��ʹ��Ĭ�ϵ����������</param>
        /// <returns></returns>
        internal static Vector2 GetSize(string? dataStr)
        {
            string[] cache;
            if(dataStr != null)
                cache=dataStr.Split(',');
            else 
                cache= normalFightBoxLocSize.Split(',');
            return new Vector2(float.Parse(cache[2]),float.Parse(cache[3]));
        }
    }
    #region HeartLevelName.attackģ������
    /// <summary>
    /// ����ս������������
    /// </summary>
    internal static class OpponentAttackData
    {
        private static readonly GameObject loadObj = Resources.Load<GameObject>("Prefabs/UI/FightUI/FightUI_FightBox_OpponentWeapon");
        /// <summary>
        /// ����������
        /// </summary>
        private static readonly UnityEngine.Transform parentObj = GameObject.Find("MainCamera/FightUI/FightUI_FightBox").transform;
        /// <summary>
        /// ���ض���ͼƬ����
        /// </summary>
        /// <param name="weaponID">�����洢����������</param>
        /// <param name="imgName">Ҫ���ص�ͼƬID</param>
        private static void LoadImg(int weaponID,string imgName)
        {
            SpriteRenderer sr =
            weapon[weaponID].GetComponent<SpriteRenderer>();
            sr.drawMode = SpriteDrawMode.Simple;//���л����򵥻���ģʽ���ö����С��ͼƬ�仯
            sr.sprite = Resources.Load<Sprite>("Images/UI/FightUI/weapon/" + imgName);
            sr.drawMode = SpriteDrawMode.Sliced;//�л�����Ƭ����ģʽ���ö������ײ��С������С�仯
        }
        /// <summary>
        /// ��������洢����
        /// </summary>
        internal static List<GameObject> weapon = new();
        /// <summary>
        /// �ƶ���������ID��<br/>
        /// {����ID,����ID}
        /// </summary>
        internal static List<int[]> weaponMoveID = new();
        /// <summary>
        /// �����߳�
        /// </summary>
        static Thread controlThread;
        /// <summary>
        /// �����߳��Ƿ���ֹ
        /// </summary>
        static bool controlThread_stop;
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="AttackId">������ʽID</param>
        internal static void Attack_Load(int AttackId)
        {
            if (controlThread == null || controlThread.IsAlive == false) 
            {
                controlThread_stop = false;

                if (opponentID.Length == 1)
                {
                    switch (opponentID[0])
                    {
                        case ID.debugDummy:
                            switch (AttackId)
                            {
                                case 0://��0��ս������
                                    controlThread = new(() =>
                                    {
                                        void CreatNew()
                                        {
                                            weapon.Add(GameObject.Instantiate(loadObj));
                                            weaponMoveID.Add(new int[] { 0, 0 });
                                            weapon[^1].transform.parent = parentObj;
                                            weapon[^1].name = (weapon.Count - 1).ToString() + "_" + weapon[^1].name;
                                            LoadImg(weapon.Count - 1, "DebugDummy_weapon1");
                                        }
                                        int loopID = 0;
                                        SleepTimer sleepTimer = new();
                                        while (true)
                                        {
                                            {
                                                bool wait = false;
                                                ThreadDelegate.QueueOnMainThread((param) =>
                                                {
                                                    switch (loopID)
                                                    {
                                                        case 0:
                                                            CreatNew();
                                                            weapon[^1]/*[weapon.Count - 1]*/.transform.localPosition = new(-0.24f, 0.543f);
                                                            weapon[^1].SetActive(true);
                                                            CreatNew();
                                                            weapon[^1].transform.localPosition = new(0.24f, 0.543f);
                                                            weapon[^1].SetActive(true);
                                                            loopID = 1;
                                                            break;
                                                        case 1:
                                                            CreatNew();
                                                            weapon[^1].transform.localPosition = new(0, 0.543f);
                                                            weapon[^1].SetActive(true);
                                                            loopID = 0;
                                                            break;
                                                    }
                                                    wait= true;
                                                }, null);
                                                sleepTimer = new();
                                                while (!wait) ;
                                            }
                                            sleepTimer.Start(500);
                                            while (true)
                                            {
                                                if (sleepTimer.IsTimerOver)
                                                    break;
                                                else if (controlThread_stop)
                                                    goto over;
                                                else
                                                    Thread.Sleep(4);
                                            }
                                        }
over:;
                                    });controlThread.Start();
                                    break;
                            }
                            break;
                    }
                } 
            }
        }
        internal static void Attack_Stop()
        {
            if (controlThread.IsAlive == true)
                controlThread_stop = true;

            foreach (GameObject w in weapon)
            {
                if (w != null)
                    GameObject.Destroy(w);
            }
            weapon.Clear();
            weaponMoveID.Clear();
        }
    }
    void OpponentAttackOver()
    {
        OpponentAttackData.Attack_Stop();

        heartNowLevel = HeartLevelName.rootButton;
        HeartUIPosChange(heartRootPos);
        FightBoxLocSizeChange(FightBoxData.normalFightBoxLocSize);
        Thread t = new(() =>
        {
            while (fightboxLocSizeChangeWaitLock)
                Thread.Sleep(1);
            ThreadDelegate.QueueOnMainThread((param) => {
                MsgBox.SetActive(true);
                FightBox.SetActive(false);
            }, null);
        });t.Start();
    }
    /// <summary>
    /// ս������������
    /// </summary>
    static class FightAttackData
    {
        internal static List<GameObject> frisk_weapon = new();
        /// <summary>
        /// �����Ƿ�����
        /// </summary>
        internal static List<bool> frisk_weapon_Use=new();
        internal static float frisk_weapon_Speed = 2f;
        /// <summary>
        /// ����frisk������
        /// </summary>
        internal static void FriskWeapon_Load()
        {
            if (!GetFriskWeaponActive())
            {
                for (int i = 0; i < 4; i++)
                {
                    frisk_weapon_Use.Add(false);
                    frisk_weapon.Add(GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/FightUI/FightUI_FightBox_FirskWeapon")));
                    frisk_weapon[i].transform.parent = GameObject.Find("MainCamera/FightUI/FightUI_FightBox").transform;
                    switch (i)
                    {
                        case 0:
                            frisk_weapon[i].transform.localPosition = new Vector2(-0.25f, -0.5f);
                            frisk_weapon[i].transform.localRotation = Quaternion.Euler(0, 0, -25);
                            break;
                        case 1:
                            frisk_weapon[i].transform.localPosition = new Vector2(0, -0.5f); break;
                        case 2:
                            frisk_weapon[i].transform.localPosition = new Vector2(0.25f, -0.5f);
                            frisk_weapon[i].transform.localRotation = Quaternion.Euler(0, 0, 25);
                            break;
                        case 3:
                            frisk_weapon[i].transform.localPosition = new Vector2(0, 0.5f);
                            frisk_weapon[i].transform.localRotation = Quaternion.Euler(0, 0, 180);
                            break;
                    }                    
                }
                IgnoreCollisionControl(GameObject.FindGameObjectsWithTag("Player"), frisk_weapon.ToArray());
            }
        }
        /// <summary>
        /// ��ȡ��ǰfrisk��������Ĵ��״̬
        /// </summary>
        /// <returns></returns>
        internal static bool GetFriskWeaponActive()
        {
            if (frisk_weapon.Count != 0 || frisk_weapon_Use.Count != 0)
                return true;
            else
                return false;
        }
        /// <summary>
        /// ж��frisk������
        /// </summary>
        internal static void FriskWeapon_Destory()
        {
            foreach(GameObject go in frisk_weapon)
            {
                GameObject.Destroy(go);
            }
            frisk_weapon.Clear();
            frisk_weapon_Use.Clear();
        }
    }    
    /// <summary>
    /// ���Ӧ�ķ��������ĵȴ���
    /// </summary>
    bool fightboxLocSizeChangeWaitLock = false;
    /// <summary>
    /// �ṩ��attack��ļ�ʱ����
    /// </summary>
    SleepTimer attackTimer;
    /// <summary>
    /// ��ҹ�����������
    /// </summary>
    void PlayerAttackOver()
    {        
            FightAttackData.FriskWeapon_Destory();
            OpponentHeart.SetActive(false);
        heartNowLevel = HeartLevelName.freeMove_Load;
    }
    #endregion
    /// <summary>
    /// ������ʽ����ս�����С��λ��<br/>�÷��������߳������еģ���������ʱ����ȴ����������
    /// </summary>
    /// <param name="dataStr">Ŀ��λ�ô�С������</param>
    /// <param name="speed">�ٶ�</param>
    void FightBoxLocSizeChange(string? dataStr,float speed=0.05f)
    {
        fightboxLocSizeChangeWaitLock=true;
        Thread t = new(() =>
        {
            SpriteRenderer fb_sr=new();
            EdgeCollider2D fb_ec=new();
            {
                bool wait = false;
                ThreadDelegate.QueueOnMainThread((param) =>
                {
                    fb_sr = FightBox.GetComponent<SpriteRenderer>();
                    fb_ec = FightBox.GetComponent<EdgeCollider2D>();
                    wait = true;
                }, null);
                while (!wait) ;
            }
            ///Ŀ��λ��
            Vector2 targetLoc=FightBoxData.GetLoc(dataStr);
            ///Ŀ���С
            Vector2 targetSize=FightBoxData.GetSize(dataStr);
            ///���ڸ��Ĳ���ֵ����Ϊ��������Բ�֧��ֱ�Ӹ���ĳһ��ֵ
            Vector2 nowLoc=new();
            Vector2 nowSize=new();
            {
                bool wait = false;
                ThreadDelegate.QueueOnMainThread((param) =>
                {
                    nowLoc = FightBox.transform.localPosition;
                nowSize = fb_sr.size;
                    wait= true;
                }, null);
                while (!wait) ;
            }
            ///cp=compute��ָ��ÿ����ֵ�ǵ������ǵݼ����ǲ��䣬�ֱ���1,-1,0����
            int cpLocX, cpLocY,cpSizeX,cpSizeY;
            ///�ж��Ƿ���ɼ���
            bool doneLocX,doneLocY,doneSizeX,doneSizeY;
            {//�жϼ��㷽����Ϊ�������ǵݼ����ǲ���
                doneLocX = false;
                doneLocY = false;
                doneSizeX = false;
                doneSizeY = false;
                if (nowLoc.x < targetLoc.x) cpLocX = 1;
                else if (nowLoc.x > targetLoc.x) cpLocX = -1;
                else
                { cpLocX = 0; doneLocX = true; }
                if (nowLoc.y < targetLoc.y) cpLocY = 1;
                else if (nowLoc.y > targetLoc.y) cpLocY = -1;
                else 
                { cpLocY = 0; doneLocY = true; }
                if (nowSize.x < targetSize.x) cpSizeX = 1;
                else if (nowSize.x > targetSize.x) cpSizeX = -1;
                else 
                { cpSizeX = 0;doneSizeX = true; }
                if (nowSize.y < targetSize.y) cpSizeY = 1;
                else if (nowSize.y > targetSize.y) cpSizeY = -1;
                else 
                { cpSizeY = 0;doneSizeY = true; }              
            }
            ///���ĵ�ǰ�������,nowLoc
            void ChangeLoc(int cpx,int cpy)
            {
                void OverX()
                {
                    nowLoc.x = targetLoc.x;
                    doneLocX = true;
                }
                void OverY()
                {
                    nowLoc.y = targetLoc.y;
                    doneLocY = true;
                }
                if (!doneLocX)
                {
                    switch (cpx)
                    {
                        case 1:
                            nowLoc.x += speed;
                            if (nowLoc.x >= targetLoc.x)
                                OverX();
                            break;
                        case -1:
                            nowLoc.x -= speed;
                            if (nowLoc.x <= targetLoc.x)
                                OverX();
                            break;
                    }
                }
                if (!doneLocY)
                {
                    switch (cpy)
                    {
                        case 1:
                            nowLoc.y += speed;
                            if (nowLoc.y >= targetLoc.y)
                                OverY();
                            break;
                        case -1:
                            nowLoc.y -= speed;
                            if (nowLoc.y <= targetLoc.y)
                                OverY();
                            break;
                    }
                }
            }
            ///���ĵ�ǰ��С����,nowSize
            void ChangeSize(int cpx, int cpy)
            {
                void OverX()
                {
                    nowSize.x = targetSize.x;
                    doneSizeX = true;
                }
                void OverY()
                {
                    nowSize.y = targetSize.y;
                    doneSizeY = true;
                }
                if (!doneSizeX)
                {
                    switch (cpx)
                    {
                        case 1:
                            nowSize.x += speed;
                            if (nowSize.x >= targetSize.x)
                                OverX();
                            break;
                        case -1:
                            nowSize.x -= speed;
                            if (nowSize.x <= targetSize.x)
                                OverX();
                            break;
                    }
                }
                if (!doneSizeY)
                {
                    switch (cpy)
                    {
                        case 1:
                            nowSize.y += speed;
                            if (nowSize.y >= targetSize.y)
                                OverY();
                            break;
                        case -1:
                            nowSize.y -= speed;
                            if (nowSize.y <= targetSize.y)
                                OverY();
                            break;
                    }
                }
            }
            ///���ݳ�����С������ײ��Ե��С
            void ChangeEdgeColliderPoints()
            {
                float[] cache=new float[2] {(nowSize.x/2)-0.01f,(nowSize.y/2)-0.01f};
                fb_ec.points = new Vector2[]
                {
                   new(-cache[0],cache[1]),
                   new(cache[0],cache[1]),
                   new(cache[0],-cache[1]),
                   new(-cache[0],-cache[1]),
                   new(-cache[0],cache[1]),
                };
            }
            while (!(doneLocX && doneLocY && doneSizeX && doneSizeY))
            {
                ChangeLoc(cpLocX, cpLocY);
                ChangeSize(cpSizeX, cpSizeY);
                {
                    bool wait = false;
                    ThreadDelegate.QueueOnMainThread((param) =>
                    {
                        FightBox.transform.localPosition = nowLoc;
                        fb_sr.size=nowSize;
                        ChangeEdgeColliderPoints();
                        wait = true;
                    }, null);
                    Thread.Sleep(40);
                    while (!wait) ;
                }
            }
            fightboxLocSizeChangeWaitLock = false;
        });t.Start();
    }
#pragma warning restore CS8632 // ֻ���� "#nullable" ע���������ڵĴ�����ʹ�ÿ�Ϊ null ���������͵�ע�͡�
    /// <summary>
    /// ��ͼ���ڸ�����ť��λ������
    /// </summary>
    private static class HeartPos
    {

        static class MainBt
        {
            public const float y = -0.851f;
            public const float fight_x = -0.9661f;
            public const float act_x = -0.4291f;
            public const float item_x = 0.0799f;
            public const float mercy_x = 0.6096f;
        }
        /// <summary>
        /// �ӳ���/ֻ�����ݿ��е��ö�Ӧλ�õ���������<br/>
        /// ֻ��ΪHeartLevelName.rootButton���4����ť
        /// </summary>
        /// <param name="hps">����ָ�������Ӧ������</param>
        /// <returns></returns>
        internal static Vector2 GetMainBtPos(HeartPosName hps)
        {
            switch (hps)
            {
                case HeartPosName.fightBt:
                    return new Vector2(MainBt.fight_x, MainBt.y);
                case HeartPosName.actBt:
                    return new Vector2(MainBt.act_x, MainBt.y);
                case HeartPosName.itemBt:
                    return new Vector2(MainBt.item_x, MainBt.y);
                case HeartPosName.mercyBt:
                    return new Vector2(MainBt.mercy_x, MainBt.y);
                default:
                    throw (new Exception("���������ܷ����Ĵ���"));
                    //return Vector2.zero;
            }
        }
        static class TextUI
        {
            public readonly static float[] x = new float[2] 
            { 
                -0.961f,0.042f 
            };
            public readonly static float[] y = new float[3]
            {
                -0.287f,-0.436f,-0.577f
            };
        }
        /// <summary>
        /// �ӳ���/ֻ�����ݿ��е��ö�Ӧλ�õ���������
        /// </summary>
        /// <param name="id">Ŀ������������<br/>����: FightUI_MsgBox_TextUI_obj1�����ı��1</param>
        /// <returns></returns>
        internal static Vector2 GetTextUIBtPos(int id)
        {
            if (id >= 3)
                return new Vector2(TextUI.x[0], TextUI.y[id - 1]);
            else
                return new Vector2(TextUI.x[1], TextUI.y[id-3-1]);
        }
        /// <summary>
        /// �ӳ���/ֻ�����ݿ��е��ö�Ӧλ�õ���������<br/>
        /// ֻ�ܽ�ö�ٲ�������Ϊǰ׺ΪtextuiObj�ģ����򽫻���ִ���
        /// </summary>
        /// <param name="hps">Ŀ������ö��ID</param>
        /// <returns></returns>
        internal static Vector2 GetTextUIBtPos(HeartPosName hps)
        {
            return GetTextUIBtPos((int)hps-HeartPosName.textuiObj1+1);//���ݼ���ó�������
        }
    }
    /// <summary>
    /// textui�����࣬ʹ��ǰ�����ʼ��
    /// </summary>
    private static class TextUI_Ctrl
    {
        static Text[] MsgBox_TextUI_Objs;
        /// <summary>
        /// �ֱ���������ı����󣬱�ʾ���Ƿ��ܱ�ѡ��
        /// </summary>
        internal static bool[] objSelect = new bool[6];
        /// <summary>
        /// ���������ı����������
        /// </summary>
        internal static string[] objStr= new string[6];
        /// <summary>
        /// ����и���ҳ�棬������ҳ��洢�ڸñ���<br/>ÿ�����������зֱ���������ı����󣬱�ʾ���Ƿ��ܱ�ѡ��
        /// </summary>
        internal static List<bool[]> morePage_objSelect = new();
        /// <summary>
        /// ����и���ҳ�棬������ҳ��洢�ڸñ���<br/>ÿ���ַ��������а��������ı����������
        /// </summary>
        internal static List<string[]> morePage_objStr = new();
        /// <summary>
        /// ����UI����
        /// </summary>
        internal static void DrawUI()
        {
            int currentPage = Page.CurrentPage;
            switch (currentPage)
            {
                case -1:
                    break;
                case 0:
                    for (int i = 0; i < 6; i++)
                        MsgBox_TextUI_Objs[i].text = objStr[i];
                    break;
                default:
                    for (int i = 0; i < 6; i++)
                        MsgBox_TextUI_Objs[i].text = morePage_objStr[currentPage - 1][i];
                    break;
            }
        }
        /// <summary>
        /// �ж��ı������Ƿ��ܱ�ѡ��
        /// </summary>
        /// <param name="objId">�ı�����ID����0��ͷ</param>
        /// <returns></returns>
        internal static bool WhetherSelectObj(int objId)
        {
            int currentPage = Page.CurrentPage;
            switch(currentPage)
            {
                case -1:
                    break;
                case 0:
                    return objSelect[objId];
                default:
                    return morePage_objSelect[currentPage-1][objId];
            }
            return false;
        }
        /// <summary>
        /// �жϵ�ǰ�Ƿ��г���һ�����ϵ�ҳ��
        /// </summary>
        /// <returns>��Ϊtrue������Ϊfalse</returns>
        internal static bool WhetherMorePages()
        {
            if (morePage_objSelect.Count == 0 || morePage_objStr.Count == 0)
                return false;
            else return true;
        }
        /// <summary>
        /// ��ʼ����
        /// </summary>
        /// <param name="msgbox_textui_objs">����FightUI_MsgBox_TextUI�е��Ӷ���</param>
        internal static void Initialize(Text[] msgbox_textui_objs)
        {
            MsgBox_TextUI_Objs= msgbox_textui_objs;
        }
        /// <summary>
        /// ҳ�������
        /// </summary>
        internal static class Page
        {
            private static int page = -1;
            /// <summary>
            /// ��ǰ��ʾ��ҳ��<br/>
            /// -1: û����ʾ<br/>
            /// 0: ��ʾ��һ��<br/>
            /// 1: ��ʾ�ڶ���<br/>
            /// ...: ����
            /// </summary>
            public static int CurrentPage
            {
                get { return page; }
                set {
                    page = value;
                    DrawUI();
                }
            }
        }
    }
    enum HeartPosName
    {
        none,freeMove,attack,
        fightBt, actBt, itemBt, mercyBt,
        textuiObj1,textuiObj2, textuiObj3, textuiObj4, textuiObj5,textuiObj6
    }
    enum Direction
    {
        up,down,left,right
    }
    /// <summary>
    /// �����ĵ�λ�ò�ˢ��UI
    /// </summary>
    /// <param name="hpName">�ĵ�λ��</param>
    void HeartUIPosChange(HeartPosName hpName)
    {
        heartNowPos = hpName;
        HeartUIUpdate();
    }
    /// <summary>
    /// �ĵ���UI�е��ƶ�����
    /// </summary>
    /// <param name="dn">�ƶ�����</param>
    void HeartUIMove(Direction dn)
    {
        switch (heartNowLevel)
        {
            case HeartLevelName.rootButton:
                {
                    int cache = (int)heartNowPos;
                    switch (dn)
                    {
                        case Direction.left:
                            cache--; break;
                        case Direction.right:
                            cache++; break;
                    }
                    if (cache < (int)HeartPosName.fightBt)
                        cache = (int)HeartPosName.mercyBt;
                    else if (cache > (int)HeartPosName.mercyBt)
                        cache = (int)HeartPosName.fightBt;

                    heartNowPos = (HeartPosName)cache;
                    HeartUIUpdate();
                }
                break;
            case HeartLevelName.textUI:
                {
                    int cache = (int)heartNowPos;
                    int change=0;
                    switch (dn)
                    {
                        case Direction.up:
                            change = -1;break;
                        case Direction.down:
                            change=1;break;
                            case Direction.left:
                                change=-3;break;
                        case Direction.right:
                            change = 3;break;
                    }
                    cache += change;
                    switch (textUIType)
                    {
                        case TextUIType.item:
                            if ((heartNowPos == HeartPosName.textuiObj4 || heartNowPos == HeartPosName.textuiObj5)
                                && dn == Direction.right && TextUI_Ctrl.Page.CurrentPage == 1 - 1)
                            {
                                switch(heartNowPos)
                                {
                                    case HeartPosName.textuiObj4:
                                        cache = (int)HeartPosName.textuiObj1;
                                        break;
                                        case HeartPosName.textuiObj5:
                                        cache =(int) HeartPosName.textuiObj2;
                                        break;
                                }    
                                TextUI_Ctrl.Page.CurrentPage = 2 - 1;
                            }
                            else if((heartNowPos == HeartPosName.textuiObj1 || heartNowPos == HeartPosName.textuiObj2)
                                && dn==Direction.left && TextUI_Ctrl.Page.CurrentPage==2-1)
                            {
                                switch (heartNowPos)
                                {
                                    case HeartPosName.textuiObj1:
                                        cache = (int)HeartPosName.textuiObj4;
                                        break;
                                    case HeartPosName.textuiObj2:
                                        cache = (int)HeartPosName.textuiObj5;
                                        break;
                                }
                                TextUI_Ctrl.Page.CurrentPage = 1 - 1;
                            }
                            break;
                        default:
                            if (cache < (int)HeartPosName.textuiObj1)
                                cache = (int)HeartPosName.textuiObj1;
                            else if (cache > (int)HeartPosName.textuiObj6)
                                cache = (int)HeartPosName.textuiObj6;
                            break;
                    }

                    if (TextUI_Ctrl.objSelect[cache - (int)HeartPosName.textuiObj1])
                    {
                        heartNowPos = (HeartPosName)cache;
                        HeartUIUpdate();
                    }
                }
                break;
        }
    }
    /// <summary>
    /// ˢ���ĵ�UIλ����ʾ�������������UI��ʾ
    /// </summary>
    void HeartUIUpdate()
    {
        Vector2 cacheV2=Vector2.zero;
        switch (heartNowLevel)
        {
            case HeartLevelName.rootButton:
                cacheV2 = HeartPos.GetMainBtPos(heartNowPos);
                Heart.transform.SetParent(gameObject.transform, false);

                fightButton.GetComponent<SpriteRenderer>().sprite = fight_ButtonImg[0];
                actButton.GetComponent<SpriteRenderer>().sprite = act_ButtonImg[0];
                itemButton.GetComponent<SpriteRenderer>().sprite = item_ButtonImg[0];
                mercyButton.GetComponent<SpriteRenderer>().sprite = mercy_ButtonImg[0];
                switch (heartNowPos)
                {
                    case HeartPosName.fightBt:
                        fightButton.GetComponent<SpriteRenderer>().sprite = fight_ButtonImg[1];break;
                    case HeartPosName.actBt:
                        actButton.GetComponent<SpriteRenderer>().sprite = act_ButtonImg[1];break;
                    case HeartPosName.itemBt:
                        itemButton.GetComponent<SpriteRenderer>().sprite = item_ButtonImg[1];break;
                    case HeartPosName.mercyBt:
                        mercyButton.GetComponent<SpriteRenderer>().sprite = mercy_ButtonImg[1];break;
                }
                break;
            case HeartLevelName.textUI:
                cacheV2=HeartPos.GetTextUIBtPos(heartNowPos);
                break;
        }
        Heart.transform.localPosition = cacheV2;        
    }
    /// <summary>
    /// �����ڵĲ㼶
    /// </summary>
     enum HeartLevelName
    {
        rootButton,textUI,
        freeMove,freeMove_Load,
        attack,attack_Load
    }
    /// <summary>
    /// �����ڵ�textUI�����
    /// </summary>
    enum TextUIType
    {
        none,item,act,mercy
    }
    /// <summary>
    /// �ĵ�ǰ���ڵ�λ��
    /// </summary>
    HeartPosName heartNowPos = HeartPosName.fightBt;
    /// <summary>
    /// ���ڸ�����ť�����ڵ�λ��
    /// </summary>
    HeartPosName heartRootPos = HeartPosName.fightBt;
    /// <summary>
    /// �ĵ�ǰ���ڵĲ㼶
    /// </summary>
    HeartLevelName heartNowLevel= HeartLevelName.rootButton;
    /// <summary>
    /// �ĵ�ǰ���ڵ�textUI�����
    /// </summary>
    TextUIType textUIType=TextUIType.none;

    public Sprite[] fight_ButtonImg;
    public Sprite[] act_ButtonImg;
    public Sprite[] item_ButtonImg;
    public Sprite[] mercy_ButtonImg;

    public GameObject Heart;
    public GameObject OpponentHeart;
    public GameObject fightButton;
    public GameObject actButton;
    public GameObject itemButton;
    public GameObject mercyButton;
    public GameObject MsgBox;
    public GameObject MsgBox_TextUI;
    public GameObject FightBox;
    public GameObject OpponentImg;
    SpriteRenderer OpponentImg_sr;
    public Text state_name;
    public Text state_LOVE;
    public Text state_HP;
    public RectTransform state_hpBar_main;
    public RectTransform state_hpBar_background;
    //public Text[] MsgBox_TextUI_Objs;

    #region OpponentHealth
    public delegate void OpponentHealthChange(int changeValue);
    public static event OpponentHealthChange OpponentHealth_Change;
    /// <summary>
    /// ���Ķ��ֵ�����ֵ<br/>������̬���ú����������ⲿ����
    /// </summary>
    /// <param name="changeValue">����ֵ</param>
    static public void ChangeOpponentHealth(int changeValue)
    {
        OpponentHealth_Change(changeValue);
    }
    /// <summary>
    /// ��������ֵ�����¼�
    /// </summary>
    /// <param name="changeValue">����ֵ</param>
    private void OpponentHealth_ChangeEvent(int changeValue)
    {
        ObjTagSet(opponentTag[0], TagHead.health,
            (int.Parse(ObjTagFind(opponentTag[0], TagHead.health)) + changeValue).ToString()
            );//�������Ѫ�������õ���ǩ
        if(opponentHpBarChangeThread==null || opponentHpBarChangeThread.IsAlive==false)
        {
            opponentHpBarChangeThread = new(OpponentHpBarChangeThread);
            opponentHpBarChangeThread.Start();
        }
        GameObject cacheGo;
        cacheGo = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/FightUI/FightUI_Opponent_hpBar_damageNum"));
        cacheGo.transform.SetParent(OpponentImg.transform.Find("FightUI_Opponent_hpBar").gameObject.transform,false);
        cacheGo.GetComponent<Text>().text= changeValue.ToString();
        cacheGo.SetActive(true);
        OpponentHeart.GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/FightUI/damage"));
    }
    Thread opponentHpBarChangeThread;
    void OpponentHpBarChangeThread()
    {
        GameObject hpBar=null;
        GameObject hpBar_background = null;
        GameObject hpBar_main = null;
        {
            bool wait = false;
            ThreadDelegate.QueueOnMainThread((param) =>
            {
                hpBar = OpponentImg.transform.Find("FightUI_Opponent_hpBar").gameObject;
                hpBar_background = hpBar.transform.Find("FightUI_Opponent_hpBar_background").gameObject;
                hpBar_main = hpBar.transform.Find("FightUI_Opponent_hpBar_main").gameObject;

                hpBar.SetActive(true);
                hpBar_background.SetActive(true);
                hpBar_main.SetActive(true);
                wait = true;
            }, null);
            while (!wait) ;
        }

        float GetUiValue()
        {
            float cache=0;
            {
                bool wait = false;
                ThreadDelegate.QueueOnMainThread((param) =>
                {
                    cache = hpBar_main.GetComponent<RectTransform>().sizeDelta.x;
                    wait = true;
                }, null);
                while (!wait) ;
            }
            return cache;
        }
        void SetUiValue(float setValue)
        {
            bool wait = false;
                ThreadDelegate.QueueOnMainThread((param) =>
                {
                    hpBar_main.GetComponent<RectTransform>().sizeDelta = 
                new Vector2(setValue, hpBar_main.GetComponent<RectTransform>().sizeDelta.y);
                    wait = true;
                }, null);
            while(!wait) ;
        }
        void AddUiValue(float addValue)
        {
            bool wait = false;
            float tmp = GetUiValue();
            ThreadDelegate.QueueOnMainThread((param) =>
            {
                hpBar_main.GetComponent<RectTransform>().sizeDelta =
                new Vector2(tmp + addValue, hpBar_main.GetComponent<RectTransform>().sizeDelta.y); 
                wait = true;
            }, null);
            while (!wait) ;
        }
        ///˫����֤����
        int doublePass=0;
        const float speedValue = 0.1f;//�ٶ�
doubleGo:;
        while (true)
        {
            float cacheuivalue = GetUiValue();//��ʱ�洢

            float uimax =0;//UI���ֵ
            float hpmax=0;//������ֵ
            float hpvalue=0;//����ֵ
            float targetUIValue=0;//Ŀ��Ҫ�ﵽ��UIֵ
            {
                bool wait = false;
                ThreadDelegate.QueueOnMainThread((param) =>
                {
                    uimax = hpBar_background.GetComponent<RectTransform>().sizeDelta.x;
                    hpmax = float.Parse(ObjTagFind(opponentTag[0], TagHead.maxHealth));
                    hpvalue = float.Parse(ObjTagFind(opponentTag[0], TagHead.health));
                    targetUIValue = (float)((float)hpvalue / (float)hpmax) * (float)uimax;
                    //Debug.Log(targetUIValue);
                    wait = true;
                }, null);
                while(!wait) ;
            }

            if (cacheuivalue > targetUIValue)
            {                
                doublePass = -1;//��ⲻͨ��ʱ�������˳��ȴ�ʱ��
                AddUiValue(-speedValue);
                cacheuivalue = GetUiValue();
                if (cacheuivalue < targetUIValue)//����������±��ʽ�б仯�����ú�����
                {
                    SetUiValue(targetUIValue);
                    break;
                }
                else if (cacheuivalue == targetUIValue)
                    break;
            }
            else if (cacheuivalue < targetUIValue)
            {
                doublePass = -1;
                AddUiValue(+speedValue);
                cacheuivalue = GetUiValue();
                if (cacheuivalue >targetUIValue)
                {
                    SetUiValue(targetUIValue);
                    break;
                }
                else if (cacheuivalue == targetUIValue)
                    break;
            }
            else break;

            Thread.Sleep(40);
        }
        if (doublePass == -1)
        {
            doublePass = 0;
            goto doubleGo;
        }
        else if (doublePass <200)//Ϊ����Ѫ���仯����ʾһ��ʱ��UI�����ֲ���ֱ��ʹ��Sleep��䣬��Ϊ�ǻᵼ���߳��޷���Ӧ��
            //ͨ����ʱ��Sleep�����ڵȴ�ʱ���м����Ƿ�����Ҫ�������ֵ�仯
        {
            doublePass++;
            Thread.Sleep(10);
            goto doubleGo;
        }
        else//���˳��ȴ��¼�����Ŀ��ֵ�󣬹ر�UI���˳��߳�
        {
            {
                bool wait = false;
                ThreadDelegate.QueueOnMainThread((param) =>
                {
                    hpBar.SetActive(false);
            hpBar_background.SetActive(false);
            hpBar_main.SetActive(false);
                    wait = true;
                }, null);
                while (!wait) ;
            }
        }
    }
    #endregion
    #region FriskHealth
    /// <summary>
    /// �޵�״̬�Ƿ�����
    /// </summary>
   static public bool friskINVState = false;
    public delegate void FriskHealthChange(int changeValue);
    public static event FriskHealthChange FriskHealth_Change;
    /// <summary>
    /// ����Frisk������ֵ<br/>������̬���ú����������ⲿ����
    /// </summary>
    /// <param name="changeValue">����ֵ</param>
    static public void ChangeFriskHealth(int changeValue)
    {
        FriskHealth_Change(changeValue);
    }
    /// <summary>
    /// Frisk����ֵ�����¼�
    /// </summary>
    /// <param name="changeValue">����ֵ</param>
    private void FriskHealth_ChangeEvent(int changeValue)
    {
        if (changeValue < 0)
            StartFriskINV();
        Player.Health.ChangeHealthNum(changeValue);
        //if (friskHpBarChangeThread == null || !friskHpBarChangeThread.IsAlive){
            friskHpBarChangeThread = new(FriskHpBarChangeThread);
            friskHpBarChangeThread.Start();
        //}        
    }
    Thread friskHpBarChangeThread;
    void FriskHpBarChangeThread()
    {
        ThreadDelegate.QueueOnMainThread((param) =>
        {
            state_HP.text = Player.Health.GetStringHealth();
            Heart.GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/FightUI/hurt1"));
        }, null);
        float GetUiValue()
        {
            float cache = 0;
            {
                bool wait = false;
                ThreadDelegate.QueueOnMainThread((param) =>
                {
                    cache = state_hpBar_main.GetComponent<RectTransform>().sizeDelta.x;
                    wait = true;
                }, null);
                while (!wait) ;
            }
            return cache;
        }
        void SetUiValue(float setValue)
        {
            bool wait = false;
            ThreadDelegate.QueueOnMainThread((param) =>
            {
                state_hpBar_main.GetComponent<RectTransform>().sizeDelta =
            new Vector2(setValue, state_hpBar_main.GetComponent<RectTransform>().sizeDelta.y);
                wait = true;
            }, null);
            while (!wait) ;
        }
        void AddUiValue(float addValue)
        {
            bool wait = false;
            float tmp = GetUiValue();
            ThreadDelegate.QueueOnMainThread((param) =>
            {
                state_hpBar_main.GetComponent<RectTransform>().sizeDelta =
                new Vector2(tmp + addValue, state_hpBar_main.GetComponent<RectTransform>().sizeDelta.y);
                wait = true;
            }, null);
            while (!wait) ;
        }
        const float speedValue = 0.1f;//�ٶ�
        while (true)
        {
            float cacheuivalue = GetUiValue();//��ʱ�洢

            float uimax = 0;//UI���ֵ
            float hpmax = 0;//������ֵ
            float hpvalue = 0;//����ֵ
            float targetUIValue = 0;//Ŀ��Ҫ�ﵽ��UIֵ
            {
                bool wait = false;
                ThreadDelegate.QueueOnMainThread((param) =>
                {
                    uimax = state_hpBar_background.GetComponent<RectTransform>().sizeDelta.x;
                    hpmax = (float)Player.Health.GetMaxHealth();
                    hpvalue = (float)Player.Health.GetHealth();
                    targetUIValue = (float)((float)hpvalue / (float)hpmax) * (float)uimax;
                    //Debug.Log(targetUIValue);
                    wait = true;
                }, null);
                while (!wait) ;
            }

            if (cacheuivalue > targetUIValue)
            {
                AddUiValue(-speedValue);
                cacheuivalue = GetUiValue();
                if (cacheuivalue < targetUIValue)//����������±��ʽ�б仯�����ú�����
                {
                    SetUiValue(targetUIValue);
                    break;
                }
                else if (cacheuivalue == targetUIValue)
                    break;
            }
            else if (cacheuivalue < targetUIValue)
            {
                AddUiValue(+speedValue);
                cacheuivalue = GetUiValue();
                if (cacheuivalue > targetUIValue)
                {
                    SetUiValue(targetUIValue);
                    break;
                }
                else if (cacheuivalue == targetUIValue)
                    break;
            }
            else break;

            Thread.Sleep(40);
        }
    }
    /// <summary>
    /// �����޵�״̬
    /// </summary>
    void StartFriskINV()
    {
        friskINVState = true;
        Thread t = new(() =>
        {
            SpriteRenderer sr=null;
            Color defC=new(),cv=new();
            {
                bool wait = false;
                ThreadDelegate.QueueOnMainThread((param) =>
                {
                    sr = Heart.GetComponent<SpriteRenderer>();
                    defC = sr.color;//Ĭ����ɫ
                    cv = new(sr.color.r, sr.color.g, sr.color.b, 1f);//colorValue��Ԥ���ĵ�ֵ
                    wait = true;
                }, null);
                while (!wait) ;
            }
            SleepTimer st = new();
            st.Start(Player.INV.GetINV() + int.Parse(ObjTagFind(opponentTag[0], TagHead.inv)));//��ʱ���޵�ʱ��
            st.TimerOver += () =>
            {
                friskINVState = false;
                ThreadDelegate.QueueOnMainThread((param) =>
                {
                    sr.color = defC;
                }, null);
            };

            int sv = 0;//switchValue
            while (!st.IsTimerOver)
            {
                bool wait = false;
                switch (sv)
                {
                    case 0:
                        ThreadDelegate.QueueOnMainThread((param) =>
                        {                           
                                cv.a = 0.5f;
                            if (!st.IsTimerOver)
                                sr.color = cv;
                            wait = true;
                        }, null);
                        sv = 1;
                        break;
                    case 1:
                        ThreadDelegate.QueueOnMainThread((param) =>
                        {
                            cv.a = 0.1f;
                            if (!st.IsTimerOver)
                                sr.color = cv;
                            wait = true;
                        }, null);
                        sv = 0;
                        break;
                }
                /*/if(st.IsTimerOver)//������֮ǰ�ٴμ��ʱ���Ƿ��ѵ�
                {
                    while (!wait) ;break;
                }*/
                Thread.Sleep(150);
                while (!wait) ;
            }
        });t.Start();
    }
    #endregion    

    /// <summary>
    /// ����ID
    /// </summary>
    public static ID[] opponentID;
    /// <summary>
    /// ���ֱ�ǩ
    /// </summary>
   public static  List<string[]>[] opponentTag;
    /// <summary>
    /// ���ֵĶ������Ʊ�ǩ
    /// </summary>
    public static List<string[]>[] opponentAcTag;
    public static List<string[]>[] opponentAcData;
    private void OnEnable()
    {
        //debug:
        opponentID =new ID[] { ID.debugDummy };
        opponentTag=new List<string[]>[] {DebugDummy.defTag};
        opponentAcTag=new List<string[]>[] {DebugDummy.acTag};
        opponentAcData= new List<string[]>[] { DebugDummy.acData };
        //debug;
        {
            Text[] cache = new Text[6];
            for(int i=1;i<=6; i++)
                cache[i-1]= MsgBox_TextUI.transform.Find("FightUI_MsgBox_TextUI_obj"+i.ToString()).gameObject.GetComponent<Text>();
            TextUI_Ctrl.Initialize(cache);//��ʼ��TextUI��
        }

        OpponentImg_sr=OpponentImg.GetComponent<SpriteRenderer>();
        OpponentImg_sr.sprite = Resources.Load<Sprite>(rpFightOpponent + ObjTagFind(opponentTag[0], TagHead.opponentImg)) as Sprite;
        state_name.text=Player.Name.GetName();
        state_LOVE.text="LV"+ Player.LOVE.GetLOVE().ToString();
        state_HP.text=Player.Health.GetStringHealth();

        GameObject.Find("MainCamera/BackgroundMusic").GetComponent<AudioSource>().Pause();
        //GameObject.Find("MainCamera/BackgroundMusic").GetComponent<AudioSource>().UnPause();
        Frisk_Walk.MoveLock(true);
        KeyState.cMenuOpenLock = true;
        KeyState.UseObjectInHandsLock = true;

        HeartUIUpdate();

        CallMsgBox(ObjTagFind(opponentAcData[0], ACDataHead.normalShow));
        {//�ȴ�enter�����𣬱�����������
            Thread enterUnlock = new(() =>
            {
                if (KeyClick_ThreadCall(KeyClick_ThreadCall_ID.EnterKey))
                {
                    while (KeyClick_ThreadCall(KeyClick_ThreadCall_ID.EnterKey))
                        Thread.Sleep(20);
                }
                enterWaitLock = false;
            });
            enterUnlock.Start();
        }
    }
    private void Start()
    {
        //����������ǩ֮�����ײ
        GameObject[] allPlayer = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] allFightUI = GameObject.FindGameObjectsWithTag("FightUI");
        GameObject[] allMoreFightUI = new GameObject[] {OpponentHeart};//�������ص�ui

        IgnoreCollisionControl(allPlayer, allFightUI);
        IgnoreCollisionControl(allPlayer,allMoreFightUI);
    }


    bool upWaitLock = false;
    bool downWaitLock = false;
    bool leftWaitLock = false;
    bool rightWaitLock = false;
    bool enterWaitLock = true;//��Ҫ�ȴ���ʼ�����ֽ������������ȷ��������ʹ��
    bool exitWaitLock = false;
    private void Update()
    {
        switch (heartNowLevel)
        {
            case HeartLevelName.freeMove_Load:
                IgnoreCollisionControl(new GameObject[] { Heart }, GameObject.FindGameObjectsWithTag("Player"));
                Heart.transform.SetParent(FightBox.transform, true);
                Heart.transform.localPosition = new(0, 0);
                Heart.SetActive(true);
                FightBoxLocSizeChange(FightBoxData.loc2FightBoxLocSize);
                heartNowLevel = HeartLevelName.freeMove;
                OpponentAttackData.Attack_Load(0);
                attackTimer = new();
                attackTimer.TimerOver += () =>
                {
                    ThreadDelegate.QueueOnMainThread((param) => {
                        OpponentAttackOver();
                    }, null);
                };
                attackTimer.Start(6 * 1000);//�����ƶ���ʱ��
                break;
            case HeartLevelName.freeMove:
                float x = Input.GetAxisRaw("Horizontal");//.GetAxis()�������ƶ����ԣ�ʹ��.GetAxisRaw()�����ƶ����ԣ�
                float y = Input.GetAxisRaw("Vertical");
                Heart.transform.Translate(Vector2.right * x * Player.HeartSpeed.GetHeartSpeed() * Time.deltaTime, Space.World);//ˮƽ�ƶ���xΪ�����ң�xΪ������
                Heart.transform.Translate(Vector2.up * y * Player.HeartSpeed.GetHeartSpeed() * Time.deltaTime, Space.World);//��ֱ�ƶ���yΪ�����ϣ�yΪ������
                break;
            case HeartLevelName.attack_Load:
                if (fightboxLocSizeChangeWaitLock == false)
                {
                    if (!FightAttackData.GetFriskWeaponActive())
                        FightAttackData.FriskWeapon_Load();
                    heartNowLevel = HeartLevelName.attack;
                    OpponentHealth_Change += OpponentHealth_ChangeEvent;//ע���������ֵ�仯�¼�
                    FriskHealth_Change += FriskHealth_ChangeEvent;
                    attackTimer = new();//��ʼ����ʱ����
                    attackTimer.TimerOver += () =>
                    {
                        ThreadDelegate.QueueOnMainThread((param) => {
                            PlayerAttackOver();
                        }, null);
                    };
                    attackTimer.Start(6*1000);//Ϊ�����趨ʱ�䣬������ʱ����ֹͣ����
                }
                break;
            case HeartLevelName.attack:
                {
                    int actionID=-1;
                    if (GameKey.UpKeyClick())
                        actionID = 3;
                    else if (GameKey.DownKeyClick())
                        actionID = 1;
                    else if (GameKey.LeftKeyClick())
                        actionID = 0;
                    else if (GameKey.RightKeyClick())
                        actionID = 2;
                    if (actionID != -1)
                    {
                        if (!FightAttackData.frisk_weapon_Use[actionID])
                        {
                            Vector2 GetVector()
                            {
                                //���Ƕ���ת��Ϊ������
                                float Angle_Z = (FightAttackData.frisk_weapon[actionID].transform.rotation.eulerAngles.z+90) * Mathf.Deg2Rad;
                                //��ȡ��������ת����
                                return new(Mathf.Cos(Angle_Z), Mathf.Sin(Angle_Z));                                
                            }
                            FightAttackData.frisk_weapon[actionID].GetComponent<Rigidbody2D>().velocity = 
                                                                GetVector()*FightAttackData.frisk_weapon_Speed;
                            FightAttackData.frisk_weapon_Use[actionID] = true;
                            Debug.Log(FightAttackData.frisk_weapon[actionID]);  
                        }
                        if(attackTimer.SleepTimeValue!=2000)//�ж��Ƿ��Ѿ����ù���ʱ��
                        {
                            bool allPass = true;
                            foreach(bool use in FightAttackData.frisk_weapon_Use)//�ж����������Ƿ���ʹ��
                            {
                                if(!use)
                                {
                                    allPass = false;
                                    break;
                                }
                            }
                            if (allPass)
                            {
                                attackTimer.Stop();
                                attackTimer = new();
                                attackTimer.TimerOver += () =>
                                {
                                    ThreadDelegate.QueueOnMainThread((param) => {
                                        PlayerAttackOver();
                                    }, null);
                                };
                                attackTimer.Start(2000);
                            }
                        }
                    }
                }
                break;
            default:
                {
                    if (!upWaitLock &&
                        heartNowLevel != HeartLevelName.freeMove && heartNowLevel != HeartLevelName.rootButton &&
                        GameKey.UpKeyClick())
                    {
                        upWaitLock = true;
                        switch (heartNowLevel)
                        {
                            case HeartLevelName.textUI:
                                HeartUIMove(Direction.up);
                                break;
                        }
                        Thread thread = new(() =>
                        {
                            while (KeyClick_ThreadCall(KeyClick_ThreadCall_ID.UpKey))
                                Thread.Sleep(20);
                            upWaitLock = false;
                        });
                        thread.Start();
                    }
                    else if (!downWaitLock &&
                        heartNowLevel != HeartLevelName.freeMove && heartNowLevel != HeartLevelName.rootButton &&
                        GameKey.DownKeyClick())
                    {
                        downWaitLock = true;
                        switch (heartNowLevel)
                        {
                            case HeartLevelName.textUI:
                                HeartUIMove(Direction.down);
                                break;
                        }
                        Thread thread = new(() =>
                        {
                            while (KeyClick_ThreadCall(KeyClick_ThreadCall_ID.DownKey))
                                Thread.Sleep(20);
                            downWaitLock = false;
                        });
                        thread.Start();
                    }
                    else if (!leftWaitLock &&
                        heartNowLevel != HeartLevelName.freeMove &&
                        GameKey.LeftKeyClick())
                    {
                        leftWaitLock = true;
                        switch (heartNowLevel)
                        {
                            case HeartLevelName.rootButton:
                            case HeartLevelName.textUI:
                                HeartUIMove(Direction.left);
                                break;
                        }
                        Thread thread = new(() =>
                        {
                            while (KeyClick_ThreadCall(KeyClick_ThreadCall_ID.LeftKey))
                                Thread.Sleep(20);
                            leftWaitLock = false;
                        });
                        thread.Start();
                    }
                    else if (!rightWaitLock &&
                        heartNowLevel != HeartLevelName.freeMove &&
                        GameKey.RightKeyClick())
                    {
                        rightWaitLock = true;
                        switch (heartNowLevel)
                        {
                            case HeartLevelName.rootButton:
                            case HeartLevelName.textUI:
                                HeartUIMove(Direction.right);
                                break;
                        }
                        Thread thread = new(() =>
                        {
                            while (KeyClick_ThreadCall(KeyClick_ThreadCall_ID.RightKey))
                                Thread.Sleep(20);
                            rightWaitLock = false;
                        });
                        thread.Start();
                    }
                    else if (!enterWaitLock && GameKey.OkKeyClick())
                    {
                        enterWaitLock = true;
                        switch (heartNowLevel)
                        {
                            case HeartLevelName.rootButton:
                                heartRootPos = heartNowPos;
                                switch (heartNowPos)
                                {
                                    case HeartPosName.fightBt:
                                        FightBox.SetActive(true);
                                        MsgBox.SetActive(false);
                                        FightBoxLocSizeChange(FightBoxData.loc1FightBoxLocSize);
                                        Heart.SetActive(false);
                                        Heart.transform.parent = MsgBox.transform;
                                        Heart.transform.localPosition = new Vector2(0, 0);
                                        OpponentHeart.SetActive(true);
                                        OpponentHeart.transform.localPosition = new Vector2(0, 0);
                                        heartNowLevel = HeartLevelName.attack_Load;
                                        break;
                                    case HeartPosName.actBt:
                                        break;
                                    case HeartPosName.itemBt:
                                        break;
                                    case HeartPosName.mercyBt:
                                        break;
                                }
                                break;
                            case HeartLevelName.textUI:
                                break;
                        }
                        Thread thread = new(() =>
                        {
                            while (KeyClick_ThreadCall(KeyClick_ThreadCall_ID.EnterKey))
                                Thread.Sleep(20);
                            enterWaitLock = false;
                        });
                        thread.Start();
                    }
                    else if (!exitWaitLock && GameKey.CancelKeyClick())
                    {
                        exitWaitLock = true;
                        /*switch (selectLevel)
                        {
                            case 0:
                                gameObject.SetActive(false);
                                cMenu_InfoBox_objectUI_Heart.SetActive(false);
                                break;
                            case 1:
                                cMenu_InfoBox_objectUI_Heart.transform.localPosition = cMenu_InfoBox_objectUI_Heart_loc[LocID];
                                selectLevel = 0;
                                break;
                        }*/

                        Thread thread = new(() =>
                        {
                            while (KeyClick_ThreadCall(KeyClick_ThreadCall_ID.ExitKey))
                                Thread.Sleep(20);
                            exitWaitLock = false;
                        });
                        thread.Start();
                    }
                }
                break ;
        }
    }
    /// <summary>
    /// ���������̵߳��ã���Ҫ�����߳��ڵ���
    /// </summary>
    /// <param name="keyID">ʹ��ö���ṩ��ͷָ�����</param>
    /// <returns></returns>
    bool KeyClick_ThreadCall(KeyClick_ThreadCall_ID keyID)
    {
        bool tmp = false;
        bool wait = false;
        ThreadDelegate.QueueOnMainThread((param) =>
        {
            switch (keyID)
            {
                case KeyClick_ThreadCall_ID.UpKey:
                    tmp = GameKey.UpKeyClick();
                    break;
                case KeyClick_ThreadCall_ID.DownKey:
                    tmp = GameKey.DownKeyClick();
                    break;
                case KeyClick_ThreadCall_ID.LeftKey:
                    tmp = GameKey.LeftKeyClick();
                    break;
                case KeyClick_ThreadCall_ID.RightKey:
                    tmp = GameKey.RightKeyClick();
                    break;
                case KeyClick_ThreadCall_ID.EnterKey:
                    tmp = GameKey.OkKeyClick();
                    break;
                case KeyClick_ThreadCall_ID.ExitKey:
                    tmp = GameKey.CancelKeyClick();
                    break;
            }
            wait = true;
        }, null);
        while (!wait) ;
        return tmp;
    }
    enum KeyClick_ThreadCall_ID
    {
        UpKey, DownKey, LeftKey, RightKey, EnterKey, ExitKey
    }

    /// <summary>
    /// ����msgbox����
    /// <paramref name="str">������ı���������ID</paramref>
    /// <paramref name="isLangKey">�Ƿ�������ID</paramref>
    /// </summary>
    void CallMsgBox(string str,bool isLangKey=true)
    {
        GameObject.Find("MainCamera").transform.Find("MessageBox").gameObject.GetComponent<MessageBox>().
        StartMsgBox(
            inputValue: str, isLangKey: isLangKey,
            audio: Resources.Load<AudioClip>("Sounds/MessageBox/SND_TXT2"),audioSourceObject: MsgBox_TextUI.transform.Find("FightUI_MsgBox_TextUI_mainText").GetComponent<AudioSource>(),
            keyContinue: false,haveNext:true,
            banChangeTextUI: true,
            gameObject_TextUI: MsgBox_TextUI,
            gameObject_TextUI_TextObj: MsgBox_TextUI.transform.Find("FightUI_MsgBox_TextUI_mainText").gameObject
                    );
    }
}
