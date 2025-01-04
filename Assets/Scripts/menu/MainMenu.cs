using UnityEngine;

public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// �������˵�ʱ���ȷ�����Ƿ��£�������£���Ҫ�ȴ������������ִ����������
    /// </summary>
    bool startEnterKeyLock=true;
    /// <summary>
    /// ��ʼ���������ڿ��Կ�ʼ��������ǰ����Ҫ�ȴ�����������ɡ�������ɲ�ͬ��������
    /// </summary>
    public static bool startLock = true;
    void Start()
    {
        startLock = true;
        if(GameKey.OkKeyClick())
            startEnterKeyLock=true;
        else 
            startEnterKeyLock = false;
        BackgroundMusic.Instance.PlaySound(BackgroundMusic.Instance.menu0);
    }
    bool loadSceneLock;//���������л��������ã������ε���
    void Update()
    {
        if (!startLock)
        {
            if (!startEnterKeyLock)
            {
                if (GameKey.OkKeyClick() && !loadSceneLock)
                {
                    loadSceneLock = true;
                    MainMenu_Check.Check = false;
                    DontDestroyOnLoad(GameObject.Find("Frisk"));
                    DontDestroyOnLoad(GameObject.Find("MainCamera"));
                    DontDestroyOnLoad(GameObject.Find("MainCameraControl"));
                    DontDestroyOnLoad(GameObject.Find("ThreadDelegate"));
                    DontDestroyOnLoad(GameObject.Find("KeyManager"));
                    SceneChange.FriskGo("bedroom", new Vector2((float)0, (float)0));
                    KeyState.cMenuOpenLock = false;
                    KeyState.UseObjectInHandsLock = false;
                }
            }
            else
            {
                if (!GameKey.OkKeyClick())
                    startEnterKeyLock = false;
            }
        }
    }
}
