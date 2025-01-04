using System;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using static CoreData.CData;
using static CoreData.CData.Player;
using static CoreData.ToolClass;
public class CMenu_InfoBox_phoneUI : MonoBehaviour
{
    /// <summary>
    /// ��ʵ��
    /// </summary>
    GameObject cMenu_InfoBox_phoneUI_Heart;
    /// <summary>
    /// 8����ϵ�˲�ѡ�к������ڵ�λ��
    /// </summary>
    Vector2[] cMenu_InfoBox_phoneUI_Heart_loc;

    AudioSource cMenu_Sound;
    public static AudioClip sound_move;
    public static AudioClip sound_enter;
    private void OnEnable()
    {
        enterWaitLock = true;
        LocID = 0;
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

        cMenu_InfoBox_phoneUI_Heart = gameObject.transform.Find("cMenu_InfoBox_phoneUI_Heart").gameObject;
        cMenu_Sound = GameObject.Find("MainCamera/cMenu/cMenu_Sound").GetComponent<AudioSource>();
        {
                float normalX = cMenu_InfoBox_phoneUI_Heart.transform.localPosition.x;

                cMenu_InfoBox_phoneUI_Heart_loc = new Vector2[8];
                for (int i = 0; i < cMenu_InfoBox_phoneUI_Heart_loc.Length; i++)
                {
                    cMenu_InfoBox_phoneUI_Heart_loc[i] = new(normalX,
                        gameObject.transform.Find("cMenu_InfoBox_phoneUI_obj" + (i + 1).ToString()).localPosition.y);
                }
        }
        sound_move = CMenu_Sound.Instance.sound_move;
        sound_enter = CMenu_Sound.Instance.sound_enter;

        if (PhoneContacts.GetLength() > 0)
            cMenu_InfoBox_phoneUI_Heart.SetActive(true);
        cMenu_InfoBox_phoneUI_Heart.transform.localPosition = cMenu_InfoBox_phoneUI_Heart_loc[0];

        for (int i = 0; i < 11; i++)//���б�ֵ
        {
            if (i < PhoneContacts.GetLength())
                gameObject.transform.Find("cMenu_InfoBox_phoneUI_obj" + (i + 1).ToString())
                     .gameObject.GetComponent<Text>().text =
                     GameLang.GetString("message_phone_"+
                     Enum.GetName(typeof( PhoneContacts.People), PhoneContacts.GetItemEnum(i).GetHashCode())
                     + "_name")
                     .Replace(" ", "\u00A0");
            else
                gameObject.transform.Find("cMenu_InfoBox_phoneUI_obj" + (i + 1).ToString())
                .gameObject.GetComponent<Text>().text = "";//��������б���������
        }
    }

    bool upWaitLock = false;
    bool downWaitLock = false;
    bool enterWaitLock = true;//��Ҫ�ȴ���ʼ�����ֽ������������ȷ��������ʹ��
    bool exitWaitLock = false;
    void Update()
    {
        if (!upWaitLock && GameKey.UpKeyClick())
        {
            upWaitLock = true;
            if (PhoneContacts.GetLength() > 0)
            {
                LocID--;
                cMenu_InfoBox_phoneUI_Heart.transform.localPosition = cMenu_InfoBox_phoneUI_Heart_loc[LocID];
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
        else if (!downWaitLock &&  GameKey.DownKeyClick())
        {
            downWaitLock = true;
            if (PhoneContacts.GetLength() > 0)
            {
                LocID++;
                cMenu_InfoBox_phoneUI_Heart.transform.localPosition = cMenu_InfoBox_phoneUI_Heart_loc[LocID];
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
        else if (!enterWaitLock && GameKey.OkKeyClick())
        {
            enterWaitLock = true;
            ExitTheUI();
            if (PhoneContacts.GetLength() > 0)
            {
                cMenu_Sound.PlayOneShot(sound_enter);
                PhoneCall.Call(PhoneContacts.GetItemEnum(LocID));
                CMenu.closeAllMenuSignal = true;
            }
            Thread thread = new(() =>
            {
                while (KeyClick_ThreadCall(KeyClick_ThreadCall_ID.EnterKey))
                    Thread.Sleep(20);
                enterWaitLock = false;
            });
            thread.Start();
        }
        else if (GameKey.CancelKeyClick() && !exitWaitLock)
        {
            ExitTheUI();
        }
    }
    void ExitTheUI()
    {
        exitWaitLock = true;

        gameObject.SetActive(false);
        cMenu_InfoBox_phoneUI_Heart.SetActive(false);

        Thread thread = new(() =>
        {
            while (KeyClick_ThreadCall(KeyClick_ThreadCall_ID.ExitKey))
                Thread.Sleep(20);
            exitWaitLock = false;
        });
        thread.Start();
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
        UpKey, DownKey, EnterKey, ExitKey
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
            if (value >= PhoneContacts.GetLength())
                locID = 0;
            else if (value < 0)
                locID = PhoneContacts.GetLength() - 1;
            else
                locID = value;
        }
    }
}
internal static class PhoneCall
{
   static MessageBox gameObj_mbox;
    public static void Call(PhoneContacts.People people)
    {
        gameObj_mbox = GameObject.Find("MainCamera").
                                   transform.Find("MessageBox").gameObject.
                                   GetComponent<MessageBox>();
        Thread t = new(() =>
        {
            switch (people)
            {
                case PhoneContacts.People.Hgnim:
                    {
                        FastCall.StartMbox_Call();
                        {
                            bool wait = false;
                            ThreadDelegate.QueueOnMainThread((param) =>
                            {
                                gameObj_mbox.StartMsgBox(
                                    new Sprite[] { GetSpriteResource("Hgnim0"),GetSpriteResource("Hgnim1") },
                                    "�㲦ͨ��һ�����صĵ绰�����Է���Զ�����Ӧ�㡣�����и�����Ϣ���绰��������������:D",
                                    moreImageShowWaitTime: 1400, haveNext: true, isLangKey: false
                                    );
                                wait = true;
                            }, null);
                            while (!wait) ;
                            while (MessageBox.msgboxLock)
                            wait = false;
                            ThreadDelegate.QueueOnMainThread((param) =>
                            {
                                gameObj_mbox.StartMsgBox(
                                    new Sprite[] { GetSpriteResource("Hgnim0") },
                                    "���ǣ�����Զ���ᱻ����������绰����ܿ�����һ���������Ϸ�ƽ���:(",
                                     haveNext: true, isLangKey: false
                                    );
                                wait = true;
                            }, null);
                            while (!wait) ;
                            while (MessageBox.msgboxLock) ;
                        }
                        FastCall.StartMbox_Over();
                        ThreadDelegate.QueueOnMainThread((param) =>
                        { gameObj_mbox.ClearAllTextUI(); }, null);
                    }
                    break;
            }
        });t.Start();
    }
    static Sprite GetSpriteResource(string fileName)
    {
        return Resources.Load<Sprite>("Images/UI/MessageBox/face/" + fileName);
    }
    /// <summary>
    /// ���ٵ���
    /// </summary>
    static class FastCall
    {
        private static void StartMbox(string inputValue,bool haveStartSound,bool haveNext)
        {
            AudioClip startSound = null;
            if(haveStartSound)
            {
                bool wait = false;
                ThreadDelegate.QueueOnMainThread((param) =>
                {
                    startSound = Resources.Load("Sounds/MessageBox/snd_phone") as AudioClip;
                    wait = true;
                }, null);
                while (!wait) ;
            }            
            {
                bool wait = false;
                ThreadDelegate.QueueOnMainThread((param) =>
                {
                    gameObj_mbox.StartMsgBox(
                        inputValue: inputValue,
                        startSound: startSound,
                        haveNext: haveNext
                        );
                    wait = true;
                }, null);
                while (!wait) ;
            }
            while (MessageBox.msgboxLock) ;
        }
       internal static void StartMbox_Call()
        {
            StartMbox("message_phone_key_call",true,true);
        }        
        internal static void StartMbox_Ring()
        {
            StartMbox("message_phone_key_ring", true, true);
        }
        internal static void StartMbox_Over()
        {
            StartMbox("message_phone_key_over",false, false);
        }
    }

}
