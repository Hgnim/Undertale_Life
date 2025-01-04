using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Localization.PropertyVariants.TrackedProperties;
using UnityEngine.Rendering.Universal;
using static CoreData.CData.Objects;
using static CoreData.CData.People;
using static CoreData.CData.Player;
using static UnityEditor.Progress;

namespace CoreData
{    
    static public class CData
    {      
        static public class Objects
        {
            /// <summary>
            /// 可拾取物品的ID，用于判断
            /// </summary>
            public enum CanPickedObject
            {
                none,
                hoe,
                wheatSeedBag
            }
            /// <summary>
            /// 可拾取物品的tag标签头部
            /// </summary>
            public enum CPObjTagHead
            {
                alias,
                langID,
                useMode,
                DF,//防御值
                AT,//攻击值
            }
            //这些数据只代表他们默认出现的位置，拾取后将创建一个单独的带tag的个体。
            static public class CPObj_hoe
            {
                /// <summary>
                /// 该物品是否存在
                /// </summary>
                static public bool Exist= true;
                /// <summary>
                /// 该物品的位置，默认给予的值是该物品的默认位置，后续可更改。
                /// </summary>
               static public Vector2 ObjectLocation = new(-0.652f, -0.697f);

                static readonly public List<string[]> defTag = new()
                {
                    new string[]{CPObjTagHead.alias.ToString(),"obj_hoe"},// 别名，用于判断资源，其资源文件都是使用变量的别名
                    new string[]{CPObjTagHead.langID.ToString(),"object_name_hoe"},// 语言文件ID
                    new string[]{CPObjTagHead.useMode.ToString(), "hand" }// 该物品的使用方法,hand是手持
                };
            }
            static public class CPObj_wheatSeedBag
            {
                /// <summary>
                /// 该物品是否存在
                /// </summary>
                static public bool Exist = true;
                /// <summary>
                /// 该物品的位置，默认给予的值是该物品的默认位置，后续可更改。
                /// </summary>
                static public Vector2 ObjectLocation = new(-0.821f, -0.697f);

                static readonly public List<string[]> defTag = new()
                {
                    new string[]{CPObjTagHead.alias.ToString(),"obj_wheatSeedBag"},// 别名
                    new string[]{CPObjTagHead.langID.ToString(),"object_name_wheatSeedBag"},// 语言文件ID
                    new string[]{CPObjTagHead.useMode.ToString(), "hand" }// 该物品的使用方法,hand是手持
                };
            }
        }
        static public class People
        {
            /// <summary>
            /// 资源文件路径ResourcesPath
            /// </summary>
            public const string rpFightOpponent = "Images/UI/FightUI/FightOpponent/";
            /// <summary>
            /// 人物对象ID
            /// </summary>
            public enum ID
            {
                none,
                debugDummy
            }
            public enum TagHead
            {
                alias
                , langID
                , maxHealth//最大血量
                , health//血量
                , opponentImg//对手贴图
                , mercy//对手敌对度，降到0后可饶恕
                    , fightUIBoxLocSize//战斗框位置大小，如果没有该标签则为默认
                ,attack//攻击力
                    ,inv//给予的无敌时间，单位: ms
            }
            /// <summary>
            /// 全写ActionControlTagHead<br/>
            /// 用于控制对手行动的标签头
            /// </summary>
            public enum ACDataHead
            {
                //Say和Show两个后缀有区别，Say代表对手用单独的对话气泡说出的话，Show代表对手用对话框给予的反馈
                //标签头后面可以加上数字编号，代表触发次数的不同对话，例如:normalShow.ToString()+"1"
                //标签头后面加上数字编号后再加一个小写r代表随机进行，否则将按编号顺序进行
                    startSay//遭遇后说的话
                    ,startShow//遭遇后的反馈，用于第一回合
                    ,normalSay//默认说的话
                    ,normalShow//默认的反馈
                    ,attackSay//被攻击时说的话
                    ,attackShow//被攻击时的反馈
                    ,actSay//使用行动时说的话
                    ,actShow//使用行动时的反馈，此处可以使用多维数的方式来表示，例如第一位代表行动编号，第二位代表不同的反馈等，例如actShow,ToString()+"2,1"
                    ,mercySay
                    ,mercyShow//同理
                    ,allDoAttackNum//拥有的发动攻击的方法的总数
            }
            public enum ACTagHead
            {
                normalShowID
                    ,attackShowID
                    ,attackID//攻击方式ID
            }


            static public class DebugDummy
            {
                static readonly public List<string[]> defTag = new()
                {
                    new string[]{TagHead.opponentImg.ToString(), "dummybattle_0" }
                    //如果对手有多个贴图组成，则可以在数组的后面继续添加贴图名，按从上到下的顺序绘制
                    ,new string[]{TagHead.health.ToString(),"100"}
                    ,new string[]{TagHead.maxHealth.ToString(),"100"}
                    ,new string[]{TagHead.mercy.ToString(),"10"}
                    ,new string[]{TagHead.attack.ToString(),"2"}
                    ,new string[]{TagHead.inv.ToString(),"3000"}
                };

                static readonly public List<string[]> acData = new()
                {
                    new string[]{ACDataHead.normalShow.ToString(), "people_DebugDummy_show_normal" }
                    ,new string[]{ACDataHead.attackShow.ToString()+"0r", "people_DebugDummy_show_attack0" }
                    ,new string[]{ACDataHead.attackShow.ToString()+"1r", "people_DebugDummy_show_attack1" }
                    ,new string[]{ACDataHead.allDoAttackNum.ToString(),"1"}
                };
                static readonly public List<string[]> acTag = new()
                {
                    new string[]{ACTagHead.normalShowID.ToString(),"1"}
                    ,new string[]{ACTagHead.attackShowID.ToString(),"1"}
                    ,new string[]{ACTagHead.attackID.ToString(),"1"}
                };
            }
        }
        static public class Player
        {
            static public class Name
            {
                private static string name="Frisk";
                /// <summary>
                /// 获取玩家名称
                /// </summary>
                /// <returns></returns>
                public static string GetName()
                {
                    return name;
                }
                /// <summary>
                /// 设置名字
                /// </summary>
                /// <param name="name">名字</param>
                public static void SetName(string name)
                {
                    name = Name.name;
                }
            }
            /// <summary>
            /// 简称DF，表示防御力
            /// </summary>
            static public class Defense
            {
                /// <summary>
                /// LOVE所给予的防御力
                /// </summary>
                private static int LVDF=10;
                /// <summary>
                /// 物品所给予的防御力
                /// </summary>
                private static int itemDF;

                /// <summary>
                /// 刷新物品的防御力数据
                /// </summary>
                public static void UpdateItemDF()
                {
                    if (MainItem.GetItem(MainItem.PickedLoc.body) != CanPickedObject.none)
                    {
                       itemDF=int.Parse(ToolClass.ObjTagFind(MainItem.GetItemTag(MainItem.PickedLoc.body), CPObjTagHead.DF));//获取物品的防御属性标签
                    }else
                        itemDF = 0;
                }
                /// <summary>
                /// 设置LOVE给予的防御力数值，只能由LOVE类进行调用
                /// </summary>
                /// <param name="lvdf">数值</param>
                internal static void SetLVDF(int lvdf)
                {
                    LVDF= lvdf;
                }
                /// <summary>
                /// 获取等级给予的防御力数值
                /// </summary>
                /// <returns></returns>
                public static int GetLVDF()
                {
                    return LVDF;
                }
                /// <summary>
                /// 获取物品给予的防御力数值
                /// </summary>
                /// <returns></returns>
                public static int GetItemDF()
                {
                    return itemDF;
                }
                /// <summary>
                /// 获取格式化后的防御力字符串，格式化为:"等级给予的防御值(物品给予的防御值)"
                /// </summary>
                /// <returns></returns>
                public static string GetStringDF()
                {
                    return GetLVDF().ToString() + "(" + GetItemDF().ToString() + ")";
                }
            }
            /// <summary>
            /// 简称AT，表示攻击力
            /// </summary>
            static public class Attack
            {
                /// <summary>
                /// LOVE所给予的攻击力
                /// </summary>
                private static int LVAT=10;
                /// <summary>
                /// 物品所给予的攻击力
                /// </summary>
                private static int itemAT;

                /// <summary>
                /// 刷新物品的攻击力数据
                /// </summary>
                public static void UpdateItemAT()
                {
                    if (MainItem.GetItem(MainItem.PickedLoc.body) != CanPickedObject.none)
                    {
                        string cache = ToolClass.ObjTagFind(MainItem.GetItemTag(MainItem.PickedLoc.hand),CPObjTagHead.AT);
                        if(cache!=null)
                        itemAT = int.Parse(cache);//获取物品的防御属性标签
                        else
                            itemAT = 0;
                    }
                    else
                        itemAT = 0;
                }
                /// <summary>
                /// 设置LOVE给予的攻击力数值，只能由LOVE类进行调用
                /// </summary>
                /// <param name="lvat">数值</param>
                internal static void SetLVAT(int lvat)
                {
                    LVAT = lvat;
                }
                /// <summary>
                /// 获取等级给予的攻击力数值
                /// </summary>
                /// <returns></returns>
                public static int GetLVAT()
                {
                    return LVAT;
                }
                /// <summary>
                /// 获取物品给予的攻击力数值
                /// </summary>
                /// <returns></returns>
                public static int GetItemAT()
                {
                    return itemAT;
                }
                /// <summary>
                /// 获取格式化后的攻击力字符串，格式化为:"等级给予的攻击力值(物品给予的攻击力值)"
                /// </summary>
                /// <returns></returns>
                public static string GetStringAT()
                {
                    return LVAT.ToString() +"("+ itemAT.ToString()+")";
                }
                /// <summary>
                /// 获取攻击力的总和
                /// </summary>
                /// <returns></returns>
                public static int GetAllAT()
                {
                    return LVAT+itemAT;
                }
            }
            static public class LOVE
            {
                private static readonly int[,] LVData = new int[,]
                {
                    //数值解释: 每一个一维数组都代表每一个LOVE等级对应的数值
                    //HP AT DF EXP
                    { 0,0,0,0},
                    {20,10,10,0},//LOVE 1
                    {24,12,10,10 },
                    {28,14,10,30 },
                    {32,16,10,70},
                    {36,18,11,120 },//5
                    {40,20,11,200 },
                    {44,22,11,300 },
                    {48,24,11,500 },
                    {52,26,12,800 },
                    {56,28,12,1200 },//10
                    {60,30,12,1700},
                    {64,32,12,2500 },
                    {68,34,13,3500 },
                    {72,36,13,5000},
                    {76,38,13,7000 },//15
                    {80,40,13,10000 },
                    {84,42,14,15000 },
                    {88,44,14,25000 },
                    {92,46,14,50000 },
                    {99,48,14,99999 }//20
                };
                private static int LOVE_Value=1;
                private static int EXP = 0;

                /// <summary>
                /// 获取到下一个LOVE还需要的EXP
                /// </summary>
                /// <returns>
                /// 如果返回-1，则表示已满级
                /// </returns>
                public static int GetNextLvNeedExp()
                {
                    if(LVData.Length >= LOVE_Value + 1)
                    {
                        return LVData[LOVE_Value + 1,3] - EXP;
                    }
                    else
                    {
                        return -1;
                    }
                }
                /// <summary>
                /// 增加经验值
                /// </summary>
                /// <param name="addNum">增加的数额</param>
                public static void AddExp(int addNum)
                {
                    EXP+= addNum;
                    if (EXP >= LVData[LOVE_Value + 1, 3])//如果经验值大于所需经验值，则升级
                    {
                        LOVE_Value++;
                        Health.SetMaxHealth(LVData[LOVE_Value, 0]);
                        Attack.SetLVAT(LVData[LOVE_Value, 1]);
                        Defense.SetLVDF(LVData[LOVE_Value, 2]);
                    }
                }
                /// <summary>
                /// 获取当前LOVE值
                /// </summary>
                /// <returns></returns>
                public static int GetLOVE()
                {
                    return LOVE_Value;
                }
                /// <summary>
                /// 返回当前EXP值
                /// </summary>
                /// <returns></returns>
                public static int GetEXP()
                {
                    return EXP;
                }
            }
            static public class Health
            {
                private static int maxHealth=20;
                /// <summary>
                /// 设置最大生命值
                /// </summary>
                /// <param name="maxHealth">最大生命值</param>
                internal static void SetMaxHealth(int maxHealth)
                {
                    maxHealth =Health.maxHealth;
                }
                /// <summary>
                /// 获取当前最大生命值
                /// </summary>
                /// <returns></returns>
                public static int GetMaxHealth()
                {
                    return maxHealth;
                }

                private static int health=maxHealth;
                /// <summary>
                /// 返回当前生命值数额
                /// </summary>
                /// <returns></returns>
                public static int GetHealth()
                {
                    return health;
                }
                /// <summary>
                /// 更改当前生命值数量，增加或减少
                /// </summary>
                /// <param name="changNum">需要增加或减少的数额</param>
                /// <returns>
                /// -1: 发生未知错误<br/>
                /// 0: 更改成功<br/>
                /// 1: 失败，生命值已小于等于0，玩家已死亡
                /// </returns>
                public static int ChangeHealthNum(int changNum)
                {
                    if (changNum > 0)
                    {
                        health += changNum;
                        return 0;
                    }
                    else if (changNum < 0)
                    {
                        if (health + changNum <= 0)
                        {
                            health = 0;
                            return 1;//这里也可以执行死亡函数，但是最好还是交给主要部分的动态线程执行
                        }
                        else
                        {
                            health += changNum;
                            return 0;
                        }
                    }
                    else
                        return -1;
                }
                /// <summary>
                /// 设置当前生命值
                /// </summary>
                /// <param name="setNum">设置的数额</param>
                public static void SetHealthNum(int setNum)
                {
                    health=setNum;
                }
                /// <summary>
                /// 获取格式化后的生命值字符串，格式为"当前生命值/最大生命值"
                /// </summary>
                /// <returns></returns>
                public static string GetStringHealth()
                {
                    return GetHealth().ToString() +"/" +GetMaxHealth().ToString();
                }
            }
            /// <summary>
            /// 玩家的心的移动速度
            /// </summary>
            static public class HeartSpeed
            {
                private static float heartSpeed=1f;
                /// <summary>
                /// 获取当前心的移动速度
                /// </summary>
                /// <returns></returns>
                public static float GetHeartSpeed()
                {
                    return heartSpeed;
                }
                /// <summary>
                /// 设置
                /// </summary>
                /// <param name="speed"></param>
                public static void SetHeartSpeed(float speed)
                {
                    heartSpeed = speed;
                }
            }
            /// <summary>
            /// 无敌时间，表示玩家受到攻击后的短时间无敌，单位: ms
            /// </summary>
            static public class INV
            {
                private static int inv=0;
                /// <summary>
                /// 获取无敌时间数值，单位: ms
                /// </summary>
                /// <returns></returns>
                public static int GetINV()
                {
                    return inv;
                }
                /// <summary>
                /// 设置玩家的无敌时间数值，一般少数护甲提供此数值
                /// </summary>
                /// <param name="value">数值</param>
                public static void SetINV(int value)
                {
                    inv= value;
                }
            }
            static public class Money
            {
                private static int money;
                /// <summary>
                /// 获取当前金钱数额
                /// </summary>
                /// <returns></returns>
                public static int GetMoneyNum()
                {
                    return money;
                }
                /// <summary>
                /// 更改金钱数量，增加或减少
                /// </summary>
                /// <param name="changeNum">需要增加或减少的数额</param>
                /// <returns>
                /// -1: 发生未知错误<br/>
                /// 0: 更改成功<br/>
                /// 1: 失败，金钱过少，无法进行扣除(金钱不能为负)
                /// </returns>
                public static int ChangeMoneyNum(int changeNum)
                {
                    if (changeNum > 0)
                    {
                        money += changeNum;
                        return 0;
                    }
                    else if (changeNum < 0)
                    {
                        if (money + changeNum < 0)
                        {
                            return 1;
                        }
                        else
                        {
                            money += changeNum;
                            return 0;
                        }
                    }
                    else
                        return -1;
                }
                /// <summary>
                /// 设置金钱数额
                /// </summary>
                /// <param name="setNum">金钱数额</param>
                public static void SetMoneyNum(int setNum)
                {
                    money = setNum;
                }
            }
            static public class MainItem
            {
                private static CanPickedObject handItem = CanPickedObject.none;
                private static List<string[]> handItem_tags = new();

                private static CanPickedObject bodyItem = CanPickedObject.none;
                private static List<string[]> bodyItem_tags = new();

                /// <summary>
                /// 装备的位置
                /// </summary>
                public enum PickedLoc
                {
                    hand,body
                }
                /// <summary>
                /// 将背包内的目标物品装备至指定位置
                /// </summary>
                /// <param name="numID">背包内的物品顺序ID</param>
                /// <param name="ploc">装备至目标位置，通过枚举提供参数</param>
                public static void PickedItem(int numID,PickedLoc ploc)
                {
                    CanPickedObject oldItem=CanPickedObject.none;
                    List<string[]> oldTags=new();
                    switch (ploc)
                    {
                        case PickedLoc.hand:
                            oldItem = handItem;
                            oldTags=handItem_tags;
                            break;
                            case PickedLoc.body:
                            oldItem = bodyItem;
                            oldTags=bodyItem_tags;
                            break;
                    }
                    CanPickedObject newItem=Player.BagItem.GetItem(numID);
                    List<string[]> newTags=Player.BagItem.GetItemTag(numID);

                    Player.BagItem.RemoveItem(numID);
                    if (oldItem != CanPickedObject.none)
                        Player.BagItem.AddItem(oldItem, oldTags);

                    switch (ploc)
                    {
                        case PickedLoc.hand:
                            handItem = newItem;
                            handItem_tags=newTags;
                            break;
                            case PickedLoc.body:
                            bodyItem = newItem;
                            bodyItem_tags=newTags;
                            break;
                    }
                }                

                /// <summary>
                /// 获取指定位置的物品名称ID
                /// </summary>
                /// <param name="ploc">目标装备的位置</param>
                /// <returns></returns>
                public static CanPickedObject GetItem(PickedLoc ploc)
                {
                    switch (ploc)
                    {
                        case PickedLoc.hand:
                            return handItem;
                        case PickedLoc.body:
                            return bodyItem;
                    }
                    return CanPickedObject.none;
                }
                /// <summary>
                /// 获取指定位置的物品Tag标签
                /// </summary>
                /// <param name="ploc">目标装备的位置</param>
                /// <returns></returns>
                public static List<string[]> GetItemTag(PickedLoc ploc)
                {
                    switch (ploc)
                    {
                        case PickedLoc.hand:
                            return handItem_tags;
                            case PickedLoc.body:
                            return bodyItem_tags;
                    }
                    return null;
                }
            }
           static public class BagItem
            {
                public static int MaxBagItemNum = 8;
                private static List<CanPickedObject> bagItems = new();
                private static List<List<string[]>> bagItem_tags = new();
                /// <summary>
                /// 添加背包物品方法
                /// </summary>
                /// <param name="item">添加的物品ID</param>
                /// <returns>
                /// 返回int数值代表背包添加物品是否成功<br/>
                /// 0: 成功添加物品<br/>
                /// 1: 失败，背包已满<br/>
                /// </returns>
                public static int AddItem(CanPickedObject item, List<string[]> tag)
                {
                    if (bagItems.Count <= MaxBagItemNum)
                    {
                        bagItems.Add(item);
                        bagItem_tags.Add(tag);
                        return 0;
                    }
                    else
                        return 1;
                }
                /// <summary>
                /// 获取指定编号的物品名称ID
                /// </summary>
                /// <param name="numID">物品顺序编号</param>
                /// <returns></returns>
                public static CanPickedObject GetItem(int numID)
                {
                    return bagItems[numID];
                }
                /// <summary>
                /// 获取指定编号的物品标签信息
                /// </summary>
                /// <param name="numID">物品顺序编号</param>
                /// <returns></returns>
                public static List<string[]> GetItemTag(int numID)
                {
                    return bagItem_tags[numID];
                }
                /// <summary>
                /// 删除指定编号的物品
                /// </summary>
                /// <param name="numID">物品顺序编号</param>
                /// <returns></returns>
                public static int RemoveItem(int numID)
                {
                    bagItems.RemoveAt(numID);
                    bagItem_tags.RemoveAt(numID);
                    return 0;
                }
                /// <summary>
                /// 获取背包内物品的总数
                /// </summary>
                /// <returns></returns>
                public static int GetLength()
                {
                    return bagItems.Count;
                }
            }
            
            static public class PhoneContacts
            {
                /// <summary>
                /// 联系人枚举
                /// </summary>
                public enum People
                {
                    none,Hgnim
                }
                /// <summary>
                /// 菜单手机内的联系人列表中显示的内容
                /// </summary>
                static List<People> showList = new() { People.Hgnim};

                /// <summary>
                /// 添加指定联系人的枚举信息
                /// </summary>
                /// <param name="people">目标联系人</param>
                /// <returns>
                /// 返回int数值代表添加联系人枚举信息是否成功<br/>
                /// 0: 成功添加目标<br/>
                /// 1: 失败，当前目标已存在<br/>
                /// </returns>
                public static int AddItem(People people)
                {
                    foreach(People p in showList)
                    {
                        if (p == people)
                            return 1;
                    }
                    showList.Add(people);
                    return 0;
                }
                /// <summary>
                /// 获取指定编号的联系人枚举信息
                /// </summary>
                /// <param name="numID">目标的顺序编号</param>
                /// <returns></returns>
                public static People GetItemEnum(int numID)
                {
                    return showList[numID];
                }
                /// <summary>
                /// 删除指定编号的联系人
                /// </summary>
                /// <param name="numID">目标的顺序编号</param>
                /// <returns></returns>
                public static void RemoveItem(int numID)
                {
                    showList.RemoveAt(numID);
                }
                /// <summary>
                /// 获取显示的联系人总数
                /// </summary>
                /// <returns></returns>
                public static int GetLength()
                {
                    return showList.Count;
                }
            }
        }
    }

    static public class ToolClass
    {
        /// <summary>
        /// 查找目标标签所表示的内容
        /// </summary>
        /// <param name="tags">目标的所有标签</param>
        /// <param name="tagHead">需要查询的标签头部</param>
        /// <returns>返回标签的内容。<br/>如果没有找到该标签，则返回null</returns>
        static public string ObjTagFind(List<string[]> tags, CPObjTagHead tagHead)
        {
            return ObjTagFind_Backend(tags,tagHead.ToString());
        }
        /// <summary>
        /// 查找目标标签所表示的内容
        /// </summary>
        /// <param name="tags">目标的所有标签</param>
        /// <param name="tagHead">需要查询的标签头部</param>
        /// <returns>返回标签的内容。<br/>如果没有找到该标签，则返回null</returns>
        static public string ObjTagFind(List<string[]> tags, TagHead tagHead)
        {
            return ObjTagFind_Backend(tags, tagHead.ToString());
        }
        /// <summary>
        /// 查找目标标签所表示的内容
        /// </summary>
        /// <param name="tags">目标的所有标签</param>
        /// <param name="tagHead">需要查询的标签头部</param>
        /// <returns>返回标签的内容。<br/>如果没有找到该标签，则返回null</returns>
        static public string ObjTagFind(List<string[]> tags, ACDataHead tagHead)
        {
            return ObjTagFind_Backend(tags, tagHead.ToString());
        }
        /// <summary>
        /// 查找目标标签所表示的内容
        /// </summary>
        /// <param name="tags">目标的所有标签</param>
        /// <param name="tagHead">需要查询的标签头部</param>
        /// <returns>返回标签的内容。<br/>如果没有找到该标签，则返回null</returns>
        static public string ObjTagFind(List<string[]> tags, ACTagHead tagHead)
        {
            return ObjTagFind_Backend(tags, tagHead.ToString());
        }
        static private string ObjTagFind_Backend(List<string[]> tags,string tagHead)
        {
            foreach (string[] tag in tags)
            {
                if (tag[0] == tagHead)
                    return tag[1];
            }
            return null;
        }


        /// <summary>
        /// 查找目标标签所表示的内容
        /// </summary>
        /// <param name="tags">目标的所有标签</param>
        /// <param name="tagHead">需要更改的标签头部</param>
        /// <param name="value">更改的内容</param>
        /// <returns>找到标签并更改成功后返回true；否则返回false</returns>
        static public bool ObjTagSet(List<string[]> tags, TagHead tagHead,string value)
        {
           return ObjTagSet_Backend(tags, tagHead.ToString(),value);
        }
        /// <summary>
        /// 查找目标标签所表示的内容
        /// </summary>
        /// <param name="tags">目标的所有标签</param>
        /// <param name="tagHead">需要更改的标签头部</param>
        /// <param name="value">更改的内容</param>
        /// <returns>找到标签并更改成功后返回true；否则返回false</returns>
        static public bool ObjTagSet(List<string[]> tags, ACTagHead tagHead, string value)
        {
            return ObjTagSet_Backend(tags, tagHead.ToString(), value);
        }
        static private bool ObjTagSet_Backend(List<string[]> tags, string tagHead,string value)
        {
            try
            {
                foreach (string[] tag in tags)
                {
                    if (tag[0] == tagHead)
                    {
                        tag[1]=value;
                        return true;
                    }
                }
            }
            catch{return false;}
            return false;
        }
        /// <summary>
        /// 碰撞忽略一键设置函数
        /// </summary>
        /// <param name="objs1">对象组1</param>
        /// <param name="objs2">对象组2</param>
        /// <param name="ignore">忽略状态</param>
        static public void IgnoreCollisionControl(GameObject[] objs1, GameObject[] objs2, bool ignore=true)
        {
            static void SetThat(Collider2D c1, Collider2D c2, bool ignore)
            {
                if (c1 != null && c2 != null)
                {
                    Physics2D.IgnoreCollision(c1, c2, ignore);
                    Debug.Log("碰撞忽略调试信息: "+c1.name + ", " + c2.name);
                }
            }
            foreach (GameObject obj1 in objs1)
            {
                Collider2D obj1c = obj1.GetComponent<Collider2D>();
                foreach (GameObject obj2 in objs2)
                {
                    Collider2D obj2c = obj2.GetComponent<Collider2D>();
                    SetThat(obj1c, obj2c, ignore);
                }
            }
        }

        /// <summary>
        /// 一个在子线程中执行Thread.Sleep()函数的动态类<br/>
        /// 通过绑定事件或获取变量返回值来判断计时器是否完成执行
        /// </summary>
        public class SleepTimer
        {
            /// <summary>
            /// 计时器结束时触发该事件<br/>
            /// 注意该方法是在线程内被调用，请注意委托
            /// </summary>
            public event Action TimerOver;
            bool isStart = false;
            /// <summary>
            /// 计时器是否启动
            /// </summary>
            public bool IsStart
            {
                get { return isStart; }
            }
            bool isTimerOver = false;
            /// <summary>
            /// 计时器是否已完成计时
            /// </summary>
            public bool IsTimerOver
            {
                get { return isTimerOver; }
            }
            int sleepTimeValue;
            /// <summary>
            /// 获取当前设置的休眠时间
            /// </summary>
            public int SleepTimeValue
            {
                get { return sleepTimeValue; }
            }
            /// <summary>
            /// 开始计时器，该方法是在子线程中执行Thread.Sleep，不会堵塞主线程进程<br/>
            /// 不能多次调用此方法
            /// </summary>
            /// <param name="sleepTime">设定定时器时间，单位：毫秒</param>
            public void Start(int sleepTime)
            {
                if (!isStart)
                {                    
                    isStart = true;
                    sleepTimeValue = sleepTime;
                    Thread thread = new(() =>
                    {
                        Thread.Sleep(sleepTime);
                        if (!isStop)
                        {
                            isTimerOver = true;
                            TimerOver?.Invoke();
                        }
                    });thread.Start();
                }
            }
            /// <summary>
            /// 是否被中途终止
            /// </summary>
            bool isStop = false;
            /// <summary>
            /// 提前终止计时器，释放实例前最好执行终止函数<br/>计时器终止后无法重新启动，必须重新实例化
            /// </summary>
            public void Stop()
            {
                isStop = true;            
            }
        }

        /// <summary>
        /// 注意，由于其特性目前尚不清楚，不很建议使用<br/>
        /// 动态休眠函数<br/>
        /// 在Unity中的Update方法函数中使用的sleep休眠函数<br/>
        /// Unity中的Update方法是每一帧调用一次，所以需要特殊的sleep函数避免线程堵塞
        /// </summary>
        /// <param name="sleepTime">休眠时间，单位：秒(s)</param>
        /// <returns>休眠时间达到后返回true，否则返回false</returns>
        static public bool UnityUpdateSleep(int sleepTime)
        {
            if (Time.timeSinceLevelLoad % sleepTime < Time.deltaTime)
                return true;
            else return false;
        }
    }
}
