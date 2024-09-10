/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Used for rotating the arrow when in the air.
*******************************************************************************************/
using UnityEngine;
using System.Collections;

namespace StickyStickStuck
{
    public class Arrow : MonoBehaviour
    {
        [SerializeField, Tooltip("Angle speed.")]
        private float angleSpeed = 10f;
        public float AngleSpeed
        {
            get { return angleSpeed; }
            set { angleSpeed = value; }
        }

        private Rigidbody rigid;
        private bool inAir = true;

        public void ResetInAir()
        {
            inAir = true;
        }

        // Use this for initialization
        void Start()
        {
            rigid = this.GetComponent<Rigidbody>();
        }

        void OnCollisionEnter(Collision collision)
        {
            inAir = false;
        }

        void OnCollisionStay(Collision collision)
        {
            inAir = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (inAir)
            {
                this.transform.forward = Vector3.Slerp(transform.forward, rigid.velocity.normalized, AngleSpeed * Time.deltaTime);
            }
        }
    }
}