using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NggImpactReceiver : MonoBehaviour
{
    float mass = 3.0F; // defines the character mass
    Vector3 impact = Vector3.zero;
    private CharacterController character;
    // Use this for initialization
    void Start()
    {
        character = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // apply the impact force:
        if (impact.magnitude > 0.2F) {
            character.Move(impact * Time.deltaTime);
            Debug.Log("Boom Player Check3");
        }
        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
    }
    // call this function to add an impact force:
    public void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        Debug.Log("dir.y : " +  dir.y);

        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground

        impact += dir.normalized * force / mass;
        Debug.Log("Boom Player Check2 dir" + dir);
        Debug.Log("Boom Player Check2 dir.N" + dir.normalized);
        Debug.Log("Boom Player Check2 impact+= :" + dir.normalized * 1000 / mass);
        Debug.Log("Boom Player Check2 impact: " + impact);
        Debug.Log("Boom Player Check2 impact.magnitude : " + impact.magnitude);
    }
}
