using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameResult : MonoBehaviourPun
{

    public GameObject p1Wins;
    public GameObject p1Defeats;
    public GameObject p2Wins;
    public GameObject p2Defeats;

    void Start()
    {

    }


    void Update()
    {
        if (GoalArea.goal)
        {
            
            p1Wins.SetActive(true);
            p2Defeats.SetActive(true);

        }
    }

}
