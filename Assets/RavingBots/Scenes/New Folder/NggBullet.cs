using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NggBullet : MonoBehaviour
{
    public string playerCheck;

    // Start is called before the first frame update
    void Start()
    {
        checkOwnerPlayer();
        
    }

    private void Update()
    {
        //transform.Translate(Vector3.forward * 10 * Time.deltaTime);
    }

    public void checkOwnerPlayer() {
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, 3f);
        string playerName;
        foreach (Collider col in colliders) {
            if (col.CompareTag("Player")) {
                playerName = col.name; // ���⿡ PlayerController�� ��ũ��Ʈ P1, P2���� ��ũ��Ʈ �߰��ؼ� ������ �Ǻ�
                Debug.Log(playerName + "�� �� ����");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("P1 Check")) {
            if (playerCheck == "P1")
            {
                Debug.Log("p1������ p1���");
            }
            else {
                Debug.Log("p2������ p1�� ���� "); //���� ȿ�� �߰�
            }
        }

        if (other.transform.CompareTag("P2 Check")) {
            if(playerCheck == "P1")
            {
                Debug.Log("p1������ p2�� ����"); //���� ȿ�� �߰�
            }
            else
            {
                Debug.Log("p2������ p2 ��� ");
            }

        }
    }
}
