using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoloPlaySelectManager : MonoBehaviour
{
    public void OnShootingSceneButtonClicked()
    {
        //SceneManager.LoadScene("");
    }
    public void OnActionSceneButtonClicked()
    {
        //SceneManager.LoadScene("");
    }
    public void OnControlSceneButtonClicked()
    {
        SceneManager.LoadScene("control_scene");
    }
    public void OnMazeSceneButtonClicked()
    {
        //SceneManager.LoadScene("");
    }
}
