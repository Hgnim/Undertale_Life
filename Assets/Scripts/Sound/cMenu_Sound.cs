using UnityEngine;

public class CMenu_Sound : MonoBehaviour
{
    private static CMenu_Sound instance;

    public static CMenu_Sound Instance { get { return instance; } }
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

    public AudioClip sound_move;
    public AudioClip sound_enter;
    private void OnEnable()
    {
        CMenu.sound_enter = sound_enter;
        CMenu.sound_move = sound_move;
        gameObject.GetComponent<AudioSource>().PlayOneShot(sound_move);
    }
}
