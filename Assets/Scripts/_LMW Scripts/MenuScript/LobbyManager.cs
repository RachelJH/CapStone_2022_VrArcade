using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1";
    public Text connectionInfoText;
    public Button joinButton;
    public Button selectShootingButton;
    public Button selectActionButton;
    public Button selectControlButton;
    public Button selectMazeButton;
    string buttonName;
    public OptionControl oc;


    private void Start()
    {
        /*
        PhotonNetwork.GameVersion = gameVersion;
        joinButton.interactable = false;
        selectShootingButton.interactable = false;
        selectActionButton.interactable = false;
        selectControlButton.interactable = false;
        selectMazeButton.interactable = false;
        

        connectionInfoText.text = "마스터 서버에 접속 중";
        */
    }
    private void Update()
    {
        /*
        //Debug.Log(PhotonNetwork.CountOfPlayersInRooms);
        if (PhotonNetwork.CountOfPlayersInRooms == 0) // #### player == 2####
        {
            selectShootingButton.interactable = true;
            selectActionButton.interactable = true;
            selectControlButton.interactable = true;
            selectMazeButton.interactable = true;
        }
        else
        {
            selectShootingButton.interactable = false;
            selectActionButton.interactable = false;
            selectControlButton.interactable = false;
            selectMazeButton.interactable = false;
        }
        */
    }

    public void OnMultiPlayButtonClick()
    {
        oc.MatchPanelOpen();
        SceneManager.LoadScene("MainMenu2");

    }

    public void OnQuitButtonClick() {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnMultiPlayButtonClick2()
    {
        oc.MatchPanelOpen();

    }
    public void OnClickShootingSceneButton()
    {
        //SceneManager.LoadScene(6);
        
    }
    public void OnClickActionSceneButton()
    {
        SceneManager.LoadScene("MainMenuOnSingleAction");

    }

    public void OnClickControlSceneButton()
    {
        SceneManager.LoadScene(6);
    }

    public void OnClickMazeSceneButton()
    {
        SceneManager.LoadScene(7);
    }
}
