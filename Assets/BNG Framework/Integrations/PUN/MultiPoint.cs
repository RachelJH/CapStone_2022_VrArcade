using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiPoint : MonoBehaviourPun
{
    public Transform s1, s2;
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            gameObject.transform.position = s1.position;
        else
            gameObject.transform.position = s2.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
