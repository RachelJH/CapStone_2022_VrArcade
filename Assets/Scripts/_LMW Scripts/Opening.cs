using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;


public class Opening : MonoBehaviour
{
    public GameObject playerCam;
    public GameObject openingCam;
    Scene currentScene;
    string sceneName;
    public GameObject[] chk;
    public float speed;
    public float t;
    public int cnt;
    bool cameraResetCheck = false;
    public bool chkCheck = false;

    private void OnEnable()
    {
        
    }

    private void Start()
    {
        speed = 0.7f;
        t = 0.7f;
        cnt = 0;
        playerCam.SetActive(false);
        openingCam.SetActive(true);
        

    }
    private void FixedUpdate()
    {
        if (openingCam != null && openingCam.activeSelf)
        {
            OpeningProcess();
        }
    }

    public void InitOpening() {
        t = 0.7f;
        speed = 0.7f;
        cnt = 0;
        cameraResetCheck = false;

    }

    public void OpeningProcess()
    {
        if (!chkCheck)
        {
            InitOpening();
            chk = new GameObject[3];
            Debug.Log("chk 0, 1, 2");
            Debug.Log("chk[0] = " + chk[0]);
            chk[0] = GameObject.Find("chk1").gameObject;
            chk[1] = GameObject.Find("chk2").gameObject;
            chk[2] = GameObject.Find("chk3").gameObject;
            chkCheck = true;
            
        }

        if (playerCam == null)
            playerCam = GameObject.Find("PlayerC").transform.Find("PlayerController").transform.Find("CameraRig").gameObject;
        if (openingCam == null)
            openingCam = GameObject.Find("Opening Object");
        
            Debug.Log(" chk[0] = GameObject.Find().gameObject");
            
        Vector3 cam = openingCam.transform.position;
        Vector3 chkp;

        if (cnt < 3)
        {
            Debug.Log("chk[].transform.postion :: cnt" + cnt);
            if (chk[cnt] != null) 
                chkp = chk[cnt].transform.position;
            else {
                Debug.Log("chk[].transform.postion :: cnt" + cnt + " null" );
                chkp = chk[cnt].transform.position;
            }
            openingCam.transform.position = Vector3.MoveTowards(cam, Vector3.Lerp(cam, chkp, t), speed);
        }
            if (cnt == 3)
        {
            Invoke("CameraReset", 3);
            
        }
    }
    void CameraReset()
    {
        Debug.Log("CameraReset :: " + cnt);
        openingCam.SetActive(false);
        playerCam.SetActive(true);
        
        if (!cameraResetCheck) { 
             Scene scene = SceneManager.GetActiveScene();
             if(scene.name == "control_scene")
                CreateControlNetworkPlayer();
            GameObject.Find("ScreenFader").SetActive(false);
            cameraResetCheck = true;
        }
        
    }

    void CreateControlNetworkPlayer() {
        GameObject player;

        player = PhotonNetwork.Instantiate(GameManager.instance.RemotePlayerObjectName, Vector3.zero, Quaternion.identity);
        PhotonNetwork.Instantiate("PlayerManager", new Vector3(0f, 0f, 0f), Quaternion.identity, 0);

        BNG.NetworkPlayer np = player.GetComponent<BNG.NetworkPlayer>();
        if (np)
        {
            np.transform.name = "MyRemotePlayer";
            np.AssignPlayerObjects();
            GameObject playerC = GameObject.Find("PlayerC");
            if (PhotonNetwork.IsMasterClient)
            {
                playerC.transform.position = GameManager.instance.controlSpawn[0].position;
            }
            else
            {
                playerC.transform.position = GameManager.instance.controlSpawn[1].position;
            }
        }
    }
}
