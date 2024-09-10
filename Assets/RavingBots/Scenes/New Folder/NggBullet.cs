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
                playerName = col.name; // 여기에 PlayerController에 스크립트 P1, P2구별 스크립트 추가해서 누군지 판별
                Debug.Log(playerName + "가 쏜 로켓");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("P1 Check")) {
            if (playerCheck == "P1")
            {
                Debug.Log("p1로켓이 p1통과");
            }
            else {
                Debug.Log("p2로켓이 p1에 닿음 "); //폭발 효과 추가
            }
        }

        if (other.transform.CompareTag("P2 Check")) {
            if(playerCheck == "P1")
            {
                Debug.Log("p1로켓이 p2에 닿음"); //폭발 효과 추가
            }
            else
            {
                Debug.Log("p2로켓이 p2 통과 ");
            }

        }
    }
}
