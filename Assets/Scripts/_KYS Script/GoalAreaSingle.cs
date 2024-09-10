using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalAreaSingle : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Application.LoadLevel("MainMenu");
        }
    }
}
