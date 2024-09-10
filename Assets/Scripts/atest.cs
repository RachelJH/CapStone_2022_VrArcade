using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class atest : MonoBehaviour
{
    public GameObject pre;
    public Transform trans;


    public void inBox() {
        Instantiate(pre, trans.position, trans.rotation);
    }

}
