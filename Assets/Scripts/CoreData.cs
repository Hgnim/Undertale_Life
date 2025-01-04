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
            /// ��ʰȡ��Ʒ��ID�������ж�
            /// </summary>
            public enum CanPickedObject
            {
                none,
                hoe,
                wheatSeedBag
            }
            /// <summary>
            /// ��ʰȡ��Ʒ��tag��ǩͷ��
            /// </summary>
            public enum CPObjTagHead
            {
                alias,
                langID,
                useMode,
                DF,//����ֵ
                AT,//����ֵ
            }
            //��Щ����ֻ��������Ĭ�ϳ��ֵ�λ�ã�ʰȡ�󽫴���һ�������Ĵ�tag�ĸ��塣
            static public class CPObj_hoe
            {
                /// <summary>
                /// ����Ʒ�Ƿ����
                /// </summary>
                static public bool Exist= true;
                /// <summary>
                /// ����Ʒ��λ�ã�Ĭ�ϸ����ֵ�Ǹ���Ʒ��Ĭ��λ�ã������ɸ��ġ�
                /// </summary>
               static public Vector2 ObjectLocation = new(-0.652f, -0.697f);

                static readonly public List<string[]> defTag = new()
                {
                    new string[]{CPObjTagHead.alias.ToString(),"obj_hoe"},// �����������ж���Դ������Դ�ļ�����ʹ�ñ����ı���
                    new string[]{CPObjTagHead.langID.ToString(),"object_name_hoe"},// �����ļ�ID
                    new string[]{CPObjTagHead.useMode.ToString(), "hand" }// ����Ʒ��ʹ�÷���,hand���ֳ�
                };
            }
            static public class CPObj_wheatSeedBag
            {
                /// <summary>
                /// ����Ʒ�Ƿ����
                /// </summary>
                static public bool Exist = true;
                /// <summary>
                /// ����Ʒ��λ�ã�Ĭ�ϸ����ֵ�Ǹ���Ʒ��Ĭ��λ�ã������ɸ��ġ�
                /// </summary>
                static public Vector2 ObjectLocation = new(-0.821f, -0.697f);

                static readonly public List<string[]> defTag = new()
                {
                    new string[]{CPObjTagHead.alias.ToString(),"obj_wheatSeedBag"},// ����
                    new string[]{CPObjTagHead.langID.ToString(),"object_name_wheatSeedBag"},// �����ļ�ID
                    new string[]{CPObjTagHead.useMode.ToString(), "hand" }// ����Ʒ��ʹ�÷���,hand���ֳ�
                };
            }
        }
        static public class People
        {
            /// <summary>
            /// ��Դ�ļ�·��ResourcesPath
            /// </summary>
            public const string rpFightOpponent = "Images/UI/FightUI/FightOpponent/";
            /// <summary>
            /// �������ID
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
                , maxHealth//���Ѫ��
                , health//Ѫ��
                , opponentImg//������ͼ
                , mercy//���ֵжԶȣ�����0�����ˡ
                    , fightUIBoxLocSize//ս����λ�ô�С�����û�иñ�ǩ��ΪĬ��
                ,attack//������
                    ,inv//������޵�ʱ�䣬��λ: ms
            }
            /// <summary>
            /// ȫдActionControlTagHead<br/>
            /// ���ڿ��ƶ����ж��ı�ǩͷ
            /// </summary>
            public enum ACDataHead
            {
                //Say��Show������׺������Say��������õ����ĶԻ�����˵���Ļ���Show��������öԻ������ķ���
                //��ǩͷ������Լ������ֱ�ţ������������Ĳ�ͬ�Ի�������:normalShow.ToString()+"1"
                //��ǩͷ����������ֱ�ź��ټ�һ��Сдr����������У����򽫰����˳�����
                    startSay//������˵�Ļ�
                    ,startShow//������ķ��������ڵ�һ�غ�
                    ,normalSay//Ĭ��˵�Ļ�
                    ,normalShow//Ĭ�ϵķ���
                    ,attackSay//������ʱ˵�Ļ�
                    ,attackShow//������ʱ�ķ���
                    ,actSay//ʹ���ж�ʱ˵�Ļ�
                    ,actShow//ʹ���ж�ʱ�ķ������˴�����ʹ�ö�ά���ķ�ʽ����ʾ�������һλ�����ж���ţ��ڶ�λ����ͬ�ķ����ȣ�����actShow,ToString()+"2,1"
                    ,mercySay
                    ,mercyShow//ͬ��
                    ,allDoAttackNum//ӵ�еķ��������ķ���������
            }
            public enum ACTagHead
            {
                normalShowID
                    ,attackShowID
                    ,attackID//������ʽID
            }


            static public class DebugDummy
            {
                static readonly public List<string[]> defTag = new()
                {
                    new string[]{TagHead.opponentImg.ToString(), "dummybattle_0" }
                    //��������ж����ͼ��ɣ������������ĺ�����������ͼ���������ϵ��µ�˳�����
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
                /// ��ȡ�������
                /// </summary>
                /// <returns></returns>
                public static string GetName()
                {
                    return name;
                }
                /// <summary>
                /// ��������
                /// </summary>
                /// <param name="name">����</param>
                public static void SetName(string name)
                {
                    name = Name.name;
                }
            }
            /// <summary>
            /// ���DF����ʾ������
            /// </summary>
            static public class Defense
            {
                /// <summary>
                /// LOVE������ķ�����
                /// </summary>
                private static int LVDF=10;
                /// <summary>
                /// ��Ʒ������ķ�����
                /// </summary>
                private static int itemDF;

                /// <summary>
                /// ˢ����Ʒ�ķ���������
                /// </summary>
                public static void UpdateItemDF()
                {
                    if (MainItem.GetItem(MainItem.PickedLoc.body) != CanPickedObject.none)
                    {
                       itemDF=int.Parse(ToolClass.ObjTagFind(MainItem.GetItemTag(MainItem.PickedLoc.body), CPObjTagHead.DF));//��ȡ��Ʒ�ķ������Ա�ǩ
                    }else
                        itemDF = 0;
                }
                /// <summary>
                /// ����LOVE����ķ�������ֵ��ֻ����LOVE����е���
                /// </summary>
                /// <param name="lvdf">��ֵ</param>
                internal static void SetLVDF(int lvdf)
                {
                    LVDF= lvdf;
                }
                /// <summary>
                /// ��ȡ�ȼ�����ķ�������ֵ
                /// </summary>
                /// <returns></returns>
                public static int GetLVDF()
                {
                    return LVDF;
                }
                /// <summary>
                /// ��ȡ��Ʒ����ķ�������ֵ
                /// </summary>
                /// <returns></returns>
                public static int GetItemDF()
                {
                    return itemDF;
                }
                /// <summary>
                /// ��ȡ��ʽ����ķ������ַ�������ʽ��Ϊ:"�ȼ�����ķ���ֵ(��Ʒ����ķ���ֵ)"
                /// </summary>
                /// <returns></returns>
                public static string GetStringDF()
                {
                    return GetLVDF().ToString() + "(" + GetItemDF().ToString() + ")";
                }
            }
            /// <summary>
            /// ���AT����ʾ������
            /// </summary>
            static public class Attack
            {
                /// <summary>
                /// LOVE������Ĺ�����
                /// </summary>
                private static int LVAT=10;
                /// <summary>
                /// ��Ʒ������Ĺ�����
                /// </summary>
                private static int itemAT;

                /// <summary>
                /// ˢ����Ʒ�Ĺ���������
                /// </summary>
                public static void UpdateItemAT()
                {
                    if (MainItem.GetItem(MainItem.PickedLoc.body) != CanPickedObject.none)
                    {
                        string cache = ToolClass.ObjTagFind(MainItem.GetItemTag(MainItem.PickedLoc.hand),CPObjTagHead.AT);
                        if(cache!=null)
                        itemAT = int.Parse(cache);//��ȡ��Ʒ�ķ������Ա�ǩ
                        else
                            itemAT = 0;
                    }
                    else
                        itemAT = 0;
                }
                /// <summary>
                /// ����LOVE����Ĺ�������ֵ��ֻ����LOVE����е���
                /// </summary>
                /// <param name="lvat">��ֵ</param>
                internal static void SetLVAT(int lvat)
                {
                    LVAT = lvat;
                }
                /// <summary>
                /// ��ȡ�ȼ�����Ĺ�������ֵ
                /// </summary>
                /// <returns></returns>
                public static int GetLVAT()
                {
                    return LVAT;
                }
                /// <summary>
                /// ��ȡ��Ʒ����Ĺ�������ֵ
                /// </summary>
                /// <returns></returns>
                public static int GetItemAT()
                {
                    return itemAT;
                }
                /// <summary>
                /// ��ȡ��ʽ����Ĺ������ַ�������ʽ��Ϊ:"�ȼ�����Ĺ�����ֵ(��Ʒ����Ĺ�����ֵ)"
                /// </summary>
                /// <returns></returns>
                public static string GetStringAT()
                {
                    return LVAT.ToString() +"("+ itemAT.ToString()+")";
                }
                /// <summary>
                /// ��ȡ���������ܺ�
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
                    //��ֵ����: ÿһ��һά���鶼����ÿһ��LOVE�ȼ���Ӧ����ֵ
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
                /// ��ȡ����һ��LOVE����Ҫ��EXP
                /// </summary>
                /// <returns>
                /// �������-1�����ʾ������
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
                /// ���Ӿ���ֵ
                /// </summary>
                /// <param name="addNum">���ӵ�����</param>
                public static void AddExp(int addNum)
                {
                    EXP+= addNum;
                    if (EXP >= LVData[LOVE_Value + 1, 3])//�������ֵ�������辭��ֵ��������
                    {
                        LOVE_Value++;
                        Health.SetMaxHealth(LVData[LOVE_Value, 0]);
                        Attack.SetLVAT(LVData[LOVE_Value, 1]);
                        Defense.SetLVDF(LVData[LOVE_Value, 2]);
                    }
                }
                /// <summary>
                /// ��ȡ��ǰLOVEֵ
                /// </summary>
                /// <returns></returns>
                public static int GetLOVE()
                {
                    return LOVE_Value;
                }
                /// <summary>
                /// ���ص�ǰEXPֵ
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
                /// �����������ֵ
                /// </summary>
                /// <param name="maxHealth">�������ֵ</param>
                internal static void SetMaxHealth(int maxHealth)
                {
                    maxHealth =Health.maxHealth;
                }
                /// <summary>
                /// ��ȡ��ǰ�������ֵ
                /// </summary>
                /// <returns></returns>
                public static int GetMaxHealth()
                {
                    return maxHealth;
                }

                private static int health=maxHealth;
                /// <summary>
                /// ���ص�ǰ����ֵ����
                /// </summary>
                /// <returns></returns>
                public static int GetHealth()
                {
                    return health;
                }
                /// <summary>
                /// ���ĵ�ǰ����ֵ���������ӻ����
                /// </summary>
                /// <param name="changNum">��Ҫ���ӻ���ٵ�����</param>
                /// <returns>
                /// -1: ����δ֪����<br/>
                /// 0: ���ĳɹ�<br/>
                /// 1: ʧ�ܣ�����ֵ��С�ڵ���0�����������
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
                            return 1;//����Ҳ����ִ������������������û��ǽ�����Ҫ���ֵĶ�̬�߳�ִ��
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
                /// ���õ�ǰ����ֵ
                /// </summary>
                /// <param name="setNum">���õ�����</param>
                public static void SetHealthNum(int setNum)
                {
                    health=setNum;
                }
                /// <summary>
                /// ��ȡ��ʽ���������ֵ�ַ�������ʽΪ"��ǰ����ֵ/�������ֵ"
                /// </summary>
                /// <returns></returns>
                public static string GetStringHealth()
                {
                    return GetHealth().ToString() +"/" +GetMaxHealth().ToString();
                }
            }
            /// <summary>
            /// ��ҵ��ĵ��ƶ��ٶ�
            /// </summary>
            static public class HeartSpeed
            {
                private static float heartSpeed=1f;
                /// <summary>
                /// ��ȡ��ǰ�ĵ��ƶ��ٶ�
                /// </summary>
                /// <returns></returns>
                public static float GetHeartSpeed()
                {
                    return heartSpeed;
                }
                /// <summary>
                /// ����
                /// </summary>
                /// <param name="speed"></param>
                public static void SetHeartSpeed(float speed)
                {
                    heartSpeed = speed;
                }
            }
            /// <summary>
            /// �޵�ʱ�䣬��ʾ����ܵ�������Ķ�ʱ���޵У���λ: ms
            /// </summary>
            static public class INV
            {
                private static int inv=0;
                /// <summary>
                /// ��ȡ�޵�ʱ����ֵ����λ: ms
                /// </summary>
                /// <returns></returns>
                public static int GetINV()
                {
                    return inv;
                }
                /// <summary>
                /// ������ҵ��޵�ʱ����ֵ��һ�����������ṩ����ֵ
                /// </summary>
                /// <param name="value">��ֵ</param>
                public static void SetINV(int value)
                {
                    inv= value;
                }
            }
            static public class Money
            {
                private static int money;
                /// <summary>
                /// ��ȡ��ǰ��Ǯ����
                /// </summary>
                /// <returns></returns>
                public static int GetMoneyNum()
                {
                    return money;
                }
                /// <summary>
                /// ���Ľ�Ǯ���������ӻ����
                /// </summary>
                /// <param name="changeNum">��Ҫ���ӻ���ٵ�����</param>
                /// <returns>
                /// -1: ����δ֪����<br/>
                /// 0: ���ĳɹ�<br/>
                /// 1: ʧ�ܣ���Ǯ���٣��޷����п۳�(��Ǯ����Ϊ��)
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
                /// ���ý�Ǯ����
                /// </summary>
                /// <param name="setNum">��Ǯ����</param>
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
                /// װ����λ��
                /// </summary>
                public enum PickedLoc
                {
                    hand,body
                }
                /// <summary>
                /// �������ڵ�Ŀ����Ʒװ����ָ��λ��
                /// </summary>
                /// <param name="numID">�����ڵ���Ʒ˳��ID</param>
                /// <param name="ploc">װ����Ŀ��λ�ã�ͨ��ö���ṩ����</param>
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
                /// ��ȡָ��λ�õ���Ʒ����ID
                /// </summary>
                /// <param name="ploc">Ŀ��װ����λ��</param>
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
                /// ��ȡָ��λ�õ���ƷTag��ǩ
                /// </summary>
                /// <param name="ploc">Ŀ��װ����λ��</param>
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
                /// ��ӱ�����Ʒ����
                /// </summary>
                /// <param name="item">��ӵ���ƷID</param>
                /// <returns>
                /// ����int��ֵ�����������Ʒ�Ƿ�ɹ�<br/>
                /// 0: �ɹ������Ʒ<br/>
                /// 1: ʧ�ܣ���������<br/>
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
                /// ��ȡָ����ŵ���Ʒ����ID
                /// </summary>
                /// <param name="numID">��Ʒ˳����</param>
                /// <returns></returns>
                public static CanPickedObject GetItem(int numID)
                {
                    return bagItems[numID];
                }
                /// <summary>
                /// ��ȡָ����ŵ���Ʒ��ǩ��Ϣ
                /// </summary>
                /// <param name="numID">��Ʒ˳����</param>
                /// <returns></returns>
                public static List<string[]> GetItemTag(int numID)
                {
                    return bagItem_tags[numID];
                }
                /// <summary>
                /// ɾ��ָ����ŵ���Ʒ
                /// </summary>
                /// <param name="numID">��Ʒ˳����</param>
                /// <returns></returns>
                public static int RemoveItem(int numID)
                {
                    bagItems.RemoveAt(numID);
                    bagItem_tags.RemoveAt(numID);
                    return 0;
                }
                /// <summary>
                /// ��ȡ��������Ʒ������
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
                /// ��ϵ��ö��
                /// </summary>
                public enum People
                {
                    none,Hgnim
                }
                /// <summary>
                /// �˵��ֻ��ڵ���ϵ���б�����ʾ������
                /// </summary>
                static List<People> showList = new() { People.Hgnim};

                /// <summary>
                /// ���ָ����ϵ�˵�ö����Ϣ
                /// </summary>
                /// <param name="people">Ŀ����ϵ��</param>
                /// <returns>
                /// ����int��ֵ���������ϵ��ö����Ϣ�Ƿ�ɹ�<br/>
                /// 0: �ɹ����Ŀ��<br/>
                /// 1: ʧ�ܣ���ǰĿ���Ѵ���<br/>
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
                /// ��ȡָ����ŵ���ϵ��ö����Ϣ
                /// </summary>
                /// <param name="numID">Ŀ���˳����</param>
                /// <returns></returns>
                public static People GetItemEnum(int numID)
                {
                    return showList[numID];
                }
                /// <summary>
                /// ɾ��ָ����ŵ���ϵ��
                /// </summary>
                /// <param name="numID">Ŀ���˳����</param>
                /// <returns></returns>
                public static void RemoveItem(int numID)
                {
                    showList.RemoveAt(numID);
                }
                /// <summary>
                /// ��ȡ��ʾ����ϵ������
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
        /// ����Ŀ���ǩ����ʾ������
        /// </summary>
        /// <param name="tags">Ŀ������б�ǩ</param>
        /// <param name="tagHead">��Ҫ��ѯ�ı�ǩͷ��</param>
        /// <returns>���ر�ǩ�����ݡ�<br/>���û���ҵ��ñ�ǩ���򷵻�null</returns>
        static public string ObjTagFind(List<string[]> tags, CPObjTagHead tagHead)
        {
            return ObjTagFind_Backend(tags,tagHead.ToString());
        }
        /// <summary>
        /// ����Ŀ���ǩ����ʾ������
        /// </summary>
        /// <param name="tags">Ŀ������б�ǩ</param>
        /// <param name="tagHead">��Ҫ��ѯ�ı�ǩͷ��</param>
        /// <returns>���ر�ǩ�����ݡ�<br/>���û���ҵ��ñ�ǩ���򷵻�null</returns>
        static public string ObjTagFind(List<string[]> tags, TagHead tagHead)
        {
            return ObjTagFind_Backend(tags, tagHead.ToString());
        }
        /// <summary>
        /// ����Ŀ���ǩ����ʾ������
        /// </summary>
        /// <param name="tags">Ŀ������б�ǩ</param>
        /// <param name="tagHead">��Ҫ��ѯ�ı�ǩͷ��</param>
        /// <returns>���ر�ǩ�����ݡ�<br/>���û���ҵ��ñ�ǩ���򷵻�null</returns>
        static public string ObjTagFind(List<string[]> tags, ACDataHead tagHead)
        {
            return ObjTagFind_Backend(tags, tagHead.ToString());
        }
        /// <summary>
        /// ����Ŀ���ǩ����ʾ������
        /// </summary>
        /// <param name="tags">Ŀ������б�ǩ</param>
        /// <param name="tagHead">��Ҫ��ѯ�ı�ǩͷ��</param>
        /// <returns>���ر�ǩ�����ݡ�<br/>���û���ҵ��ñ�ǩ���򷵻�null</returns>
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
        /// ����Ŀ���ǩ����ʾ������
        /// </summary>
        /// <param name="tags">Ŀ������б�ǩ</param>
        /// <param name="tagHead">��Ҫ���ĵı�ǩͷ��</param>
        /// <param name="value">���ĵ�����</param>
        /// <returns>�ҵ���ǩ�����ĳɹ��󷵻�true�����򷵻�false</returns>
        static public bool ObjTagSet(List<string[]> tags, TagHead tagHead,string value)
        {
           return ObjTagSet_Backend(tags, tagHead.ToString(),value);
        }
        /// <summary>
        /// ����Ŀ���ǩ����ʾ������
        /// </summary>
        /// <param name="tags">Ŀ������б�ǩ</param>
        /// <param name="tagHead">��Ҫ���ĵı�ǩͷ��</param>
        /// <param name="value">���ĵ�����</param>
        /// <returns>�ҵ���ǩ�����ĳɹ��󷵻�true�����򷵻�false</returns>
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
        /// ��ײ����һ�����ú���
        /// </summary>
        /// <param name="objs1">������1</param>
        /// <param name="objs2">������2</param>
        /// <param name="ignore">����״̬</param>
        static public void IgnoreCollisionControl(GameObject[] objs1, GameObject[] objs2, bool ignore=true)
        {
            static void SetThat(Collider2D c1, Collider2D c2, bool ignore)
            {
                if (c1 != null && c2 != null)
                {
                    Physics2D.IgnoreCollision(c1, c2, ignore);
                    Debug.Log("��ײ���Ե�����Ϣ: "+c1.name + ", " + c2.name);
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
        /// һ�������߳���ִ��Thread.Sleep()�����Ķ�̬��<br/>
        /// ͨ�����¼����ȡ��������ֵ���жϼ�ʱ���Ƿ����ִ��
        /// </summary>
        public class SleepTimer
        {
            /// <summary>
            /// ��ʱ������ʱ�������¼�<br/>
            /// ע��÷��������߳��ڱ����ã���ע��ί��
            /// </summary>
            public event Action TimerOver;
            bool isStart = false;
            /// <summary>
            /// ��ʱ���Ƿ�����
            /// </summary>
            public bool IsStart
            {
                get { return isStart; }
            }
            bool isTimerOver = false;
            /// <summary>
            /// ��ʱ���Ƿ�����ɼ�ʱ
            /// </summary>
            public bool IsTimerOver
            {
                get { return isTimerOver; }
            }
            int sleepTimeValue;
            /// <summary>
            /// ��ȡ��ǰ���õ�����ʱ��
            /// </summary>
            public int SleepTimeValue
            {
                get { return sleepTimeValue; }
            }
            /// <summary>
            /// ��ʼ��ʱ�����÷����������߳���ִ��Thread.Sleep������������߳̽���<br/>
            /// ���ܶ�ε��ô˷���
            /// </summary>
            /// <param name="sleepTime">�趨��ʱ��ʱ�䣬��λ������</param>
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
            /// �Ƿ���;��ֹ
            /// </summary>
            bool isStop = false;
            /// <summary>
            /// ��ǰ��ֹ��ʱ�����ͷ�ʵ��ǰ���ִ����ֹ����<br/>��ʱ����ֹ���޷�������������������ʵ����
            /// </summary>
            public void Stop()
            {
                isStop = true;            
            }
        }

        /// <summary>
        /// ע�⣬����������Ŀǰ�в���������ܽ���ʹ��<br/>
        /// ��̬���ߺ���<br/>
        /// ��Unity�е�Update����������ʹ�õ�sleep���ߺ���<br/>
        /// Unity�е�Update������ÿһ֡����һ�Σ�������Ҫ�����sleep���������̶߳���
        /// </summary>
        /// <param name="sleepTime">����ʱ�䣬��λ����(s)</param>
        /// <returns>����ʱ��ﵽ�󷵻�true�����򷵻�false</returns>
        static public bool UnityUpdateSleep(int sleepTime)
        {
            if (Time.timeSinceLevelLoad % sleepTime < Time.deltaTime)
                return true;
            else return false;
        }
    }
}
