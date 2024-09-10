using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class DeadZoneCheck : MonoBehaviourPun
{
    PhotonView pv;

    private void OnTriggerStay(Collider other)
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EndPoint"))
        {
            Scene scene = SceneManager.GetActiveScene();

            if(scene.name == "maze1" || scene.name == "control_scene")
                pv = GameObject.Find("MyRemotePlayer").GetComponent<PhotonView>();

            if (pv != null)
            {
                Debug.Log("GoalTag");
                if (scene.name == "maze1")
                     pv.RPC("EscapeMazeOnServer", RpcTarget.MasterClient);
                if (scene.name == "control_scene")
                    pv.RPC("GoalOnServer", RpcTarget.MasterClient);
            }
        }

        if (PhotonNetwork.PlayerList.Length == 2) {
            if (other.CompareTag("SceneLoaderShooter"))
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.LoadLevel(1);
                }
            }

            if (other.CompareTag("SceneLoaderAction"))
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.LoadLevel(2);
                }
            }

            if (other.CompareTag("SceneLoaderMaze"))
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.LoadLevel(3);
                }
            }

            if (other.CompareTag("SceneLoaderControll"))
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.LoadLevel(4);
                }
            }


            if (other.CompareTag("DeadZone"))
            {
                pv = GameObject.Find("MyRemotePlayer").GetComponent<PhotonView>();
                if (pv != null)
                {
                    pv.RPC("ScoreUp", RpcTarget.MasterClient, pv.Owner.ToString());
                    pv.RPC("FallOnServer", RpcTarget.MasterClient);
                }
            } 
        }
    }

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
