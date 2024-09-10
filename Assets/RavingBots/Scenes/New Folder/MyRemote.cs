using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MyRemote : MonoBehaviourPun
{
    PhotonView pv;
    int kills;
    /*
    [PunRPC]
    private void FallProcessOnServer(PhotonMessageInfo info)
    {
        Debug.Log("RPC?");
        MyRemote.Find(info.Sender).GetKill();
    }

    public void GetKill()
    {
        pv.RPC(nameof(RPC_GetKill), RpcTarget.MasterClient);
        Debug.Log("GetKill");
    }

    [PunRPC]
    void RPC_GetKill()
    {
        kills++;

        Hashtable hash = new Hashtable();
        hash.Add("kills", kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        Debug.Log("RPC_GetKill :: " + PhotonNetwork.LocalPlayer.NickName + " :: " + kills);
    }
    */
    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    [PunRPC]
    void ScoreUp(string ownerName)
    {
        Debug.Log("ScoreUp " + ownerName);
        if (ownerName == "#01 'Master'") {
            Debug.Log("M");
            GameManager.instance.shooter_score[0]++;
        }
        if (ownerName == "#02 'Client'") {
            GameManager.instance.shooter_score[1]++;
            Debug.Log("C"); }
        Debug.Log(GameManager.instance.shooter_score[0] + " " + GameManager.instance.shooter_score[1]);
    }

    [PunRPC]
    private void FallOnServer(PhotonMessageInfo info)
    {
        PlayerManager.Find(info.Sender).GetKill();
    }

    [PunRPC]
    private void KillZombieOnServer(PhotonMessageInfo info) {
        PlayerManager.Find(info.Sender).GetZombieKill();

    }

    [PunRPC]
    private void EscapeMazeOnServer(PhotonMessageInfo info) {
        PlayerManager.Find(info.Sender).GetMaze();
    }

    [PunRPC]
    private void GoalOnServer(PhotonMessageInfo info) {
        Debug.Log(":::GOS");
        PlayerManager.Find(info.Sender).GetControll();
    }

    public static MyRemote Find(Photon.Realtime.Player player)
    {
        return FindObjectsOfType<MyRemote>().SingleOrDefault(x => x.pv.Owner == player);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeadZone")) {
            Debug.Log("DZ");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
