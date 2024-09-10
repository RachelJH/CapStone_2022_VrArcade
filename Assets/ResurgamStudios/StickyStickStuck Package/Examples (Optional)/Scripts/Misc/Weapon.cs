/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Used for making a weapon in the weapon scenes.
*******************************************************************************************/
using UnityEngine;
using System.Collections;

namespace StickyStickStuck
{
    public class Weapon : MonoBehaviour
    {
        #region Properties

        //Input to shoot
        [SerializeField]
        private string inputControl = "Fire1";
        public string InputControl
        {
            get { return inputControl; }
            set { inputControl = value; }
        }

        //GameObject to shoot
        [SerializeField]
        private Spawn spawn;
        public Spawn _spawn
        {
            get { return spawn; }
            set { spawn = value; }
        }

        //Bullet Speed
        [SerializeField]
        private float power = 800f;
        public float Power
        {
            get { return power; }
            set { power = value; }
        }

        //Bullet Speed
        [SerializeField]
        private float torque = 1600f;
        public float Torque
        {
            get { return torque; }
            set { torque = value; }
        }

        [SerializeField]
        private float maxAngularVelocity = 15f;
        public float MaxAngularVelocity
        {
            get { return maxAngularVelocity; }
            set { maxAngularVelocity = value; }
        }
        #endregion

        #region Unity Functions

        private Animator animator;

        //Use this for initialization
        void Start()
        {
            animator = this.transform.GetComponent<Animator>();
        }

        //Update is called once per frame
        void Update()
        {
            if (Input.GetButtonUp(InputControl))
            {
                animator.SetTrigger("Fire");
            }
        }

        public void FireArrow()
        {
            _spawn.Spawning(this.transform.forward * Power, this.transform.right * Torque, MaxAngularVelocity);

            animator.ResetTrigger("Fire");
        }

        #endregion
    }
}