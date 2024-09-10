using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class ResultPrint : MonoBehaviourPun
{
    public TMP_Text stageName;
    public TMP_Text winnerName;
    public TMP_Text myName;

    // Start is called before the first frame update
    void Start()
    {
        stageName.text = GameManager.instance.lastStageName;
        winnerName.text = GameManager.instance.winnerUserName;
        myName.text = PhotonNetwork.LocalPlayer.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
