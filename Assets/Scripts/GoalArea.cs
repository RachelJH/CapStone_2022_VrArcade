using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GoalArea : MonoBehaviour
{
    PhotonView pv;
    public static bool goal;
    private void Start()
    {
        goal = false;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            goal = true;
            pv = GameObject.Find("MyRemotePlayer").GetComponent<PhotonView>();
            if (pv != null) {
                pv.RPC("EscapeMazeOnServer", RpcTarget.MasterClient);
            }
        }
    }
}
