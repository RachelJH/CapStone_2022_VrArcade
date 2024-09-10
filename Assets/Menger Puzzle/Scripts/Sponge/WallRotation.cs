using UnityEngine;
using System.Collections;
using System;

// Carrying out the rotation process of the currently selected cube face.

public class WallRotation : MonoBehaviour
{
    public static event Action OnWallRotated = delegate { };

    private Quaternion destRot;
    private bool notallowed;
    private float speed = 0.075f;                    // rotation speed

    public void RotateCube(int sign, Vector3 axis, GameObject wall)   // direction and axis of the rotation
    {
        if (!notallowed && wall != null)
        {
            notallowed = true;
            wall.SetActive(true);               // at this point, the OnTriggerEnter () event for this wall fires

            Quaternion deltaRot = Quaternion.Euler(sign * 90 * axis);
            destRot = wall.transform.rotation * deltaRot;               // modification of the current rotation

            StartCoroutine(Rotate(wall));
        }
    }

    private IEnumerator Rotate(GameObject wall)
    {
        yield return new WaitForSeconds(0.1f);
        SoundManager.GetSoundEffect(1, 0.25f);

        while (wall.transform.rotation != destRot)
        {
            wall.transform.rotation = Quaternion.Lerp(wall.transform.rotation, destRot, speed);
            yield return null;
        }

        wall.transform.rotation = destRot;
        OnWallRotated();
        notallowed = false;
        
        wall.GetComponent<BoxCollider>().enabled = false;
        wall.transform.DetachChildren();
        yield return null;
        wall.GetComponent<BoxCollider>().enabled = true;
        wall.SetActive(false);

        InputController.allowed = true;
    }
}