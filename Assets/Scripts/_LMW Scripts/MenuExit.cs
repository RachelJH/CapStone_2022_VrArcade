using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuExit : MonoBehaviour
{
    public GameObject optionPanel; 
    // Start is called before the first frame update
    public void startScene()
    {
        SceneManager.LoadScene(1);
    }
    
    public void exitScene()
    {
        Application.Quit();
    }

    public void optionOpen()
    {
        optionPanel.SetActive(true);
    }
    public void optionClose()
    {
        optionPanel.SetActive(false);
    }
}
