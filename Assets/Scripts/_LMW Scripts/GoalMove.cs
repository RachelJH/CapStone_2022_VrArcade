using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GoalMove : MonoBehaviour
{
    public Text finishText;
    Vector3 destination = new Vector3 (700.18f, 100.51f, -17.89f);
    bool destinationCheck = false;
    Scene scene;

    private void Start()
    {
        scene = SceneManager.GetActiveScene();
    }

    void FixedUpdate()
    {
        if (destinationCheck == true)
        {

            transform.position = Vector3.Slerp(transform.position, destination, 0.01f);
        }
    } 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TeleportPad"))
        {
            Debug.Log("ÅÚ·¹Æ÷Æ® ÆÐµå Á¢ÃË");
            destinationCheck = true;
            
        }
        
            if (other.CompareTag("Goal"))
            {
                //Debug.Log("°ñ Á¢ÃË");
                destinationCheck = false;
                if (scene.name == "control_scene_single")
                {
                    finishText.gameObject.SetActive(true);
                    StartCoroutine(FadeTextToZeroAlpha(8f, finishText));
                    Invoke("toMainMenu", 10);
                }
            }
        
    }
    private void toMainMenu()
    {
        
        SceneManager.LoadScene("MainMenu");
    }
    public IEnumerator FadeTextToZeroAlpha(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }
}

