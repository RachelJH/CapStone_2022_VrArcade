using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class ActionScoreboardItem : MonoBehaviourPunCallbacks
{
    Player player;
    //public TMP_Text killsNameText;
    public Text killsNameText;
    public Text userNameText;

    public void Initialized(Player player)
    {   //초기화
        userNameText.text = player.NickName; // 닉네임 표시
        Debug.Log("Initialized ::: " + player.NickName);
        this.player = player; // 리얼타임플레이어 = 로컬플레이어 할당
        UpdateStats();
    }

    void UpdateStats()
    {
        if (player.CustomProperties.TryGetValue("zombieKills", out object zombieKills1))
        {
            userNameText.text = player.NickName;
            killsNameText.text = zombieKills1.ToString();
            Debug.Log(" UpdateStats :::" + (int)zombieKills1);
            //Debug.Log("UpdateStats : " + player.NickName + " :::> " + kills);
        }
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer == player)
        {
            if (changedProps.ContainsKey("zombieKills"))
            {
                Debug.Log(" OnPlayerPropertiesUpdate :::");
                UpdateStats();
            }
        }
    }
}
