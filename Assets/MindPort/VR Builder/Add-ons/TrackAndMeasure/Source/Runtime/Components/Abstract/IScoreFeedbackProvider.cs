using UnityEngine;

namespace VRBuilder.TrackAndMeasure.Components
{
    /// <summary>
    /// An object implementing this interface can provide feedback when a numerical score is updated.
    /// </summary>
    public interface IScoreFeedbackProvider
    {
        /// <summary>
        /// Triggers the implemented feedback.
        /// </summary>
        void TriggerFeedback(float scoreDelta, float finalScore, Transform positionProvider);
    }
}