using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutZombiDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyAfter", 3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DestroyAfter() {
        Destroy(gameObject);
    }

}
