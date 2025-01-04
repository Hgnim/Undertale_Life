using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;

    public static BackgroundMusic Instance { get { return instance; } }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public AudioClip menu0;
    public AudioClip house1;
    public AudioClip yard;
    public AudioClip town;
    /// <summary>
    /// 播放指定的背景音乐
    /// </summary>
    /// <param name="ac">指定音乐</param>
    public void PlaySound(AudioClip ac)
    {
        AudioSource meAS =
        gameObject.GetComponent<AudioSource>();
        if(meAS.isPlaying)
            meAS.Stop();
        meAS.clip = ac;
        meAS.Play();
    }
}
