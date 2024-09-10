using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    //public GameObject[] checkPoint;
   // bool[] checking;
    public Rigidbody playerRigidbody;
    public Vector3 cpPos = new Vector3(133.807f, 20.47f, -21.99883f); // �÷��̾� ���� ��ġ�� �ʱ� üũ����Ʈ ��ġ�� ����

    private void Start()
    {
        //checking = new bool[checkPoint.Length];
        playerRigidbody = GetComponent<Rigidbody>();
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "CheckPoint")
        {
            cpPos = other.transform.position;
        }
    }
}
