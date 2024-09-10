using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NggWeaponSpawn : MonoBehaviour
{

    public GameObject weapon;
    public Transform weaponSpawner;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {   Destroy(other);
            Instantiate(weapon, weaponSpawner);
        }
    }
}
