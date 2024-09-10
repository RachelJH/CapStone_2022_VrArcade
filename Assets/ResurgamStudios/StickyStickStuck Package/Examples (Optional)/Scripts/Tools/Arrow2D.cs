/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Used for rotating the arrow when in the air.
*******************************************************************************************/
using UnityEngine;
using System.Collections;

namespace StickyStickStuck
{
    public class Arrow2D : MonoBehaviour
    {
        [SerializeField, Tooltip("Angle speed.")]
        private float angleSpeed = 10f;
        public float AngleSpeed
        {
            get { return angleSpeed; }
            set { angleSpeed = value; }
        }

        private Rigidbody2D rigid;
        private bool inAir = true;

        public void ResetInAir()
        {
            inAir = true;
        }

        // Use this for initialization
        void Start()
        {
            rigid = this.GetComponent<Rigidbody2D>();
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            inAir = false;
        }

        void OnCollisionStay2D(Collision2D collision)
        {
            inAir = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (inAir)
            {
                this.transform.right = Vector3.Slerp(transform.right, rigid.velocity.normalized, AngleSpeed * Time.deltaTime);
            }
        }
    }
}