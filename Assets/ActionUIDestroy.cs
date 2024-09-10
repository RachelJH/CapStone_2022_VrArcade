using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ActionUIDestroy : MonoBehaviour
{
    public Text text;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeTextToZeroAlpha(5f, text));
    }

    void enableUI() {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
