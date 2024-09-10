using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;



public class OptionControl : MonoBehaviour
{
    public AudioMixer masterVolume;
    public Button playButton;
    public Button closeButton;
    public Button exitButton;
    public GameObject mainPanel;
    public GameObject machingPanel;
    public GameObject scenePanel;
    public GameObject soloPanel;
    //Color ambientDarkest = new Color(169,169,169);
    //Color ambientLightest = new Color(211, 211, 211);

    public void MatchPanelOpen() 
    {
        mainPanel.SetActive(false);
        machingPanel.SetActive(true);
        
    }
    public void MatchPanelClose()
    {
        mainPanel.SetActive(true);
        machingPanel.SetActive(false);
        
    }
  
    public void ScenePanelOpen()
    {
        scenePanel.SetActive(true);
        machingPanel.SetActive(false);
        
    }   
    public void ScenePanelClose()
    {
        machingPanel.SetActive(true);
        scenePanel.SetActive(false);
        
    }
    public void SoloPanelOpen()
    {
        mainPanel.SetActive(false);
        soloPanel.SetActive(true);
        
    }
    public void SoloPanelClose()
    {
        mainPanel.SetActive(true);
        soloPanel.SetActive(false);
        
    }




}
