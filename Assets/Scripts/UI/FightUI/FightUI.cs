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
#pragma warning disable CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
    /// <summary>
    /// 战斗框数据计算类
    /// </summary>
    static class FightBoxData
    {
        /// <summary>
        /// 预设战斗框位置1，偏向敌人
        /// </summary>
        public const string loc1FightBoxLocSize = "-0.0252,0.2,0.74,0.64";
        /// <summary>
        /// 预设战斗框位置1，偏向自己
        /// </summary>
        public const string loc2FightBoxLocSize = "-0.0252,-0.32,0.74,0.64";
        /// <summary>
        /// 战斗框未改变时的初始位置
        /// </summary>
        public const string normalFightBoxLocSize = "-0.0252,-0.427,2,0.46";
        /// <summary>
        /// 根据战斗框数据组获取坐标数据
        /// </summary>
        /// <param name="dataStr">由','组成的数据组<br/>如果为null则使用默认的数据组参数</param>
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
        /// 根据战斗框数据组获取大小数据
        /// </summary>
        /// <param name="dataStr">由','组成的数据组<br/>如果为null则使用默认的数据组参数</param>
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
    #region HeartLevelName.attack模块内容
    /// <summary>
    /// 对手战斗攻击计算类
    /// </summary>
    internal static class OpponentAttackData
    {
        private static readonly GameObject loadObj = Resources.Load<GameObject>("Prefabs/UI/FightUI/FightUI_FightBox_OpponentWeapon");
        /// <summary>
        /// 父容器对象
        /// </summary>
        private static readonly UnityEngine.Transform parentObj = GameObject.Find("MainCamera/FightUI/FightUI_FightBox").transform;
        /// <summary>
        /// 加载对象图片函数
        /// </summary>
        /// <param name="weaponID">武器存储变量数组编号</param>
        /// <param name="imgName">要加载的图片ID</param>
        private static void LoadImg(int weaponID,string imgName)
        {
            SpriteRenderer sr =
            weapon[weaponID].GetComponent<SpriteRenderer>();
            sr.drawMode = SpriteDrawMode.Simple;//先切换到简单绘制模式，让对象大小随图片变化
            sr.sprite = Resources.Load<Sprite>("Images/UI/FightUI/weapon/" + imgName);
            sr.drawMode = SpriteDrawMode.Sliced;//切换到切片绘制模式，让对象的碰撞大小随对象大小变化
        }
        /// <summary>
        /// 武器对象存储对象
        /// </summary>
        internal static List<GameObject> weapon = new();
        /// <summary>
        /// 移动方法控制ID。<br/>
        /// {攻击ID,个体ID}
        /// </summary>
        internal static List<int[]> weaponMoveID = new();
        /// <summary>
        /// 控制线程
        /// </summary>
        static Thread controlThread;
        /// <summary>
        /// 控制线程是否终止
        /// </summary>
        static bool controlThread_stop;
        /// <summary>
        /// 攻击加载
        /// </summary>
        /// <param name="AttackId">攻击方式ID</param>
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
                                case 0://第0个战斗方案
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
    /// 战斗攻击计算类
    /// </summary>
    static class FightAttackData
    {
        internal static List<GameObject> frisk_weapon = new();
        /// <summary>
        /// 武器是否被消耗
        /// </summary>
        internal static List<bool> frisk_weapon_Use=new();
        internal static float frisk_weapon_Speed = 2f;
        /// <summary>
        /// 加载frisk的武器
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
        /// 获取当前frisk武器对象的存活状态
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
        /// 卸载frisk的武器
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
    /// 相对应的方法函数的等待锁
    /// </summary>
    bool fightboxLocSizeChangeWaitLock = false;
    /// <summary>
    /// 提供给attack块的计时器类
    /// </summary>
    SleepTimer attackTimer;
    /// <summary>
    /// 玩家攻击结束函数
    /// </summary>
    void PlayerAttackOver()
    {        
            FightAttackData.FriskWeapon_Destory();
            OpponentHeart.SetActive(false);
        heartNowLevel = HeartLevelName.freeMove_Load;
    }
    #endregion
    /// <summary>
    /// 动画形式更改战斗框大小和位置<br/>该方法是在线程内运行的，所以运行时不会等待其运行完毕
    /// </summary>
    /// <param name="dataStr">目标位置大小数据组</param>
    /// <param name="speed">速度</param>
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
            ///目标位置
            Vector2 targetLoc=FightBoxData.GetLoc(dataStr);
            ///目标大小
            Vector2 targetSize=FightBoxData.GetSize(dataStr);
            ///用于更改并赋值，因为对象的属性不支持直接更改某一个值
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
            ///cp=compute，指定每个数值是递增还是递减还是不变，分别用1,-1,0代替
            int cpLocX, cpLocY,cpSizeX,cpSizeY;
            ///判断是否完成计算
            bool doneLocX,doneLocY,doneSizeX,doneSizeY;
            {//判断计算方法，为递增还是递减还是不变
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
            ///更改当前坐标变量,nowLoc
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
            ///更改当前大小变量,nowSize
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
            ///根据场景大小更改碰撞边缘大小
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
#pragma warning restore CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
    /// <summary>
    /// 心图标在各个按钮的位置坐标
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
        /// 从常量/只读数据库中调用对应位置的坐标数据<br/>
        /// 只能为HeartLevelName.rootButton层的4个按钮
        /// </summary>
        /// <param name="hps">调用指定对象对应的坐标</param>
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
                    throw (new Exception("几乎不可能发生的错误！"));
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
        /// 从常量/只读数据库中调用对应位置的坐标数据
        /// </summary>
        /// <param name="id">目标对象名的序号<br/>例如: FightUI_MsgBox_TextUI_obj1最后面的编号1</param>
        /// <returns></returns>
        internal static Vector2 GetTextUIBtPos(int id)
        {
            if (id >= 3)
                return new Vector2(TextUI.x[0], TextUI.y[id - 1]);
            else
                return new Vector2(TextUI.x[1], TextUI.y[id-3-1]);
        }
        /// <summary>
        /// 从常量/只读数据库中调用对应位置的坐标数据<br/>
        /// 只能将枚举参数设置为前缀为textuiObj的，否则将会出现错误
        /// </summary>
        /// <param name="hps">目标对象的枚举ID</param>
        /// <returns></returns>
        internal static Vector2 GetTextUIBtPos(HeartPosName hps)
        {
            return GetTextUIBtPos((int)hps-HeartPosName.textuiObj1+1);//根据计算得出对象编号
        }
    }
    /// <summary>
    /// textui控制类，使用前必须初始化
    /// </summary>
    private static class TextUI_Ctrl
    {
        static Text[] MsgBox_TextUI_Objs;
        /// <summary>
        /// 分别代表六个文本对象，表示其是否能被选中
        /// </summary>
        internal static bool[] objSelect = new bool[6];
        /// <summary>
        /// 代表六个文本对象的内容
        /// </summary>
        internal static string[] objStr= new string[6];
        /// <summary>
        /// 如果有更多页面，则多出的页面存储在该变量<br/>每个布尔数组中分别代表六个文本对象，表示其是否能被选中
        /// </summary>
        internal static List<bool[]> morePage_objSelect = new();
        /// <summary>
        /// 如果有更多页面，则多出的页面存储在该变量<br/>每个字符串数组中包含六个文本对象的内容
        /// </summary>
        internal static List<string[]> morePage_objStr = new();
        /// <summary>
        /// 绘制UI函数
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
        /// 判断文本对象是否能被选中
        /// </summary>
        /// <param name="objId">文本对象ID，以0开头</param>
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
        /// 判断当前是否有超过一个以上的页面
        /// </summary>
        /// <returns>有为true，否则为false</returns>
        internal static bool WhetherMorePages()
        {
            if (morePage_objSelect.Count == 0 || morePage_objStr.Count == 0)
                return false;
            else return true;
        }
        /// <summary>
        /// 初始化类
        /// </summary>
        /// <param name="msgbox_textui_objs">对象FightUI_MsgBox_TextUI中的子对象</param>
        internal static void Initialize(Text[] msgbox_textui_objs)
        {
            MsgBox_TextUI_Objs= msgbox_textui_objs;
        }
        /// <summary>
        /// 页面控制类
        /// </summary>
        internal static class Page
        {
            private static int page = -1;
            /// <summary>
            /// 当前显示的页面<br/>
            /// -1: 没有显示<br/>
            /// 0: 显示第一面<br/>
            /// 1: 显示第二面<br/>
            /// ...: 更多
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
    /// 更改心的位置并刷新UI
    /// </summary>
    /// <param name="hpName">心的位置</param>
    void HeartUIPosChange(HeartPosName hpName)
    {
        heartNowPos = hpName;
        HeartUIUpdate();
    }
    /// <summary>
    /// 心的在UI中的移动函数
    /// </summary>
    /// <param name="dn">移动方向</param>
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
    /// 刷新心的UI位置显示和其它相关联的UI显示
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
    /// 心所在的层级
    /// </summary>
     enum HeartLevelName
    {
        rootButton,textUI,
        freeMove,freeMove_Load,
        attack,attack_Load
    }
    /// <summary>
    /// 心所在的textUI的类别
    /// </summary>
    enum TextUIType
    {
        none,item,act,mercy
    }
    /// <summary>
    /// 心当前所在的位置
    /// </summary>
    HeartPosName heartNowPos = HeartPosName.fightBt;
    /// <summary>
    /// 心在根部按钮中所在的位置
    /// </summary>
    HeartPosName heartRootPos = HeartPosName.fightBt;
    /// <summary>
    /// 心当前所在的层级
    /// </summary>
    HeartLevelName heartNowLevel= HeartLevelName.rootButton;
    /// <summary>
    /// 心当前所在的textUI的类别
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
    /// 更改对手的生命值<br/>公共静态调用函数，给予外部调用
    /// </summary>
    /// <param name="changeValue">更改值</param>
    static public void ChangeOpponentHealth(int changeValue)
    {
        OpponentHealth_Change(changeValue);
    }
    /// <summary>
    /// 对手生命值更改事件
    /// </summary>
    /// <param name="changeValue">更改值</param>
    private void OpponentHealth_ChangeEvent(int changeValue)
    {
        ObjTagSet(opponentTag[0], TagHead.health,
            (int.Parse(ObjTagFind(opponentTag[0], TagHead.health)) + changeValue).ToString()
            );//计算对手血量并设置到标签
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
        ///双重验证变量
        int doublePass=0;
        const float speedValue = 0.1f;//速度
doubleGo:;
        while (true)
        {
            float cacheuivalue = GetUiValue();//临时存储

            float uimax =0;//UI最大值
            float hpmax=0;//总生命值
            float hpvalue=0;//生命值
            float targetUIValue=0;//目标要达到的UI值
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
                doublePass = -1;//检测不通过时，重置退出等待时间
                AddUiValue(-speedValue);
                cacheuivalue = GetUiValue();
                if (cacheuivalue < targetUIValue)//如果操作后导致表达式有变化则设置后跳出
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
        else if (doublePass <200)//为了在血量变化后显示一段时间UI，但又不能直接使用Sleep语句，因为那会导致线程无法响应。
            //通过短时间Sleep可以在等待时间中间检查是否有需要处理的数值变化
        {
            doublePass++;
            Thread.Sleep(10);
            goto doubleGo;
        }
        else//当退出等待事件到达目标值后，关闭UI且退出线程
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
    /// 无敌状态是否启动
    /// </summary>
   static public bool friskINVState = false;
    public delegate void FriskHealthChange(int changeValue);
    public static event FriskHealthChange FriskHealth_Change;
    /// <summary>
    /// 更改Frisk的生命值<br/>公共静态调用函数，给予外部调用
    /// </summary>
    /// <param name="changeValue">更改值</param>
    static public void ChangeFriskHealth(int changeValue)
    {
        FriskHealth_Change(changeValue);
    }
    /// <summary>
    /// Frisk生命值更改事件
    /// </summary>
    /// <param name="changeValue">更改值</param>
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
        const float speedValue = 0.1f;//速度
        while (true)
        {
            float cacheuivalue = GetUiValue();//临时存储

            float uimax = 0;//UI最大值
            float hpmax = 0;//总生命值
            float hpvalue = 0;//生命值
            float targetUIValue = 0;//目标要达到的UI值
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
                if (cacheuivalue < targetUIValue)//如果操作后导致表达式有变化则设置后跳出
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
    /// 启动无敌状态
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
                    defC = sr.color;//默认颜色
                    cv = new(sr.color.r, sr.color.g, sr.color.b, 1f);//colorValue，预更改的值
                    wait = true;
                }, null);
                while (!wait) ;
            }
            SleepTimer st = new();
            st.Start(Player.INV.GetINV() + int.Parse(ObjTagFind(opponentTag[0], TagHead.inv)));//计时，无敌时间
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
                /*/if(st.IsTimerOver)//在休眠之前再次检查时间是否已到
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
    /// 对手ID
    /// </summary>
    public static ID[] opponentID;
    /// <summary>
    /// 对手标签
    /// </summary>
   public static  List<string[]>[] opponentTag;
    /// <summary>
    /// 对手的动作控制标签
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
            TextUI_Ctrl.Initialize(cache);//初始化TextUI类
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
        {//等待enter键弹起，避免连续触发
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
        //忽略两个标签之间的碰撞
        GameObject[] allPlayer = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] allFightUI = GameObject.FindGameObjectsWithTag("FightUI");
        GameObject[] allMoreFightUI = new GameObject[] {OpponentHeart};//部分隐藏的ui

        IgnoreCollisionControl(allPlayer, allFightUI);
        IgnoreCollisionControl(allPlayer,allMoreFightUI);
    }


    bool upWaitLock = false;
    bool downWaitLock = false;
    bool leftWaitLock = false;
    bool rightWaitLock = false;
    bool enterWaitLock = true;//需要等待初始化部分将其解锁，避免确定键连续使用
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
                attackTimer.Start(6 * 1000);//自由移动的时间
                break;
            case HeartLevelName.freeMove:
                float x = Input.GetAxisRaw("Horizontal");//.GetAxis()将保留移动惯性，使用.GetAxisRaw()消除移动惯性，
                float y = Input.GetAxisRaw("Vertical");
                Heart.transform.Translate(Vector2.right * x * Player.HeartSpeed.GetHeartSpeed() * Time.deltaTime, Space.World);//水平移动，x为正向右，x为负向左
                Heart.transform.Translate(Vector2.up * y * Player.HeartSpeed.GetHeartSpeed() * Time.deltaTime, Space.World);//垂直移动，y为正向上，y为负向下
                break;
            case HeartLevelName.attack_Load:
                if (fightboxLocSizeChangeWaitLock == false)
                {
                    if (!FightAttackData.GetFriskWeaponActive())
                        FightAttackData.FriskWeapon_Load();
                    heartNowLevel = HeartLevelName.attack;
                    OpponentHealth_Change += OpponentHealth_ChangeEvent;//注册对手生命值变化事件
                    FriskHealth_Change += FriskHealth_ChangeEvent;
                    attackTimer = new();//初始化计时器类
                    attackTimer.TimerOver += () =>
                    {
                        ThreadDelegate.QueueOnMainThread((param) => {
                            PlayerAttackOver();
                        }, null);
                    };
                    attackTimer.Start(6*1000);//为攻击设定时间，超过该时间则停止攻击
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
                                //将角度制转换为弧度制
                                float Angle_Z = (FightAttackData.frisk_weapon[actionID].transform.rotation.eulerAngles.z+90) * Mathf.Deg2Rad;
                                //获取并返回旋转向量
                                return new(Mathf.Cos(Angle_Z), Mathf.Sin(Angle_Z));                                
                            }
                            FightAttackData.frisk_weapon[actionID].GetComponent<Rigidbody2D>().velocity = 
                                                                GetVector()*FightAttackData.frisk_weapon_Speed;
                            FightAttackData.frisk_weapon_Use[actionID] = true;
                            Debug.Log(FightAttackData.frisk_weapon[actionID]);  
                        }
                        if(attackTimer.SleepTimeValue!=2000)//判断是否已经设置过计时器
                        {
                            bool allPass = true;
                            foreach(bool use in FightAttackData.frisk_weapon_Use)//判断所有武器是否已使用
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
    /// 仅供在子线程调用，不要在主线程内调用
    /// </summary>
    /// <param name="keyID">使用枚举提供箭头指向参数</param>
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
    /// 调用msgbox函数
    /// <paramref name="str">输出的文本或是语言ID</paramref>
    /// <paramref name="isLangKey">是否是语言ID</paramref>
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
