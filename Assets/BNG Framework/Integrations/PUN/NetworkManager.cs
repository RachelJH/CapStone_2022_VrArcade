#if PUN_2_OR_NEWER
using Photon.Pun;
using Photon.Realtime;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace BNG {
    public class NetworkManager :
#if PUN_2_OR_NEWER
MonoBehaviourPunCallbacks 
#else
        MonoBehaviour
#endif

{


        /// <summary>
        /// Maximum number of players per room. If the room is full, a new radom one will be created.
        /// </summary>
        [Tooltip("Maximum number of players per room. If the room is full, a new random one will be created. 0 = No Max.")]
        [SerializeField]
        private byte maxPlayersPerRoom = 0;

        [Tooltip("If true, the JoinRoomName will try to be Joined On Start. If false, need to call JoinRoom yourself.")]
        public bool JoinRoomOnStart = true;

        [Tooltip("If true, do not destroy this object when moving to another scene")]
        public bool dontDestroyOnLoad = true;

        public string JoinRoomName = "RandomRoom";

        [Tooltip("Game Version can be used to separate rooms.")]
        public string GameVersion = "1";

        [Tooltip("Name of the Player object to spawn. Must be in a /Resources folder.")]
        public string RemotePlayerObjectName = "RemotePlayer";

        [Tooltip("Optional GUI Text element to output debug information.")]
        public Text DebugText;

        ScreenFader sf;
#if PUN_2_OR_NEWER
        Scene scene;

        void Awake() {
            scene = SceneManager.GetActiveScene();
            // Required if you want to call PhotonNetwork.LoadLevel() 
            PhotonNetwork.AutomaticallySyncScene = true;

            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(this.gameObject);
            }

            if (Camera.main != null) {
                sf = Camera.main.GetComponentInChildren<ScreenFader>(true);
            }
            
        }

        void Start() {
            
        }

        public void Connect() {
            // Connect to Random Room if Connected to Photon Server
            if (PhotonNetwork.IsConnected)
            {
                if (JoinRoomOnStart)
                {
                    LogText("Joining Room : " + JoinRoomName);
                    PhotonNetwork.JoinRoom(JoinRoomName);
                }
            }
            // Otherwise establish a new connection. We can then connect via OnConnectedToMaster
            else
            {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = GameVersion;
            }
        }

        void Update() {
            // Show Loading Progress
            if (PhotonNetwork.LevelLoadingProgress > 0 && PhotonNetwork.LevelLoadingProgress < 1) {
                Debug.Log(PhotonNetwork.LevelLoadingProgress);
            }

            

            if (scene.name == "MainMenu") {
                Debug.Log("Destroy");
                Destroy(this);
            }
        }

        public override void OnJoinRoomFailed(short returnCode, string message) {
            LogText("Room does not exist. Creating <color=yellow>" + JoinRoomName + "</color>");
            PhotonNetwork.CreateRoom(JoinRoomName, new RoomOptions { MaxPlayers = maxPlayersPerRoom }, TypedLobby.Default);
        }

        public override void OnJoinRandomFailed(short returnCode, string message) {
            Debug.Log("OnJoinRandomFailed Failed, Error : " + message);
        }

        public override void OnConnectedToMaster() {

            LogText("Connected to Master Server. \n");

            if (JoinRoomOnStart) {
                LogText("Joining Room : <color=aqua>" + JoinRoomName + "</color>");
                PhotonNetwork.JoinRoom(JoinRoomName);
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer) {
            base.OnPlayerEnteredRoom(newPlayer);

            float playerCount = PhotonNetwork.IsConnected && PhotonNetwork.CurrentRoom != null ? PhotonNetwork.CurrentRoom.PlayerCount : 0;
            
            LogText("Connected players : " + playerCount);
            
        }

        public override void OnJoinedRoom() {
            LogText("Joined Room. Creating Remote Player Representation.");

            // 닉네임 설정 임시 마/클로 나눔
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.NickName = "Master";

                PlayerPrefs.SetString("USER_ID", "Master");
                
            }
            else
            {
                PhotonNetwork.NickName = "Client";
                PlayerPrefs.SetString("USER_ID", "Client");
                
            }

            /*
            GameObject player;
            // Network Instantiate the object used to represent our player. This will have a View on it and represent the player
            //player = PhotonNetwork.Instantiate(RemotePlayerObjectName, spawnP.position, Quaternion.identity, 0);
            player = PhotonNetwork.Instantiate(RemotePlayerObjectName, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);

            //플레이어 매니저 추가
            PhotonNetwork.Instantiate("PlayerManager", new Vector3(0f, 0f, 0f), Quaternion.identity, 0);

            NetworkPlayer np = player.GetComponent<NetworkPlayer>();
            Scene scene = SceneManager.GetActiveScene();

            if (np) {
                np.transform.name = "MyRemotePlayer";
                np.AssignPlayerObjects();
                
                // 닉네임 설정 임시 마/클로 나눔
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.NickName = "Master";
                    
                    PlayerPrefs.SetString("USER_ID", "Master");
                    playerC.transform.position = spawnP1.position;
                    if(scene.name == "Shooter") PhotonNetwork.Instantiate("RocketL", spawnP1.position, Quaternion.identity, 0);
                }
                else
                {
                    PhotonNetwork.NickName = "Client";
                    PlayerPrefs.SetString("USER_ID", "Client");
                    playerC.transform.position = spawnP2.position;
                    if (scene.name == "Shooter") PhotonNetwork.Instantiate("RocketL", spawnP2.position, Quaternion.identity, 0);
                }
            }
            */

            PhotonNetwork.LoadLevel("MultiSelectScene");

            ScoreManager sm = (ScoreManager)FindObjectOfType(typeof(ScoreManager));
            ActionScoreManager asm = (ActionScoreManager)FindObjectOfType(typeof(ActionScoreManager));
            if (sm != null)
            {
                foreach (Player nplayer in PhotonNetwork.PlayerList)
                {
                    Debug.Log("AddSBI");
                    sm.AddScoreBoardItem(nplayer);
                }
            }
            else if (asm != null) {
                foreach (Player nplayer in PhotonNetwork.PlayerList)
                {
                    Debug.Log("AddSBI");
                    asm.AddScoreBoardItem(nplayer);
                }
            }
            
        }

        public override void OnDisconnected(DisconnectCause cause) {
            LogText("Disconnected from PUN due to cause : " + cause);

            if (!PhotonNetwork.ReconnectAndRejoin()) {
                LogText("Reconnect and Joined.");
            }

            base.OnDisconnected(cause);
        }

        public void LoadScene(string sceneName) {
            // Fade Screen out
            StartCoroutine(doLoadLevelWithFade(sceneName));
        }

        IEnumerator doLoadLevelWithFade(string sceneName) {

            if (sf) {
                sf.DoFadeIn();
                yield return new WaitForSeconds(sf.SceneFadeInDelay);
            }

            PhotonNetwork.LoadLevel(sceneName);

            yield return null;
        }

        void LogText(string message) {

            // Output to worldspace to help with debugging.
            if (DebugText) {
                DebugText.text += "\n" + message;
            }

            Debug.Log(message);
        }

#endif
    }
}

