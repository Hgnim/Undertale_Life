using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    /// <summary>
    /// ���ó����л����󲿷����ΪFrisk�ߵ��ض�λ���л��������������
    /// </summary>
    /// <param name="scene">Ŀ�곡��</param>
    /// <param name="friskPoint">�л�������Frisk����������</param>
    /// <param name="backgroundMusic">�����������ã����û��������ʹ��Ĭ������</param>
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
            while (BlackScreen.checkScene != null) { }//�ȴ������������
            ThreadDelegate.QueueOnMainThread((param) => {//���ű�������
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
