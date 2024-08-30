using UnityEngine;
using Cinemachine;

public class CameraSetup : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public Transform followTarget;
    public Transform lookAtTarget;

    void Start()
    {
        if (virtualCamera != null)
        {
            virtualCamera.Follow = followTarget;
            virtualCamera.LookAt = lookAtTarget;
        }
    }
}