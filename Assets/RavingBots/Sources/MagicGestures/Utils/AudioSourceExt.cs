using UnityEngine;

namespace RavingBots.MagicGestures.Utils
{
	/// <summary>
	///     The extension methods for <c>AudioSource</c>.
	/// </summary>
	public static class AudioSourceExt
	{
		/// <summary>
		///     Convenience wrapper around <c>AudioSource.Play</c>.
		/// </summary>
		/// <remarks>
		///     This method can optionally set a new clip, volume and pitch before calling
		///     <c>Play</c>. It will also behave correctly when the clip is already playing,
		///     or when the <c>AudioSource</c> is <see langword="null" />.
		/// </remarks>
		public static void SafePlay(
			this AudioSource audioSource,
			AudioClip clip = null,
			float volume = -1f,
			float pitch = -1f,
			bool onlyIfLouder = false)
		{
			if (audioSource != null)
			{
				if (!onlyIfLouder || !audioSource.isPlaying || (volume > audioSource.volume))
				{
					if (clip != null)
						audioSource.clip = clip;

					if (volume > 0)
						audioSource.volume = volume;

					if (pitch > 0)
						audioSource.pitch = pitch;

					audioSource.Play();
				}
			}
			else
				Debug.LogWarning("AudioSource unassigned");
		}

		/// <summary>
		///     Convenience wrapper around <c>AudioSource.Stop</c>.
		/// </summary>
		/// <remarks>
		///     This method will call <c>Stop</c> if the source is not
		///     <see langword="null" /> and has a clip assigned.
		/// </remarks>
		public static void SafeStop(this AudioSource audioSource)
		{
			if (audioSource != null)
			{
				if (audioSource.clip)
					audioSource.Stop();
			}
			else
				Debug.LogWarning("AudioSource unassigned");
		}

		/// <summary>
		///     Convenience wrapper around <see cref="SafePlay" />
		///     for collision sounds.
		/// </summary>
		/// <remarks>
		///     This method ensures collision sounds are only played when
		///     collision velocity is sufficient, and also scales the volume based on
		///     the velocity.
		/// </remarks>
		public static void SafePlayCollision(
			this AudioSource audioSource,
			AudioClip clip,
			float velocity,
			float minVelocity,
			float velocityToVolume,
			float pitch = 1f)
		{
			if (velocity > minVelocity)
				audioSource.SafePlay(clip, Mathf.Clamp01(velocity * velocityToVolume), 1f, true);
		}
	}
}
