using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _NGG_Script_RotateCube : MonoBehaviour
{
    private float degreePerSecond = 450f;
    private Transform tempTransform;
    public Transform fireHole;
    public void ShotEvent() {
        Debug.Log("S");
    }

    float z;

    public void TestRay() {
        RaycastHit hit;
        
        if (Physics.Raycast(fireHole.position, fireHole.forward * 100f, out hit)) {
            if (hit.collider.tag == "Cube") {
                z += Time.deltaTime * 2000;

                Debug.Log("test");
                hit.transform.rotation = Quaternion.Euler(0,0,z);
                //hit.transform.Rotate(Vector3.back * Time.deltaTime * degreePerSecond);
            }
        }
    }
}
