using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ZGameManager : MonoBehaviourPunCallbacks
{
    public static ZGameManager instance;
    private static ZGameManager m_instance; // �̱����� �Ҵ�� static ����

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
        // ���� �̱��� ������Ʈ�� �� �ٸ� GameManager ������Ʈ�� �ִٸ�
        if (instance)
        {
            // �ڽ��� �ı�
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
