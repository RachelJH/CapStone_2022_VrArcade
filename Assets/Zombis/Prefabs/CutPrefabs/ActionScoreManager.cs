using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class ActionScoreManager : MonoBehaviourPunCallbacks
{
    // �÷��̾� - ���ھ������� �ؽ�����
    Dictionary<Photon.Realtime.Player, ActionScoreboardItem> scoreboardItems = new Dictionary<Photon.Realtime.Player, ActionScoreboardItem>();
    [SerializeField]
    GameObject scoreBoardItemPrefab;
    [SerializeField]
    Transform container;
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("AddSBI " + PhotonNetwork.PlayerList);
        //�뿡 �����ϴ� �÷��̾ŭ ���ھ�忡 �÷��̾� �߰�
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Debug.Log("AddSBI");
            AddScoreBoardItem(player);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        //���ο� �÷��̾ �뿡 ����� �÷��̾� ���ھ�忡 �߰�
        AddScoreBoardItem(newPlayer);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        //�÷��̾� ����� ���ھ� ����
        RemoveScoreBoardItem(otherPlayer);
    }

    public void AddScoreBoardItem(Photon.Realtime.Player player)
    {
        ActionScoreboardItem item = Instantiate(scoreBoardItemPrefab, container).GetComponent<ActionScoreboardItem>();
        item.Initialized(player);
        // ��ųʸ� �ؽ�(�÷��̾� = �г���) - Ű(������) ����
        scoreboardItems[player] = item;
    }

    public void RemoveScoreBoardItem(Photon.Realtime.Player player)
    {
        //���ھ�忡 ���ھ� ����
        Destroy(scoreboardItems[player].gameObject);
        scoreboardItems.Remove(player);
    }
}
