using static CoreData.CData.Player.BagItem;
using static CoreData.CData.Player;
using System.Threading;
using UnityEngine;
using static CoreData.ToolClass;
using UnityEngine.UI;
using static CoreData.CData;


public class CMenu_InfoBox_objectUI : MonoBehaviour
{
    /// <summary>
    /// ��ʵ��
    /// </summary>
    GameObject cMenu_InfoBox_objectUI_Heart;
    /// <summary>
    /// 8����Ʒ��ѡ�к������ڵ�λ��
    /// </summary>
    Vector2[] cMenu_InfoBox_objectUI_Heart_loc;
    /// <summary>
    /// ����������ť��ѡ������ڵ�λ��
    /// </summary>
    Vector2[] cMenu_InfoBox_objectUI_Heart_loc2;

    AudioSource cMenu_Sound;
    public static AudioClip sound_move;
    public static AudioClip sound_enter;
    private void OnEnable()
    {
        enterWaitLock = true;
        LocID=0;
        selectLevel = 0;
        {
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

        cMenu_InfoBox_objectUI_Heart = gameObject.transform.Find("cMenu_InfoBox_objectUI_Heart").gameObject;
        cMenu_Sound = GameObject.Find("MainCamera/cMenu/cMenu_Sound").GetComponent<AudioSource>();
        {
            {
                float normalX = cMenu_InfoBox_objectUI_Heart.transform.localPosition.x;

                cMenu_InfoBox_objectUI_Heart_loc = new Vector2[8];
                for (int i = 0; i < cMenu_InfoBox_objectUI_Heart_loc.Length; i++)
                {
                    cMenu_InfoBox_objectUI_Heart_loc[i] = new(normalX,
                        gameObject.transform.Find("cMenu_InfoBox_objectUI_obj" + (i + 1).ToString()).localPosition.y);
                }
            }
            {
                float normalY = gameObject.transform.Find("cMenu_InfoBox_objectUI_use").localPosition.y;
                cMenu_InfoBox_objectUI_Heart_loc2 = new Vector2[2]
                {
                    new(-0.371f,normalY),new(0.095f,normalY)
                };
            }
        }
        sound_move=CMenu_Sound.Instance.sound_move;
        sound_enter=CMenu_Sound.Instance.sound_enter;

        if (BagItem.GetLength() > 0)
            cMenu_InfoBox_objectUI_Heart.SetActive(true);
        cMenu_InfoBox_objectUI_Heart.transform.localPosition = cMenu_InfoBox_objectUI_Heart_loc[0];

        for(int i = 0; i < 8; i++)//���б�ֵ
        {
            if(i < BagItem.GetLength())
           gameObject.transform.Find("cMenu_InfoBox_objectUI_obj" + (i + 1).ToString())
                .gameObject.GetComponent<Text>().text=
                GameLang.GetString(ObjTagFind(BagItem.GetItemTag(i),Objects.CPObjTagHead.langID))
                .Replace(" ", "\u00A0");
            else
                gameObject.transform.Find("cMenu_InfoBox_objectUI_obj" + (i + 1).ToString())
                .gameObject.GetComponent<Text>().text = "";//��������б���������
        }
    }

    /// <summary>
    /// ѡ��㼶<br/>
    /// 0: ѡ����Ʒ�Ĳ㼶<br/>
    /// 1: ѡ�������ť�Ĳ㼶
    /// </summary>
    int selectLevel = 0;
    bool upWaitLock = false;
    bool downWaitLock = false;
    bool leftWaitLock = false;
    bool rightWaitLock = false;
    bool enterWaitLock = true;//��Ҫ�ȴ���ʼ�����ֽ������������ȷ��������ʹ��
    bool exitWaitLock=false;
    void Update()
    {
        if (!upWaitLock && selectLevel==0 && GameKey.UpKeyClick())
        {
            upWaitLock = true;
            if (BagItem.GetLength() > 0)
            {
                LocID--;
                cMenu_InfoBox_objectUI_Heart.transform.localPosition = cMenu_InfoBox_objectUI_Heart_loc[LocID];
                cMenu_Sound.PlayOneShot(sound_move);
            }
            Thread thread = new(() =>
            {
                while (KeyClick_ThreadCall(KeyClick_ThreadCall_ID.UpKey))
                    Thread.Sleep(20);
                upWaitLock = false;
            });
            thread.Start();
        }
        else if (!downWaitLock && selectLevel==0 && GameKey.DownKeyClick())
        {
            downWaitLock = true;
            if (BagItem.GetLength() > 0)
            {
                LocID++;
                cMenu_InfoBox_objectUI_Heart.transform.localPosition = cMenu_InfoBox_objectUI_Heart_loc[LocID];
                cMenu_Sound.PlayOneShot(sound_move);
            }
            Thread thread = new(() =>
            {
                while (KeyClick_ThreadCall(KeyClick_ThreadCall_ID.DownKey))
                    Thread.Sleep(20);
                downWaitLock = false;
            });
            thread.Start();
        }
        else if(!leftWaitLock && selectLevel == 1 && GameKey.LeftKeyClick())
        {
            leftWaitLock = true;
            if (BagItem.GetLength() > 0)
            {
                LocID2--;
                cMenu_InfoBox_objectUI_Heart.transform.localPosition = cMenu_InfoBox_objectUI_Heart_loc2[LocID2];
                cMenu_Sound.PlayOneShot(sound_move);
            }
            Thread thread = new(() =>
            {
                while (KeyClick_ThreadCall(KeyClick_ThreadCall_ID.LeftKey))
                    Thread.Sleep(20);
                leftWaitLock = false;
            });
            thread.Start();
        }
        else if(!rightWaitLock && selectLevel == 1 && GameKey.RightKeyClick())
        {
            rightWaitLock = true;
            if (BagItem.GetLength() > 0)
            {
                LocID2++;
                cMenu_InfoBox_objectUI_Heart.transform.localPosition = cMenu_InfoBox_objectUI_Heart_loc2[LocID2];
                cMenu_Sound.PlayOneShot(sound_move);
            }
            Thread thread = new(() =>
            {
                while (KeyClick_ThreadCall(KeyClick_ThreadCall_ID.RightKey))
                    Thread.Sleep(20);
                rightWaitLock = false;
            });
            thread.Start();
        }
        else if(!enterWaitLock && GameKey.OkKeyClick())
        {
            enterWaitLock = true;
            if (BagItem.GetLength() > 0)
            {
                cMenu_Sound.PlayOneShot(sound_enter);
                switch (selectLevel)
                {
                    case 0:
                        LocID2 = 0;
                        cMenu_InfoBox_objectUI_Heart.transform.localPosition = cMenu_InfoBox_objectUI_Heart_loc2[LocID2];                        
                        selectLevel = 1;
                        break;
                    case 1:
                        switch (LocID2)
                        {
                            case 0:
                                CanUseObj_use.UseObject(LocID);
                                break;  
                            case 1:
                                CanUseObj_use.DropObject(LocID);
                                break;
                        }
                        cMenu_InfoBox_objectUI_Heart.transform.localPosition = cMenu_InfoBox_objectUI_Heart_loc[LocID];
                        selectLevel = 0;
                        gameObject.SetActive(false);
                        cMenu_InfoBox_objectUI_Heart.SetActive(false);
                        break;
                }
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
            switch (selectLevel)
            {
                case 0:
                    gameObject.SetActive(false);
                    cMenu_InfoBox_objectUI_Heart.SetActive(false);
                    break;
                case 1:
                    cMenu_InfoBox_objectUI_Heart.transform.localPosition = cMenu_InfoBox_objectUI_Heart_loc[LocID];
                    selectLevel = 0;
                    break;
            }
            
            Thread thread = new(() =>
            {
                while (KeyClick_ThreadCall(KeyClick_ThreadCall_ID.ExitKey))
                    Thread.Sleep(20);
                exitWaitLock =false;
            });
            thread.Start();
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
                    tmp=GameKey.CancelKeyClick();
                    break;
            }
            wait = true;
        }, null);
        while (!wait) ;
        return tmp;
    }
    enum KeyClick_ThreadCall_ID
    {
        UpKey, DownKey,LeftKey,RightKey,EnterKey,ExitKey
    }

    private int locID;
    /// <summary>
    /// ��ǰѡ�е���Ʒλ��ID
    /// </summary>
    int LocID
    {
        get { return locID; }
        set
        {
            if (value >= BagItem.GetLength())
                locID = 0;
            else if (value < 0)
                locID = BagItem.GetLength() - 1;
            else
                locID = value;
            ObjNoteChange();
        }
    }
    private int locID2;
    /// <summary>
    /// ��ǰѡ�еİ�ťλ��ID
    /// </summary>
    int LocID2
    {
        get { return locID2; }
        set
        {
            if (value >= 2)
                locID2 = 0;
            else if (value < 0)
                locID2 = 2-1;
            else
                locID2 = value;
        }
    }

    void ObjNoteChange()
    {
        if (BagItem.GetLength() > 0)
            gameObject.transform.Find("cMenu_InfoBox_objectUI_note")
                .gameObject.GetComponent<Text>().text =
                GameLang.GetString(ObjTagFind(BagItem.GetItemTag(LocID),Objects.CPObjTagHead. langID)+"_note")
                .Replace(" ", "\u00A0");
        else
            //��û����Ʒ��ʱ��˵����ΪĬ��ֵ����������
            gameObject.transform.Find("cMenu_InfoBox_objectUI_note").gameObject.GetComponent<Text>().text = GameLang.GetString("ui_cMenu_InfoBox_objectUI_note_def");
    }
}
