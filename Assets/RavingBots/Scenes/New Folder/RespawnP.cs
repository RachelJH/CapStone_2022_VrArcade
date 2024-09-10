using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RespawnP : MonoBehaviour
{
    PhotonView pv;
    public Transform spawnP;

    private void OnTriggerEnter(Collider other)
    {

        if (other.transform.CompareTag("Player"))
        {

            Debug.Log("DeadZone");
            other.transform.position = spawnP.position;
            /*
            pv = other.transform.root.GetComponent<PhotonView>();
            if (pv != null)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    if (pv.Owner.NickName == "Master")
                    {
                        GameManager.instance.shooter_score[0]++;
                    }
                    else
                    {
                        GameManager.instance.shooter_score[1]++;
                    }

                    Debug.Log("master : " + GameManager.instance.shooter_score[0] + "\n client : " + GameManager.instance.shooter_score[1]);
                }
                else Debug.Log("no master");


            }
            else Debug.Log("pv is null");
            */
        }

        if (other.transform.CompareTag("NetPlayer")) { 

            Debug.Log("NetDeadZone");
            pv = other.transform.root.GetComponent<PhotonView>();
            if (pv != null)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    if (pv.Owner.NickName == "Master")
                    {
                        GameManager.instance.shooter_score[0]++;
                    }
                    else
                    {
                        GameManager.instance.shooter_score[1]++;
                    }

                    Debug.Log("master : " + GameManager.instance.shooter_score[0] + "\n client : " + GameManager.instance.shooter_score[1]);
                }
                else Debug.Log("no master");


            }
            else Debug.Log("pv is null");

        }
    }
}


