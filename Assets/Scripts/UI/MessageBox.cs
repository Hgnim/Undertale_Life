using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    //注意，此处不要使用单实例储存，因为平时该对象是隐藏状态，单实例存储调用无法找到隐藏状态下的对象。

    /// <summary>
    /// 对话框锁，避免重复启动
    /// </summary>
    public static bool msgboxLock;
    /// <summary>
    /// 通过代码强制继续对话框
    /// </summary>
    public static bool codeContinue=false;
    /// <summary>
    /// 通过代码强制跳过逐字动画
    /// </summary>
    public static bool codeSkip = false;
   /// <summary>
    /// 代表文本已经全部输出完毕，用于代码判断
    /// </summary>
    public static bool msgOutputDone=false;
    /// <summary>
    /// 启动对话框
    /// <br/>
    /// 仅文字形式
    /// </summary>
    /// <param name="inputValue">将在对话框内输出的字符串。参数可用是字符串，也可以是ID。可以使用逗号隔开多个ID</param>
    /// <param name="waitTime">对话框内文字逐字输出之间等待的时间，单位：ms</param>
    /// <param name="waitTime_advanced">更高级的对话框内文字逐字输出之间的等待时间，优先级高于waitTime，单位: ms<br/>
    /// 使用格式:<br/>成对使用，第一个参数代表开始字符的顺序编号(从0开始数)。<br/>
    /// 例如:<br/>数组参数{0,100,5,200,8,100} 从第0个字符开始100ms等待时间，到第5个字符后200ms等待时间，到第8个字符后100ms等待时间。以此类推。</param>
    /// <param name="soundSpeedDown">Unity引擎声音播放太快会出现问题，通过该参数降低音效的播放速度。<br/>数值越大，声音速度就越慢。</param>
    /// <param name="isLangKey">inputValue参数是否为语言文件的ID。</param>
    /// <param name="langFile">如果不为null，则使用参数指定的语言文件。</param>
    /// <param name="keyContinue">是否允许用户按下按键继续对话框，禁用后，将无法通过按键继续对话文本。</param>
    /// <param name="audio">定义要逐字播放的音效</param>
    /// <param name="prefix">自定义消息前缀，默认情况下为"* "</param>
    /// <param name="haveNext">是否包含连续性的下一条消息。<br/>如果包含，则在方法最后不会将消息框关闭和释放数据等操作。</param>
    /// <param name="gameObject_TextUI">指定文本所在的UI面板，默认为对话框的文本UI对象</param>
    /// <param name="gameObject_TextUI_TextObj">指定文本对象，默认为对话框内的文本对象</param>
    /// <param name="audioSourceObject">指定播放音频的对象，默认为对话框内的音频播放对象</param>
    /// <param name="banChangeTextUI">强制禁止在方法结束后关闭gameObject_TextUI对象和更改gameObject_TextUI_TextObj对象的文本</param>
    /// <param name="startSound">对话框启动时播放的音效</param>
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
            msg=msg.Replace(" ", "\u00A0");//将普通空格转换为不间断空格，避免乱换行

            int allLength = msg.Length;//字符串总长度
            //使用线程执行逐字输出就避免了在主线程内使用sleep函数，避免了主线程挂起
            Thread startThread= new(() =>
            {
                {
                    bool wait=false;//等待对话框启动完毕
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
                    char overCode = 'r';//结束代码: r(run) a(user aborted) e(normal end)
                    Thread cancelClickThread = new(() =>
                    {
                        bool breakBool = false;
                        bool runLock = false;//委托同步
                        while (!breakBool)
                        {
                            runLock = true;
                            ThreadDelegate.QueueOnMainThread((param) =>
                            {
                                if (!breakBool)
                                {
                                    if (GameKey.CancelKeyClick() || codeSkip)//当用户按下取消按键时，触发终止代码
                                    {
                                        if (keyContinue || codeSkip)//是否允许按键操作
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
                        bool waitEnd=true;//等待委托执行结束，避免出现数据不同步
                        int lastI=-1;//输出了一个字后，将其存储在该变量中，防止出现奇奇怪怪的输出不同步
                        {
                            int waitTime_advanced_Level = 0;//高级等待时间等级，该变量用于递增来判断不同阶段的等待时间
                            for (int i = 0; i < allLength; i++) //逐字输出文本
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
                                        if (i % soundSpeedDown != 0)//得余计算，避免音效播放太快，导致unity引擎无法响应音效
                                            goto skipSound;
                                    }
                                    if(startSound == null)
                                    audioSourceObject.PlayOneShot(audio);
skipSound:;
                                }
                                waitEnd = true;
                            }, null);

                                if (overCode == 'a')//当用户终止时，直接快速填充完文字并跳出
                                {
                                    while (!waitEnd) ;
                                    bool waitEnd2 = false;//等待委托执行结束
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
                                    if (waitTime_advanced_Level != -1)//如果为-1则不再进行判断
                                    {
                                        if (waitTime_advanced[waitTime_advanced_Level] == i)
                                        {
                                            waitTime = waitTime_advanced[waitTime_advanced_Level + 1];
                                            waitTime_advanced_Level += 2;
                                            if (!(waitTime_advanced.Length >= waitTime_advanced_Level + 2))//判断数组是否包含下一组元素，如果们没有则禁用继续判断
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
                {//等待用户继续对话
                    bool clickLock = false;//避免重复触发
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
                    while (!codeContinue) ;//等待代码继续对话框
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
    /// 启动对话框<br/>
    /// 文字加图像(或动画)形式<br/>
    /// 基于“仅文字形式”
    /// </summary>
    /// <param name="images">对话框内显示的图像或动画。<br/>单个元素为静态图像，多个元素则为动画显示</param>
    /// <param name="moreImageShowWaitTime">对话框内图像动画，每个图片循环中的等待时间</param>
    /// 
    /// 下列注释与StartMsgBox的“仅文字形式”同步
    /// <param name="inputValue">将在对话框内输出的字符串。参数可用是字符串，也可以是ID。可以使用逗号隔开多个ID</param>
    /// <param name="waitTime">对话框内文字逐字输出之间等待的时间，单位：ms</param>
    /// <param name="waitTime_advanced">更高级的对话框内文字逐字输出之间的等待时间，优先级高于waitTime，单位: ms<br/>
    /// 使用格式:<br/>成对使用，第一个参数代表开始字符的顺序编号(从0开始数)。<br/>
    /// 例如:<br/>数组参数{0,100,5,200,8,100} 从第0个字符开始100ms等待时间，到第5个字符后200ms等待时间，到第8个字符后100ms等待时间。以此类推。</param>
    /// <param name="soundSpeedDown">Unity引擎声音播放太快会出现问题，通过该参数降低音效的播放速度。<br/>数值越大，声音速度就越慢。</param>
    /// <param name="isLangKey">inputValue参数是否为语言文件的ID。</param>
    /// <param name="langFile">如果不为null，则使用参数指定的语言文件。</param>
    /// <param name="keyContinue">是否允许用户按下按键继续对话框，禁用后，将无法通过按键继续对话文本。</param>
    /// <param name="audio">定义要逐字播放的音效</param>
    /// <param name="prefix">自定义消息前缀，默认情况下为"* "</param>
    /// <param name="haveNext">是否包含连续性的下一条消息。<br/>如果包含，则在方法最后不会将消息框关闭和释放数据等操作。</param>
    /// <param name="startSound">对话框启动时播放的音效</param>    
    public void StartMsgBox(
        Sprite[] images,
        //下列大部分参数与StartMsgBox的“仅文字形式”同步
        string inputValue,
        int moreImageShowWaitTime = 100,//此参数为文字加图像形式函数中的参数，因为是可选参数，所以必须放在后面
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


                //初始化对象
                {
                    givenum0();//需要先设置图像，再激活对象
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
        {//等待对话框调用结束后将图像对象关闭，避免不必要的滞留
            while (msgboxLock);
                ThreadDelegate.QueueOnMainThread((param) => {
                    gameObject_ImgObj.SetActive(false);
                }, null);
        });waitEnd.Start();
    }

    /// <summary>
    /// 将指定的所有目标取消激活<br/>一般用于多种不同的TextUI调用全部结束后清除本地残留的TextUI，防止其滞留
    /// </summary>
    /// <param name="allTextUI">目标对象<br/>当该参数为默认的null时，将在方法内自动赋值为所有的本地TextUI对象</param>
    public void ClearAllTextUI(List<GameObject> allTextUI=null)
    {
        allTextUI??=GetLocalAllTextUI();
        foreach(GameObject textUI in allTextUI)
            textUI.SetActive(false);
    }
    /// <summary>
    /// 返回所有本地还存活的TextUI对象
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
