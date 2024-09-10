/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Used for hand interaction.
*******************************************************************************************/

using System;
using UnityEngine;
using System.Collections;

namespace StickyStickStuck
{
    public class HandIcon : MonoBehaviour
    {
        [SerializeField]
        private GameObject icon;
        public GameObject Icon
        {
            get { return icon; }
            set { icon = value; }
        }
        
        [SerializeField]
        private SSS sss;
        public SSS Sss
        {
            get { return sss; }
            set { sss = value; }
        }

        private bool found;

        private void Awake()
        {
            icon.SetActive(false);
        }

        private void Update()
        {
            icon.SetActive(!Sss.Enable && found);
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.attachedRigidbody != null && !other.isTrigger)
                found = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.attachedRigidbody != null && !other.isTrigger)
                found = false;
        }
    }
}