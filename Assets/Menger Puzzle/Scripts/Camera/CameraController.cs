using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera mainCam;
    private ViewRotation vr;
    private bool rotAllowed;

    private void Awake()
    {
        vr = new ViewRotation(10);
        mainCam = Camera.main;
    }

    private void Update() => RotateCamera();

    private void RotateCamera()
    {
        if (Input.GetMouseButtonDown(1)) rotAllowed = true;
        if (Input.GetMouseButtonUp(1)) rotAllowed = false;

        if (rotAllowed)
            vr.OrbitCamera(transform.position);
    }
}