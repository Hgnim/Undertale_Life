using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadEdge_ChangeScene : MonoBehaviour
{
    public string sceneName;
    public Vector2 targetPos;
    /// <summary>
    /// ���Ŀ�곯��
    /// </summary>
    public char targetFacing;

    bool loadSceneLock;//���������л��������ã������ε���
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
