using UnityEngine;

namespace VRBuilder.TrackAndMeasure.Components
{
    /// <summary>
    /// Provides score feedback by displaying a floating text showing the score increase.
    /// </summary>
    public class FloatingTextScoreFeedback : MonoBehaviour, IScoreFeedbackProvider
    {
        [SerializeField]
        protected FloatingTextMesh textMeshPrefab;

        /// <inheritdoc/>
        public void TriggerFeedback(float scoreDelta, float finalScore, Transform positionProvider)
        {
            FloatingTextMesh textMesh = GameObject.Instantiate(textMeshPrefab).GetComponent<FloatingTextMesh>();
            textMesh.transform.position = positionProvider.position;
            textMesh.Initialize((scoreDelta > 0 ? "+" : "") + scoreDelta.ToString());
        }
    }
}