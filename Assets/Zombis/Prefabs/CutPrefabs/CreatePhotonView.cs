using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class CreatePhotonView : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Awake()
    {
        gameObject.AddComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
