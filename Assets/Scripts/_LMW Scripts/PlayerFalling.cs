using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFalling : MonoBehaviour
{
    public AudioClip fallSound;
    public Text fallText;
    private void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        AudioSource adc;
        if (other.CompareTag("Player")){
            other.transform.position = other.GetComponentInParent<Checkpoint>().cpPos;
            adc = other.GetComponent<AudioSource>();
            adc.clip = fallSound;
            adc.Play();
            fallText.gameObject.SetActive(true);
            StartCoroutine(FadeTextToZeroAlpha(5f, fallText));


        }
    }
    public IEnumerator FadeTextToFullAlpha(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
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
