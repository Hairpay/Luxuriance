using UnityEngine;

/// <summary>
/// Defines complete behaviour of the camera
/// </summary>
public class CameraBehaviour : MonoBehaviour
{
    private Transform _cameraTransform;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        _cameraTransform.position = this.transform.position;
    }
}