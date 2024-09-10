using UnityEngine;

namespace VRBuilder.TrackAndMeasure.Components
{
    /// <summary>
    /// Provides score feedback by playing a sound at the provided location.
    /// </summary>
    public class AudioScoreFeedback : MonoBehaviour, IScoreFeedbackProvider
    {
        [SerializeField]
        private AudioSource audioSourcePrefab;

        private AudioSource audioSource;

        private void OnEnable()
        {
            audioSource = GameObject.Instantiate<AudioSource>(audioSourcePrefab);
        }

        private void OnDisable()
        {
            Destroy(audioSource);
        }
        
        /// <inheritdoc/>
        public void TriggerFeedback(float scoreDelta, float finalScore, Transform positionProvider)
        {
            audioSource.transform.position = positionProvider.position;
            audioSource.Play();
        }
    }
}