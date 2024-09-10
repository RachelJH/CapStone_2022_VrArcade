using System;
using VRBuilder.Core.Properties;

namespace VRBuilder.TrackAndMeasure.Properties
{
    /// <summary>
    /// Property that acts as a timer.
    /// </summary>
    public interface ITimerProperty : ISceneObjectProperty
    {
        /// <summary>
        /// Raised when the timer starts.
        /// </summary>
        event EventHandler<EventArgs> TimerStarted;

        /// <summary>
        /// Raised when the timer is paused.
        /// </summary>
        event EventHandler<EventArgs> TimerStopped;

        /// <summary>
        /// Raised when a timer reaches zero.
        /// </summary>
        event EventHandler<EventArgs> TimerAtZero;

        /// <summary>
        /// If true, the timer will count down instead of up.
        /// </summary>
        bool IsCountdown { get; set; }

        /// <summary>
        /// True when the timer is active.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        void StartTimer();

        /// <summary>
        /// Stops the timer.
        /// </summary>
        void StopTimer();
    }
}