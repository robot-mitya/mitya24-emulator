using UnityEngine;
using UnityEngine.Assertions;

public class ZoomController : MonoBehaviour
{
    public const float MinFov = 4;
    public const float MaxFov = 80;
    
    [Range(MinFov, MaxFov)]
    public float fov = 30;

    public Camera mainCamera;

    public float zoomSpeedFactor = 0.5f;

    private float _initialFov;

    public float FieldOfView
    {
        get => fov;
        set
        {
            float newValue = Mathf.Clamp(value, MinFov, MaxFov);
            if (!Mathf.Approximately(newValue, fov))
            {
                fov = newValue;
                mainCamera.fieldOfView = newValue;
            }
        }
    }
    
    private void Awake()
    {
        Assert.IsNotNull(mainCamera);
        _initialFov = fov;
        mainCamera.fieldOfView = fov;
    }
    
    private void OnGUI()
    {
        FieldOfView += -Input.mouseScrollDelta.y * zoomSpeedFactor;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(2)) FieldOfView = _initialFov;
    }
}
