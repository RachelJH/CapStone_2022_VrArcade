using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyNetwork : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(GameObject.Find("NetworkManager"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
