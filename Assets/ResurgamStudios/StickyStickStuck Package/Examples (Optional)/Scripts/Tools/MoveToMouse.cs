/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Moves the object to the mouse location.
*******************************************************************************************/
using UnityEngine;
using System.Collections;

namespace StickyStickStuck
{
    public class MoveToMouse : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            var location = Camera.main.ScreenPointToRay(Input.mousePosition);
            this.transform.position = new Vector3(location.origin.x, location.origin.y, this.transform.position.z);
        }
    }
}