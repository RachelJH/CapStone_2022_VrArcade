using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("·Îº¿ Ãæµ¹");
        if (other.CompareTag("Robot")) {
            Debug.Log("·Îº¿ Ãæµ¹");
            gameObject.transform.Rotate(Vector3.back * Time.deltaTime * 20f);
        }
    }
}
