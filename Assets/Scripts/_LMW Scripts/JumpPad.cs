using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public LayerMask layerMask;
    public float searchPlayer;
    

    [Range(100, 10000)]
    public float bounceheight;
    /*private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Boom Player Check1");
        Collider[] colliders = Physics.OverlapSphere(transform.position, searchPlayer, layerMask);
        foreach (Collider collider in colliders)
        {
            if (collider != null)
            {
                Vector3 dir = collider.transform.position - transform.position;
                Debug.Log("Boom Player Check1");

                Debug.Log(dir + " / " + transform.position + " / " + collider.transform.position);
                NggImpactReceiver impacter = collider.GetComponent<NggImpactReceiver>();
                Debug.Log("Boom" + collider.name);
                impacter.AddImpact(dir, bounceheight);
            }
        }
    }*/
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Boom Player Check1");
        Collider[] colliders = Physics.OverlapSphere(transform.position, searchPlayer, layerMask);
        foreach (Collider collider in colliders)
        {
            if (collider != null)
            {
                //Vector3 dir = collider.transform.position - transform.position;
                //Debug.Log("Boom Player Check1");
                // Debug.Log(dir + " / " + transform.position + " / " + collider.transform.position);
                NggImpactReceiver1 impacter = collider.GetComponent<NggImpactReceiver1>();
                
                
                //Debug.Log("Boom" + collider.name);
                impacter.AddImpact(Vector3.up, bounceheight);
            }
        }
    }

}
