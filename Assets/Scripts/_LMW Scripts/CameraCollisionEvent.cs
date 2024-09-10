using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollisionEvent : MonoBehaviour
{
   
    Opening opening;
    int n;
    private void Start()
    {
        n = GameManager.instance.GetComponent<Opening>().cnt;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(n);
        if (n < 3)
        {
            n++;
            GameManager.instance.GetComponent<Opening>().cnt++;
        }
       
    }
    
}
