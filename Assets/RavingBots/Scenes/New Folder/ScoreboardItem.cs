using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class ScoreboardItem : MonoBehaviourPunCallbacks
{
    Player player;
    //public TMP_Text killsNameText;
    public Text killsNameText;
    //public Text userNameText;

   

    public void Initialized(Player player)
    {   //�ʱ�ȭ
        //userNameText.text = player.NickName; // �г��� ǥ��
        Debug.Log("Initialized ::: " + player.NickName);
        this.player = player; // ����Ÿ���÷��̾� = �����÷��̾� �Ҵ�
        UpdateStats();
    }

    void UpdateStats()
    {
        if (player.CustomProperties.TryGetValue("kills", out object kills))
        {
            //userNameText.text = player.NickName;
            killsNameText.text = kills.ToString();
            //Debug.Log("UpdateStats : " + player.NickName + " :::> " + kills);
            
        }
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer == player)
        {
            if (changedProps.ContainsKey("kills"))
            {
                UpdateStats();
            }
        }
    }
}
