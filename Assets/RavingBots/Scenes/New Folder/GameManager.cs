using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public int shooterEndScore;
    public int actionEndScore;

    public Transform[] selectSpawn;
    public Transform[] mazeSpawn;
    public Transform[] shooterSpawn;
    public Transform[] actionSpawn;
    public Transform[] controlSpawn;
    public Transform[] resultSpawn;

    public string lastStageName;
    public string winnerUserName;
    public bool endCheck;

    // 외부에서 싱글톤 오브젝트를 가져올때 사용할 프로퍼티
    public static GameManager instance;
    private static GameManager m_instance; // 싱글톤이 할당될 static 변수
    public bool isGameover { get; private set; } // 게임 오버 상태

    public int[] shooter_score;
    public string RemotePlayerObjectName = "RemotePlayer";
    //public bool shooterEnd = false;


    private void Awake()
    {
        
        shooter_score[0] = 0;
        shooter_score[1] = 0;

        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance)
        {

            // 자신을 파괴
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
    }
    private void Update()
    {
        if (!endCheck)
        {
            shooterEndCheck(shooterEndScore);
            mazeEnd();
            ControllEndCheck();
            ActionEndCheck(actionEndScore);
        }

    }

    void shooterEndCheck(int shooterEndScore)
    {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.TryGetValue("kills", out object kills))
                {
                    if ((int)kills >= shooterEndScore)
                    {
                        endCheck = true;
                        Debug.Log(player.NickName + "의 패배");
                        winnerUserName = player.NickName;
                        string otherPlayer = winnerUserName;
                        foreach (Player player1 in PhotonNetwork.PlayerList) {
                            if (player1.NickName != otherPlayer) {
                            winnerUserName = player1.NickName;
                            }
                        }
                        
                        Invoke("ResultSceneLoad", 1f);
                    }
                }
            }
    }

    void mazeEnd()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("mazeE", out object maze1))
            {
                if ((int)maze1 == 1)
                {
                    endCheck = true;
                    Debug.Log(player.NickName + "의 승리");
                    winnerUserName = player.NickName;
                    Invoke("ResultSceneLoad", 1f);
                }
            }
        }

    }

    void ControllEndCheck() {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("controll", out object controll1))
            {
                if ((int)controll1 == 1)
                {
                    endCheck = true;
                    Debug.Log(player.NickName + "의 승리");
                    winnerUserName = player.NickName;
                    Invoke("ResultSceneLoad", 1f);
                }
            }
        }
    }

    void ActionEndCheck(int actionEndScore) {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("zombieKills", out object zombieKills1))
            {
                if ((int)zombieKills1 == actionEndScore)
                {
                    endCheck = true;
                    Debug.Log(player.NickName + "의 승리");
                    winnerUserName = player.NickName;
                    Invoke("ResultSceneLoad", 1f);
                }
            }
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(shooter_score[0]);
            stream.SendNext(shooter_score[1]);
        }
        else
        {
            shooter_score[0] = (int)stream.ReceiveNext();
            shooter_score[1] = (int)stream.ReceiveNext();
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void ResultSceneLoad() {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(5); // 결과 씬 이동
        }
    }

    void SelectSceneLoad()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(0); // 결과 씬 이동
        }
    }



    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {

        if (scene.buildIndex == 8) {
            Destroy(this.gameObject);
        }

        if (scene.buildIndex != 5) { // 결과 씬이 아닌 씬이 로드되면 위너 초기화
            winnerUserName = "";
        }

        if (scene.buildIndex != 4)
        { // 컨트롤 씬이 아니면 Opening 스크립트 끄기
            Opening opening = gameObject.GetComponent<Opening>();
            opening.enabled = false;
        }

        if (scene.buildIndex == 0) { // 선택
            GameObject player;
            player = PhotonNetwork.Instantiate(RemotePlayerObjectName, Vector3.zero, Quaternion.identity);
            PhotonNetwork.Instantiate("PlayerManager", new Vector3(0f, 0f, 0f), Quaternion.identity, 0);

            BNG.NetworkPlayer np = player.GetComponent<BNG.NetworkPlayer>();
            if (np)
            {
                np.transform.name = "MyRemotePlayer";
                np.AssignPlayerObjects();
                GameObject playerC = GameObject.Find("PlayerC");
                if (PhotonNetwork.IsMasterClient)
                {
                    playerC.transform.position = selectSpawn[0].position;
                    
                }
                else
                {
                    playerC.transform.position = selectSpawn[1].position;
                    
                }
            }
        }

        if (scene.buildIndex == 1) // 슈터
        {  // game scene
            GameObject player;
            lastStageName = "SHOOTER";

            player = PhotonNetwork.Instantiate(RemotePlayerObjectName, Vector3.zero, Quaternion.identity);


            PhotonNetwork.Instantiate("PlayerManager", new Vector3(0f, 0f, 0f), Quaternion.identity, 0);

            BNG.NetworkPlayer np = player.GetComponent<BNG.NetworkPlayer>();
            if (np)
            {
                np.transform.name = "MyRemotePlayer";
                np.AssignPlayerObjects();
                GameObject playerC = GameObject.Find("PlayerC");
                if (PhotonNetwork.IsMasterClient)
                {
                    playerC.transform.position = shooterSpawn[0].position;
                    PhotonNetwork.Instantiate("RocketL", shooterSpawn[0].position, Quaternion.identity, 0);
                }
                else {
                    playerC.transform.position = shooterSpawn[1].position;
                    PhotonNetwork.Instantiate("RocketL", shooterSpawn[1].position, Quaternion.identity, 0);
                }
            }
        }

        if (scene.buildIndex == 2) // 액션
        {  // game scene
            GameObject player;
            lastStageName = "ACTION";

            player = PhotonNetwork.Instantiate(RemotePlayerObjectName, Vector3.zero, Quaternion.identity);
            PhotonNetwork.Instantiate("PlayerManager", new Vector3(0f, 0f, 0f), Quaternion.identity, 0);

            BNG.NetworkPlayer np = player.GetComponent<BNG.NetworkPlayer>();
            if (np)
            {
                np.transform.name = "MyRemotePlayer";
                np.AssignPlayerObjects();
                GameObject playerC = GameObject.Find("PlayerC");
                if (PhotonNetwork.IsMasterClient)
                {
                    playerC.transform.position = actionSpawn[0].position;
                }
                else
                {
                    playerC.transform.position = actionSpawn[1].position;
                }
            }
        }

        if (scene.buildIndex == 3) // 미로
        {  // game scene
            GameObject player;
            lastStageName = "MAZE";

            player = PhotonNetwork.Instantiate(RemotePlayerObjectName, Vector3.zero, Quaternion.identity);
            PhotonNetwork.Instantiate("PlayerManager", new Vector3(0f, 0f, 0f), Quaternion.identity, 0);

            BNG.NetworkPlayer np = player.GetComponent<BNG.NetworkPlayer>();
            if (np)
            {
                np.transform.name = "MyRemotePlayer";
                np.AssignPlayerObjects();
                GameObject playerC = GameObject.Find("PlayerC");
                if (PhotonNetwork.IsMasterClient)
                {
                    playerC.transform.position = mazeSpawn[0].position;
                }
                else
                {
                    playerC.transform.position = mazeSpawn[1].position;
                }
            }
        }

        if (scene.buildIndex == 4) // 컨트롤
        {  // game scene
            GameObject player;
            lastStageName = "CONTROL";

            gameObject.GetComponent<Opening>().enabled = true;
            Opening opening = gameObject.GetComponent<Opening>();
            opening.chkCheck = false;
            opening.OpeningProcess();
            /*
            player = PhotonNetwork.Instantiate(RemotePlayerObjectName, Vector3.zero, Quaternion.identity);
            PhotonNetwork.Instantiate("PlayerManager", new Vector3(0f, 0f, 0f), Quaternion.identity, 0);

            BNG.NetworkPlayer np = player.GetComponent<BNG.NetworkPlayer>();
            if (np)
            {
                np.transform.name = "MyRemotePlayer";
                np.AssignPlayerObjects();
                GameObject playerC = GameObject.Find("PlayerC");
                if (PhotonNetwork.IsMasterClient)
                {
                    playerC.transform.position = controlSpawn[0].position;
                }
                else
                {
                    playerC.transform.position = controlSpawn[1].position;
                }
            }
            */
        }

        if (scene.buildIndex == 6) {
            gameObject.GetComponent<Opening>().enabled = true;
            Opening opening = gameObject.GetComponent<Opening>();
            opening.OpeningProcess();
        }
            if (scene.buildIndex == 5)
        { // 결과 씬이 호출되면 프로퍼티 초기화

            Debug.Log("ResultS");
            endCheck = false;

            // game scene
            GameObject player1;
            
            player1 = PhotonNetwork.Instantiate(RemotePlayerObjectName, Vector3.zero, Quaternion.identity);
            PhotonNetwork.Instantiate("PlayerManager", new Vector3(0f, 0f, 0f), Quaternion.identity, 0);

            BNG.NetworkPlayer np = player1.GetComponent<BNG.NetworkPlayer>();
            if (np)
            {
                np.transform.name = "MyRemotePlayer";
                np.AssignPlayerObjects();
                GameObject playerC = GameObject.Find("PlayerC");
                if (PhotonNetwork.IsMasterClient)
                {
                    playerC.transform.position = resultSpawn[0].position;
                }
                else
                {
                    playerC.transform.position = resultSpawn[1].position;
                }
            }

            foreach (Player player in PhotonNetwork.PlayerList)
            {

                if (player.CustomProperties.TryGetValue("kills", out object kills))
                {
                    player.CustomProperties["kills"] = 0;
                }

                if (player.CustomProperties.TryGetValue("mazeE", out object maze1))
                {
                    player.CustomProperties["mazeE"] = 0;
                }

                if (player.CustomProperties.TryGetValue("zombieKills", out object zombieKills1))
                {
                    player.CustomProperties["zombieKills"] = 0;
                  
                }

                if (player.CustomProperties.TryGetValue("controll", out object controll1))
                {
                    player.CustomProperties["controll"] = 0;
                    
                }
            }

            if(PhotonNetwork.IsMasterClient)
            Invoke("SelectSceneLoad", 10f);

        }
    }
}
