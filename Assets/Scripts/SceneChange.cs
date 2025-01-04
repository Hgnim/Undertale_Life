using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    /// <summary>
    /// 调用场景切换，大部分情况为Frisk走到特定位置切换场景，例如进门
    /// </summary>
    /// <param name="scene">目标场景</param>
    /// <param name="friskPoint">切换场景后Frisk的坐标数据</param>
    /// <param name="backgroundMusic">背景音乐设置，如果没有设置则使用默认设置</param>
    public static void FriskGo(string scene,Vector2 friskPoint,AudioClip backgroundMusic=null)
    {
        Thread waitLoadSceneThread;
        waitLoadSceneThread = new(() =>
        {
            ThreadDelegate.QueueOnMainThread((param) => {
                KeyState.cMenuOpenLock = true;
                KeyState.UseObjectInHandsLock = true;
                Frisk_Walk.MoveLock(true);                
                BlackScreen.checkScene = scene;
                GameObject.Find("MainCamera").transform.Find("BlackScreen").gameObject.SetActive(true);
            }, null);
            while (!BlackScreen.blackDone) { Thread.Sleep(10); }
            ThreadDelegate.QueueOnMainThread((param) => {
                GameObject.Find("Frisk").transform.position = friskPoint;                
                SceneManager.LoadScene(scene);
            }, null);
            while (BlackScreen.checkScene != null) { }//等待场景加载完毕
            ThreadDelegate.QueueOnMainThread((param) => {//播放背景音乐
                AudioClip aClip=null;
                if (backgroundMusic != null)
                    aClip = backgroundMusic;
                else
                {
                    switch (scene)
                    {
                        case "bedroom":
                            aClip = BackgroundMusic.Instance.house1;
                            break;
                        case "yard":
                            aClip = BackgroundMusic.Instance.yard;
                            break;
                        case "town":
                            aClip = BackgroundMusic.Instance.town;break;
                    }
                }
                BackgroundMusic.Instance.PlaySound(aClip);
            }, null);
        });
        waitLoadSceneThread.Start();
    }
}
