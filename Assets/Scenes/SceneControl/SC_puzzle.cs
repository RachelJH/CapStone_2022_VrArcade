using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SC_puzzle : MonoBehaviour
{
    public Transform fireHole;
    private string sceneName;
    public void SC_Load()
    {
        RaycastHit hit;

        if (Physics.Raycast(fireHole.position, fireHole.forward * 10000f, out hit))
        {
           
            

            if (hit.collider.tag == "sceneSelect")
            {
                sceneName = hit.transform.gameObject.ToString();
                if(sceneName.Equals( "maze_scene (UnityEngine.GameObject)"))
                    SceneManager.LoadScene("maze_scene");

                if (sceneName.Equals("puzzle_scene (UnityEngine.GameObject)"))
                    SceneManager.LoadScene("puzzle_scene");

                if (sceneName.Equals("action_scene (UnityEngine.GameObject)"))
                    SceneManager.LoadScene("action_scene");

                if (sceneName.Equals("control_scene (UnityEngine.GameObject)"))
                    SceneManager.LoadScene("control_scene");

                if (sceneName.Equals("ready_scene (UnityEngine.GameObject)"))
                    SceneManager.LoadScene("ready_scene");

                Debug.Log(hit.transform.gameObject);
               
            }
        }
    }

    public void goPuzzle() {
        
    }
}
