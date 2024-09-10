/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: 
*******************************************************************************************/
using UnityEngine;
using System.Collections;

namespace StickyStickStuck
{
    public class Timer
    {
        private bool startTimer;
        public bool StartTimer
        {
            get { return startTimer; }
            set { startTimer = value; }
        }

        private float time;
        public float Time
        {
            get { return time; }
            set { time = value; }
        }

        public float countdown;
        public float Countdown
        {
            get { return countdown; }
            set { countdown = value; }
        }

        private bool alarm = false;
        public bool Alarm
        {
            get { return alarm; }
            set { alarm = value; }
        }

        private float timeStamp = 0f;

        public void UpdateTimer()
        {
            if (StartTimer && !Alarm)
            {
                if (Time > Countdown)
                {
                    Alarm = false;

                    if (timeStamp == 0)
                    {
                        timeStamp = UnityEngine.Time.time;
                    }

                    Countdown = (UnityEngine.Time.time - timeStamp);
                }
                else
                {
                    Alarm = true;
                }
            }
            else
            {
                Countdown = 0f;
                Alarm = false;
                timeStamp = 0f;
            }
        }
    }
}