using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("�κ� �浹");
        if (other.CompareTag("Robot")) {
            Debug.Log("�κ� �浹");
            gameObject.transform.Rotate(Vector3.back * Time.deltaTime * 20f);
        }
    }
}
