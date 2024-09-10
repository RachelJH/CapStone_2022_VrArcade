using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakingWall : MonoBehaviour
{
    //bool shoot;
    // Start is called before the first frame update
    void Start()
    {
      //  InvokeRepeating("Shoot", .05f, .05f);
    }

    // Update is called once per frame
    void Update()
    {
       /* if (Input.GetButtonDown("XRI_Right_TriggerButton"))
        {
            shoot = true;
        }
        else if (Input.GetButtonUp("XRI_Right_TriggerButton"))
        {
            shoot = false;
        }
       */
    }
    /*
    public void Shoot()
    {
        if (shoot)
        {
            RaycastHit hit = new RaycastHit();
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
            Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.green);
            if (Physics.Raycast(ray, out hit))
            {
                ////	SEND A MESSAGE DAMAGING THE OBJECT HIT
                hit.collider.gameObject.SendMessage("Damage", 1f, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
       */
    public void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Wall" && collision != null)
        {
            collision.gameObject.SendMessage("Damage", 1f, SendMessageOptions.DontRequireReceiver);
           Destroy(collision.gameObject, 3f);
        }
    }



}
