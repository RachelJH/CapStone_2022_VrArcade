/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Used for making a weapon in the weapon 2D scenes.
*******************************************************************************************/
using UnityEngine;
using System.Collections;

namespace StickyStickStuck
{
    public class Weapon2D : MonoBehaviour
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
        
        //Use this for initialization
        void Start()
        {
        }

        //Update is called once per frame
        void Update()
        {
            if (Input.GetButtonUp(InputControl))
            {
                FireArrow();
            }
        }

        public void FireArrow()
        {
            _spawn.Spawning(this.transform.right * Power, Vector3.right * Torque, MaxAngularVelocity);
        }

        #endregion
    }
}