using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;
    GameObject controller;
    int kills;
    int zombieKills;
    int maze;
    int controll;

    // Start is called before the first frame update
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void GetKill()
    {
        AudioSource scoreAS = GameObject.Find("SoundScore").GetComponent<AudioSource>();
        scoreAS.Play();
        PV.RPC(nameof(RPC_GetKill), PV.Owner);
        Debug.Log("GetKill");
    }

    public void GetControll() {
        PV.RPC(nameof(RPC_GetControll), PV.Owner);
    }

    public void GetZombieKill() {
        PV.RPC(nameof(RPC_GetZombieKill), PV.Owner);
        Debug.Log("GetZombieKill ::: ");
    }

    public void GetMaze() {
        PV.RPC(nameof(RPC_GetMaze), PV.Owner);
        Debug.Log("GetMaze");
    }

    [PunRPC]
    void RPC_GetControll() {
        controll = 1;
        Debug.Log("RPC_GetMaze ::: ");
        Hashtable hash = new Hashtable();
        hash.Add("controll", controll);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("controll", out object controll1))
            Debug.Log("RPC_GetMaze ::: " + (int)controll1);
    }

    [PunRPC]
    void RPC_GetKill()
    {
        kills++;

        Hashtable hash = new Hashtable();
        hash.Add("kills", kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    [PunRPC]
    void RPC_GetZombieKill() {
        zombieKills++;

        Hashtable hash = new Hashtable();
        hash.Add("zombieKills", zombieKills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("zombieKills", out object zombieKills1))
            Debug.Log("RPC_GetZombieKill ::: " + (int)zombieKills1);
    }

    [PunRPC]
    void RPC_GetMaze() {
        maze = 1;
        Hashtable hash = new Hashtable();
        hash.Add("mazeE", maze);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("mazeE", out object maze1))
            Debug.Log("RPC_GetMaze ::: " + (int)maze1);
    }

    public static PlayerManager Find(Photon.Realtime.Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player);
    }
    
}
