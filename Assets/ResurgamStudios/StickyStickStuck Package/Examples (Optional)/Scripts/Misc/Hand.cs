/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Used for hand interaction.
*******************************************************************************************/
using UnityEngine;
using System.Collections;

namespace StickyStickStuck
{
    public class Hand : MonoBehaviour
    {
        #region Properties
        
        [SerializeField]
        private string inputControl = "Fire1";
        public string InputControl
        {
            get { return inputControl; }
            set { inputControl = value; }
        }
        
        [SerializeField]
        private SSS sss;
        public SSS Sss
        {
            get { return sss; }
            set { sss = value; }
        }
        
        #endregion

        private bool isPressed = false;
        private bool broke = false;
        
        #region Unity Functions
        
        //Use this for initialization
        void Start()
        {
            
        }
        
        //Update is called once per frame
        void Update()
        {
            isPressed = Input.GetButton(InputControl);

            if (Input.GetButtonUp(InputControl))
                broke = false;
            
            Sss.Enable = (isPressed && !broke);
        }

        public void SetBroke()
        {
            broke = true;
        }
        
        #endregion
    }
}