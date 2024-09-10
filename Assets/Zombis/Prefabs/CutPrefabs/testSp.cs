using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
public class testSp : MonoBehaviourPun
{
    public GameObject pf;
    public Transform spP;
    int cnt=0;
    // Start is called before the first frame update
    void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            createZ();
        }
        else {
            Debug.Log("¸¶½ºÅÍ¾Æ´Ô");
        }
            
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient && cnt == 0)
        {
            createZ();
        }
        else
        {
            Debug.Log("¸¶½ºÅÍ¾Æ´Ô");
        }
    }

    
    void createZ() {
        cnt++;
        GameObject cZ = PhotonNetwork.Instantiate(pf.gameObject.name, spP.position, spP.rotation);
        testCn cN = cZ.GetComponent<testCn>();
        cN.photonView.RPC("ChangeName", RpcTarget.All);
    }
}
