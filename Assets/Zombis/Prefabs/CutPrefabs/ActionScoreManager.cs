using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class ActionScoreManager : MonoBehaviourPunCallbacks
{
    // 플레이어 - 스코어보드아이템 해쉬구성
    Dictionary<Photon.Realtime.Player, ActionScoreboardItem> scoreboardItems = new Dictionary<Photon.Realtime.Player, ActionScoreboardItem>();
    [SerializeField]
    GameObject scoreBoardItemPrefab;
    [SerializeField]
    Transform container;
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("AddSBI " + PhotonNetwork.PlayerList);
        //룸에 존재하는 플레이어만큼 스코어보드에 플레이어 추가
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
        //새로운 플레이어가 룸에 입장시 플레이어 스코어보드에 추가
        AddScoreBoardItem(newPlayer);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        //플레이어 퇴장시 스코어 제거
        RemoveScoreBoardItem(otherPlayer);
    }

    public void AddScoreBoardItem(Photon.Realtime.Player player)
    {
        ActionScoreboardItem item = Instantiate(scoreBoardItemPrefab, container).GetComponent<ActionScoreboardItem>();
        item.Initialized(player);
        // 딕셔너리 해쉬(플레이어 = 닉네임) - 키(아이템) 구성
        scoreboardItems[player] = item;
    }

    public void RemoveScoreBoardItem(Photon.Realtime.Player player)
    {
        //스코어보드에 스코어 제거
        Destroy(scoreboardItems[player].gameObject);
        scoreboardItems.Remove(player);
    }
}
