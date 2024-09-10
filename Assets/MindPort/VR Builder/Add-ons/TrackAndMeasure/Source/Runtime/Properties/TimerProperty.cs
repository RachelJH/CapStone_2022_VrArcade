using System;
using UnityEngine;
using VRBuilder.Core.Properties;

namespace VRBuilder.TrackAndMeasure.Properties
{
    /// <summary>
    /// Property that acts as a timer and stores time on the required <see cref="NumberDataProperty"/>.
    /// </summary>
    [RequireComponent(typeof(NumberDataProperty))]
    public class TimerProperty : ProcessSceneObjectProperty, ITimerProperty
    {
        private NumberDataProperty timeProperty;
        private float startTime;
        private float timerOriginalState;

        /// <inheritdoc/>
        public bool IsCountdown { get; set; }

        /// <inheritdoc/>
        public bool IsRunning { get { return isRunning; } }

        private bool isRunning = false;

        /// <inheritdoc/>
        public event EventHandler<EventArgs> TimerStarted;

        /// <inheritdoc/>
        public event EventHandler<EventArgs> TimerStopped;

        /// <inheritdoc/>
        public event EventHandler<EventArgs> TimerAtZero;

        private void Awake()
        {
            timeProperty = GetComponent<NumberDataProperty>();
            timeProperty.ValueReset += OnValueReset;
        }

        private void OnValueReset(object sender, EventArgs args)
        {
            if(isRunning)
            {
                startTime = Time.time;
                timerOriginalState = timeProperty.GetValue();
            }
        }

        private void Update()
        {
            if (isRunning == false)
            {
                return;
            }

            if (IsCountdown)
            {
                timeProperty.SetValue(Mathf.Max(timerOriginalState - (Time.time - startTime), 0));

                if (timeProperty.GetValue() <= 0)
                {
                    TimerAtZero?.Invoke(this, EventArgs.Empty);
                    StopTimer();
                }
            }
            else
            {
                timeProperty.SetValue(timerOriginalState + (Time.time - startTime));
            }
        }

        /// <inheritdoc/>
        public void StartTimer()
        {
            isRunning = true;
            startTime = Time.time;
            timerOriginalState = timeProperty.GetValue();
            TimerStarted?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc/>
        public void StopTimer()
        {
            isRunning = false;
            TimerStopped?.Invoke(this, EventArgs.Empty);
        }
    }
}