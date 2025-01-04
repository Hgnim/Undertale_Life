using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadEdge_ChangeScene : MonoBehaviour
{
    public string sceneName;
    public Vector2 targetPos;
    /// <summary>
    /// 检查目标朝向
    /// </summary>
    public char targetFacing;

    bool loadSceneLock;//用于锁定切换场景调用，避免多次调用
    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.name == "Frisk")
        {
            if (!loadSceneLock && BlackScreen.backDone && Frisk_Walk.ImgFacingCheck(targetFacing, true, false))
            {
                loadSceneLock = true;
                SceneChange.FriskGo(sceneName, targetPos);
            }
        }
    }
}
