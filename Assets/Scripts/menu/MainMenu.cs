using UnityEngine;

public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// 进入主菜单时检查确定键是否按下，如果按下，需要等待按键弹起后再执行其它任务
    /// </summary>
    bool startEnterKeyLock=true;
    /// <summary>
    /// 开始任务锁，在可以开始进行任务前，需要等待其它步骤完成。避免造成不同步等问题
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
    bool loadSceneLock;//用于锁定切换场景调用，避免多次调用
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
