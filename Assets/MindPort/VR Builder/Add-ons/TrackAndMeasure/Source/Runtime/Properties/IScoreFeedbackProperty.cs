using UnityEngine;
using VRBuilder.Core.Properties;

namespace VRBuilder.TrackAndMeasure.Properties
{
    /// <summary>
    /// Property for an object providing feedback over a numerical score.
    /// </summary>
    public interface IScoreFeedbackProperty : ISceneObjectProperty
    {
        /// <summary>
        /// Triggers the feedback.
        /// </summary>        
        void TriggerFeedback(float scoreDelta, float finalScore, Transform position);
    }
}