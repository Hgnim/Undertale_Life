using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    //ע�⣬�˴���Ҫʹ�õ�ʵ�����棬��Ϊƽʱ�ö���������״̬����ʵ���洢�����޷��ҵ�����״̬�µĶ���

    /// <summary>
    /// �Ի������������ظ�����
    /// </summary>
    public static bool msgboxLock;
    /// <summary>
    /// ͨ������ǿ�Ƽ����Ի���
    /// </summary>
    public static bool codeContinue=false;
    /// <summary>
    /// ͨ������ǿ���������ֶ���
    /// </summary>
    public static bool codeSkip = false;
   /// <summary>
    /// �����ı��Ѿ�ȫ�������ϣ����ڴ����ж�
    /// </summary>
    public static bool msgOutputDone=false;
    /// <summary>
    /// �����Ի���
    /// <br/>
    /// ��������ʽ
    /// </summary>
    /// <param name="inputValue">���ڶԻ�����������ַ����������������ַ�����Ҳ������ID������ʹ�ö��Ÿ������ID</param>
    /// <param name="waitTime">�Ի����������������֮��ȴ���ʱ�䣬��λ��ms</param>
    /// <param name="waitTime_advanced">���߼��ĶԻ����������������֮��ĵȴ�ʱ�䣬���ȼ�����waitTime����λ: ms<br/>
    /// ʹ�ø�ʽ:<br/>�ɶ�ʹ�ã���һ����������ʼ�ַ���˳����(��0��ʼ��)��<br/>
    /// ����:<br/>�������{0,100,5,200,8,100} �ӵ�0���ַ���ʼ100ms�ȴ�ʱ�䣬����5���ַ���200ms�ȴ�ʱ�䣬����8���ַ���100ms�ȴ�ʱ�䡣�Դ����ơ�</param>
    /// <param name="soundSpeedDown">Unity������������̫���������⣬ͨ���ò���������Ч�Ĳ����ٶȡ�<br/>��ֵԽ�������ٶȾ�Խ����</param>
    /// <param name="isLangKey">inputValue�����Ƿ�Ϊ�����ļ���ID��</param>
    /// <param name="langFile">�����Ϊnull����ʹ�ò���ָ���������ļ���</param>
    /// <param name="keyContinue">�Ƿ������û����°��������Ի��򣬽��ú󣬽��޷�ͨ�����������Ի��ı���</param>
    /// <param name="audio">����Ҫ���ֲ��ŵ���Ч</param>
    /// <param name="prefix">�Զ�����Ϣǰ׺��Ĭ�������Ϊ"* "</param>
    /// <param name="haveNext">�Ƿ���������Ե���һ����Ϣ��<br/>������������ڷ�����󲻻Ὣ��Ϣ��رպ��ͷ����ݵȲ�����</param>
    /// <param name="gameObject_TextUI">ָ���ı����ڵ�UI��壬Ĭ��Ϊ�Ի�����ı�UI����</param>
    /// <param name="gameObject_TextUI_TextObj">ָ���ı�����Ĭ��Ϊ�Ի����ڵ��ı�����</param>
    /// <param name="audioSourceObject">ָ��������Ƶ�Ķ���Ĭ��Ϊ�Ի����ڵ���Ƶ���Ŷ���</param>
    /// <param name="banChangeTextUI">ǿ�ƽ�ֹ�ڷ���������ر�gameObject_TextUI����͸���gameObject_TextUI_TextObj������ı�</param>
    /// <param name="startSound">�Ի�������ʱ���ŵ���Ч</param>
    public void StartMsgBox
        (
        string inputValue,
        int waitTime = 100,
        int[] waitTime_advanced = null,
        int soundSpeedDown = 0,
        bool isLangKey = true,
        string langFile = null,
        bool keyContinue = true,
        AudioClip audio = null,
        string prefix = "* ",
        bool haveNext = false,
        GameObject gameObject_TextUI = null,
        GameObject gameObject_TextUI_TextObj = null,
        AudioSource audioSourceObject = null,
        bool banChangeTextUI = false,
        AudioClip startSound = null
        )
    {
        if (!msgboxLock)
        {
            if(gameObject_TextUI == null)
            gameObject.SetActive(true);
            if(gameObject_TextUI==null)
            gameObject_TextUI= gameObject.transform.Find("MsgBoxUI_OnlyText").gameObject;
            if(gameObject_TextUI_TextObj==null)
            gameObject_TextUI_TextObj = gameObject_TextUI.transform.Find("MsgBoxUI_OnlyText_Text").gameObject;
            Text gameObject_TextUI_TextObj_Text=gameObject_TextUI_TextObj.GetComponent<Text>();
            string msg=prefix;
            msgboxLock = true;
            Frisk_Walk.MoveLock(true);
            KeyState.cMenuOpenLock = true;
            KeyState.UseObjectInHandsLock = true;
            gameObject_TextUI.SetActive(true);
            if(audioSourceObject==null)
            audioSourceObject = gameObject.transform.Find("MessageBox_Sound").gameObject.GetComponent<AudioSource>();
            if (audio == null) 
                 audio= MessageBox_Sound_Class.Instance.sound_TXT1;            

            if (isLangKey) 
            {
                string[] langIDs = inputValue.Split(',');
                if (langFile == null)
                    foreach(string langID in langIDs)
                        msg += GameLang.GetString(langID);                   
                else
                    foreach (string langID in langIDs)
                        msg +=GameLang.GetString(langID, langFile);
            }
            else { msg+=inputValue;}
            msg=msg.Replace(" ", "\u00A0");//����ͨ�ո�ת��Ϊ����Ͽո񣬱����һ���

            int allLength = msg.Length;//�ַ����ܳ���
            //ʹ���߳�ִ����������ͱ����������߳���ʹ��sleep���������������̹߳���
            Thread startThread= new(() =>
            {
                {
                    bool wait=false;//�ȴ��Ի����������
                    ThreadDelegate.QueueOnMainThread((param) => {
                        while (!gameObject_TextUI.activeSelf) { Thread.Sleep(50); }
                        while (!gameObject_TextUI_TextObj.activeSelf) { Thread.Sleep(50); }
                        wait = true;
                    }, null);
                    while (!wait) { Thread.Sleep(50); }
                }
                if (startSound != null)
                {
                    ThreadDelegate.QueueOnMainThread((param) =>
                    {
                        audioSourceObject.PlayOneShot(startSound);
                    }, null);
                }
                {
                    char overCode = 'r';//��������: r(run) a(user aborted) e(normal end)
                    Thread cancelClickThread = new(() =>
                    {
                        bool breakBool = false;
                        bool runLock = false;//ί��ͬ��
                        while (!breakBool)
                        {
                            runLock = true;
                            ThreadDelegate.QueueOnMainThread((param) =>
                            {
                                if (!breakBool)
                                {
                                    if (GameKey.CancelKeyClick() || codeSkip)//���û�����ȡ������ʱ��������ֹ����
                                    {
                                        if (keyContinue || codeSkip)//�Ƿ�����������
                                        {
                                            if (overCode == 'r')
                                                overCode = 'a';
                                            breakBool = true;
                                        }
                                    }
                                    else if (overCode == 'e')
                                    {
                                        breakBool = true;
                                    }
                                }
                                runLock = false;
                            }, null);
                            while (runLock) ;
                        }
                    });
                    cancelClickThread.Start();
                    {
                        bool waitEnd=true;//�ȴ�ί��ִ�н���������������ݲ�ͬ��
                        int lastI=-1;//�����һ���ֺ󣬽���洢�ڸñ����У���ֹ��������ֵֹ������ͬ��
                        {
                            int waitTime_advanced_Level = 0;//�߼��ȴ�ʱ��ȼ����ñ������ڵ������жϲ�ͬ�׶εĵȴ�ʱ��
                            for (int i = 0; i < allLength; i++) //��������ı�
                            {
                                waitEnd = false;
                                ThreadDelegate.QueueOnMainThread((param) =>
                            {
                                if (overCode == 'r')
                                {
                                    lastI = i;
                                    gameObject_TextUI_TextObj_Text.text += msg.Substring(i, 1);
                                    if (soundSpeedDown != 0)
                                    {
                                        if (i % soundSpeedDown != 0)//������㣬������Ч����̫�죬����unity�����޷���Ӧ��Ч
                                            goto skipSound;
                                    }
                                    if(startSound == null)
                                    audioSourceObject.PlayOneShot(audio);
skipSound:;
                                }
                                waitEnd = true;
                            }, null);

                                if (overCode == 'a')//���û���ֹʱ��ֱ�ӿ�����������ֲ�����
                                {
                                    while (!waitEnd) ;
                                    bool waitEnd2 = false;//�ȴ�ί��ִ�н���
                                    ThreadDelegate.QueueOnMainThread((param) =>
                                    {
                                        for (int i2 = lastI + 1; i2 < allLength; i2++)
                                        {
                                            gameObject_TextUI_TextObj_Text.text += msg.Substring(i2, 1);
                                        }
                                        waitEnd2 = true;
                                    }, null);
                                    while (!waitEnd2) ;
                                    break;
                                }
                                if (waitTime_advanced !=null)
                                {
                                    if (waitTime_advanced_Level != -1)//���Ϊ-1���ٽ����ж�
                                    {
                                        if (waitTime_advanced[waitTime_advanced_Level] == i)
                                        {
                                            waitTime = waitTime_advanced[waitTime_advanced_Level + 1];
                                            waitTime_advanced_Level += 2;
                                            if (!(waitTime_advanced.Length >= waitTime_advanced_Level + 2))//�ж������Ƿ������һ��Ԫ�أ������û������ü����ж�
                                                waitTime_advanced_Level = -1;
                                        } 
                                    }
                                }
                                Thread.Sleep(waitTime);
                                while (!waitEnd) ;
                            }
                        }
                        if (overCode == 'r')
                            overCode = 'e';
                    }
                }
                msgOutputDone = true;
                if(keyContinue)
                {//�ȴ��û������Ի�
                    bool clickLock = false;//�����ظ�����
                    while (!clickLock)
                    {                        
                        ThreadDelegate.QueueOnMainThread((param) => {
                            if ((GameKey.OkKeyClick() && !clickLock)||codeContinue)
                            {
                                clickLock = true;
                            }
                        }, null);                       
                        Thread.Sleep(10);
                    }
                }
                else
                {
                    while (!codeContinue) ;//�ȴ���������Ի���
                }
                ThreadDelegate.QueueOnMainThread((param) => {
                    if (!haveNext)
                    {
                        Frisk_Walk.MoveLock(false);
                        KeyState.cMenuOpenLock = false;
                        KeyState.UseObjectInHandsLock = false;
                        if(!banChangeTextUI)
                        gameObject_TextUI.SetActive(false);
                        gameObject.SetActive(false);
                    }
                    if(!banChangeTextUI)
                    gameObject_TextUI_TextObj_Text.text = "";               
                     msgboxLock = false;
                }, null);

                codeContinue = false;
                msgOutputDone=false;
                codeSkip = false;
            });
            startThread.Start();
        }
    }
    /// <summary>
    /// �����Ի���<br/>
    /// ���ּ�ͼ��(�򶯻�)��ʽ<br/>
    /// ���ڡ���������ʽ��
    /// </summary>
    /// <param name="images">�Ի�������ʾ��ͼ��򶯻���<br/>����Ԫ��Ϊ��̬ͼ�񣬶��Ԫ����Ϊ������ʾ</param>
    /// <param name="moreImageShowWaitTime">�Ի�����ͼ�񶯻���ÿ��ͼƬѭ���еĵȴ�ʱ��</param>
    /// 
    /// ����ע����StartMsgBox�ġ���������ʽ��ͬ��
    /// <param name="inputValue">���ڶԻ�����������ַ����������������ַ�����Ҳ������ID������ʹ�ö��Ÿ������ID</param>
    /// <param name="waitTime">�Ի����������������֮��ȴ���ʱ�䣬��λ��ms</param>
    /// <param name="waitTime_advanced">���߼��ĶԻ����������������֮��ĵȴ�ʱ�䣬���ȼ�����waitTime����λ: ms<br/>
    /// ʹ�ø�ʽ:<br/>�ɶ�ʹ�ã���һ����������ʼ�ַ���˳����(��0��ʼ��)��<br/>
    /// ����:<br/>�������{0,100,5,200,8,100} �ӵ�0���ַ���ʼ100ms�ȴ�ʱ�䣬����5���ַ���200ms�ȴ�ʱ�䣬����8���ַ���100ms�ȴ�ʱ�䡣�Դ����ơ�</param>
    /// <param name="soundSpeedDown">Unity������������̫���������⣬ͨ���ò���������Ч�Ĳ����ٶȡ�<br/>��ֵԽ�������ٶȾ�Խ����</param>
    /// <param name="isLangKey">inputValue�����Ƿ�Ϊ�����ļ���ID��</param>
    /// <param name="langFile">�����Ϊnull����ʹ�ò���ָ���������ļ���</param>
    /// <param name="keyContinue">�Ƿ������û����°��������Ի��򣬽��ú󣬽��޷�ͨ�����������Ի��ı���</param>
    /// <param name="audio">����Ҫ���ֲ��ŵ���Ч</param>
    /// <param name="prefix">�Զ�����Ϣǰ׺��Ĭ�������Ϊ"* "</param>
    /// <param name="haveNext">�Ƿ���������Ե���һ����Ϣ��<br/>������������ڷ�����󲻻Ὣ��Ϣ��رպ��ͷ����ݵȲ�����</param>
    /// <param name="startSound">�Ի�������ʱ���ŵ���Ч</param>    
    public void StartMsgBox(
        Sprite[] images,
        //���д󲿷ֲ�����StartMsgBox�ġ���������ʽ��ͬ��
        string inputValue,
        int moreImageShowWaitTime = 100,//�˲���Ϊ���ּ�ͼ����ʽ�����еĲ�������Ϊ�ǿ�ѡ���������Ա�����ں���
        int waitTime = 100,
        int[] waitTime_advanced = null,
        int soundSpeedDown = 0,
        bool isLangKey = true,
        string langFile = null,
        bool keyContinue = true,
        AudioClip audio = null,
        string prefix = "* ",
        bool haveNext = false,
        AudioClip startSound = null
        )
    {
        GameObject gameObject_TextUI = gameObject.transform.Find("MsgBoxUI_FaceText").gameObject;
        GameObject gameObject_TextUI_TextObj = gameObject_TextUI.transform.Find("MsgBoxUI_FaceText_Text").gameObject;
        GameObject gameObject_ImgObj = gameObject_TextUI.transform.Find("MsgBoxUI_FaceText_Img").gameObject;
        Image gameObject_ImgObj_img = gameObject_ImgObj.GetComponent<Image>();
        AudioSource audioSourceObject = gameObject.transform.Find("MessageBox_Sound").gameObject.GetComponent<AudioSource>();
        Thread imgT = new(() =>
        {
            if (images.Length > 0)
            {               
                void givenum0()
                {
                    bool wait = false;
                    ThreadDelegate.QueueOnMainThread((param) =>
                    {
                        gameObject_ImgObj_img.sprite = images[0];                        
                        wait = true;
                    }, null);
                    while (!wait) ;
                }


                //��ʼ������
                {
                    givenum0();//��Ҫ������ͼ���ټ������
                    bool wait = false;
                    ThreadDelegate.QueueOnMainThread((param) =>
                    {
                        gameObject_ImgObj.SetActive(true);
                        wait = true;
                    }, null);
                    while (!wait) ;
                }                
                if (images.Length > 1)
                {
                    while (true)
                    {
                        foreach (Sprite img in images)
                        {
                            bool wait = false;
                            ThreadDelegate.QueueOnMainThread((param) =>
                            {
                                gameObject_ImgObj_img.sprite = img;
                                wait = true;
                            }, null);
                            Thread.Sleep(moreImageShowWaitTime);
                            while (!wait) ;
                            if (msgOutputDone)
                            {
                                givenum0();
                                goto exitWhile;
                            }
                        }
                    }
exitWhile:;
                }
            }
        });imgT.Start();
        StartMsgBox(inputValue, waitTime, waitTime_advanced, soundSpeedDown, isLangKey, langFile, keyContinue, audio, prefix, haveNext
            , gameObject_TextUI, gameObject_TextUI_TextObj, audioSourceObject,false,startSound);
        Thread waitEnd = new(() =>
        {//�ȴ��Ի�����ý�����ͼ�����رգ����ⲻ��Ҫ������
            while (msgboxLock);
                ThreadDelegate.QueueOnMainThread((param) => {
                    gameObject_ImgObj.SetActive(false);
                }, null);
        });waitEnd.Start();
    }

    /// <summary>
    /// ��ָ��������Ŀ��ȡ������<br/>һ�����ڶ��ֲ�ͬ��TextUI����ȫ��������������ز�����TextUI����ֹ������
    /// </summary>
    /// <param name="allTextUI">Ŀ�����<br/>���ò���ΪĬ�ϵ�nullʱ�����ڷ������Զ���ֵΪ���еı���TextUI����</param>
    public void ClearAllTextUI(List<GameObject> allTextUI=null)
    {
        allTextUI??=GetLocalAllTextUI();
        foreach(GameObject textUI in allTextUI)
            textUI.SetActive(false);
    }
    /// <summary>
    /// �������б��ػ�����TextUI����
    /// </summary>
    /// <returns></returns>
    private List<GameObject> GetLocalAllTextUI()
    {
        const string prefix= "MainCamera/MainCamera/MessageBox/MsgBoxUI_";
        string[] allTextUI = new string[] { "OnlyText","FaceText"};
        List<GameObject> output=new();
        foreach (string textUI in allTextUI) {
            GameObject cache = GameObject.Find(prefix+ textUI);
            //Debug.Log(cache);
            if(cache==true)
                output.Add(cache);
                }
        return output;
    }
}
