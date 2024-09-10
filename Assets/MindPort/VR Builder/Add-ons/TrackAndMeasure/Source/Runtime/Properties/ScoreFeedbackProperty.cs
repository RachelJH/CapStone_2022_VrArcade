using UnityEngine;
using VRBuilder.Core.Properties;
using VRBuilder.TrackAndMeasure.Components;

namespace VRBuilder.TrackAndMeasure.Properties
{
    /// <summary>
    /// Implementation that triggers feedback by iterating through all <see cref="IScoreFeedbackProvider"/>s on the game object or its children.
    /// </summary>
    public class ScoreFeedbackProperty : ProcessSceneObjectProperty, IScoreFeedbackProperty
    {
        /// <inheritdoc/>
        public void TriggerFeedback(float scoreDelta, float finalScore, Transform position)
        {
            foreach (IScoreFeedbackProvider provider in GetComponentsInChildren<IScoreFeedbackProvider>())
            {
                provider.TriggerFeedback(scoreDelta, finalScore, position);
            }

        }
    }
}