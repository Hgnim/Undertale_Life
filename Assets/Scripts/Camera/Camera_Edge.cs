using Cinemachine;
using UnityEngine;

public class Camera_Edge : MonoBehaviour
{
    CinemachineConfiner cameraControl;
    void Start()
    {
        cameraControl = GameObject.Find("MainCameraControl").GetComponent<CinemachineConfiner>();
        cameraControl.m_BoundingShape2D = gameObject.GetComponent<PolygonCollider2D>();
    }
}
