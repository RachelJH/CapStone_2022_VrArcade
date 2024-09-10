using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ZGameManager : MonoBehaviourPunCallbacks
{
    public static ZGameManager instance;
    private static ZGameManager m_instance; // 싱글톤이 할당될 static 변수

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        removeClone();
    }

    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance)
        {
            // 자신을 파괴
            Destroy(gameObject);
            return;
        }
        //DontDestroyOnLoad(gameObject);
        instance = this;
    }

    public void removeClone() {

        GameObject[] rc = GameObject.FindGameObjectsWithTag("Zombi");
        foreach (var target in rc) {
            target.gameObject.transform.root.name = target.gameObject.transform.root.name.Replace("(Clone)", "");
        }
        Debug.Log("rC");

    }
}
