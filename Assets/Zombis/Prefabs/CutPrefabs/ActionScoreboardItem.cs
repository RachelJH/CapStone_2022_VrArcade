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
    {   //�ʱ�ȭ
        userNameText.text = player.NickName; // �г��� ǥ��
        Debug.Log("Initialized ::: " + player.NickName);
        this.player = player; // ����Ÿ���÷��̾� = �����÷��̾� �Ҵ�
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
