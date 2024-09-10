using UnityEngine;

// The class responsible for zooming in and out of the camera from the protagonist.

public class CameraZoom : MonoBehaviour
{
    private float scrollAmount = 0.25f;

    private void Update() => SetZoom();

    private void SetZoom()
    {
        float scroll = Input.mouseScrollDelta.y;
        scroll = Mathf.Clamp(scroll, -scrollAmount, scrollAmount);
        transform.Translate(scroll * transform.forward, Space.World);
    }
}