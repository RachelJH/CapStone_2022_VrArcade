using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class testCn : MonoBehaviourPun
{
    PhotonView pv;

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        gameObject.name = gameObject.name.Replace("(Clone)", "");
        gameObject.name = gameObject.name.Replace("(p)", "");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    public void ChangeName() {
        gameObject.name = gameObject.name.Replace("(Clone)", "");
    }
}
