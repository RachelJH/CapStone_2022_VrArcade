using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ActionSingleCheck : MonoBehaviour
{
    int kills;
    public int singleScore;
    public Text singleScoreText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (kills >= singleScore) {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void GetKill() {
        kills++;
        singleScoreText.text = kills.ToString();
    }
}
