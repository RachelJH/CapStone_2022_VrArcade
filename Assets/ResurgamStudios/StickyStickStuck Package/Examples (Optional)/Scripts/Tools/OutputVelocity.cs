/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Outputs the velocity of the gameobject.
*******************************************************************************************/
using UnityEngine;
using System.Collections;

namespace StickyStickStuck
{
    public class OutputVelocity : MonoBehaviour
    {
        private Rigidbody rigid;
        private Rigidbody2D rigid2D;

        // Use this for initialization
        void Start()
        {
            if (this.GetComponent<Rigidbody>() != null)
            {
                rigid = this.GetComponent<Rigidbody>();
            }
            else if (this.GetComponent<Rigidbody2D>() != null)
            {
                rigid2D = this.GetComponent<Rigidbody2D>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (rigid != null)
            {
                Debug.Log(rigid.velocity.magnitude);
            }
            else if (rigid2D != null)
            {
                Debug.Log(rigid2D.velocity.magnitude);
            }
        }
    }
}