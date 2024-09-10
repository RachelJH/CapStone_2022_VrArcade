using UnityEngine;

// Capturing game objects representing the surfaces of elementary cube blocks and placing them in the currently rotating wall.
// The surfaces currently in the area of ​​the activated cube face become its children.

public class WallActivation : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Surface"))
            other.transform.SetParent(transform);
    }
}