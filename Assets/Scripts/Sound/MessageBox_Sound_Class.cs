using UnityEngine;

public class MessageBox_Sound_Class : MonoBehaviour
{
    private static MessageBox_Sound_Class instance;

    public static MessageBox_Sound_Class Instance {  get { return instance; } }
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

    public AudioClip sound_TXT1;
}
