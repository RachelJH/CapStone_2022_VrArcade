using RavingBots.MagicGestures.Utils;

using UnityEngine;

namespace RavingBots.MagicGestures.UI.Elements
{
	/// <summary>
	///     A bubble button UI element.
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class BubbleButton : UiElement
	{
		/// <summary>
		///     The target scale when the button is pointed at.
		/// </summary>
		public float HoverScale = 0.1f;
		/// <summary>
		///     The target scale when the button is pressed.
		/// </summary>
		public float PressScale = 0.2f;

		/// <summary>
		///     The sound that plays when the button is pointed at.
		/// </summary>
		[SerializeField]
		protected AudioClip HoverSound;
		/// <summary>
		///     The sound that plays when the button is pressed.
		/// </summary>
		[SerializeField]
		protected AudioClip PressSound;

		/// <summary>
		///     The current scale of the element.
		/// </summary>
		private Vector3 _scale;
		/// <summary>
		///     The audio source of the element.
		/// </summary>
		private AudioSource _audioSource;
		/// <summary>
		///     <see langword="true" /> after <see cref="Start" /> has been called.
		/// </summary>
		private bool _started;

		/// <summary>
		///     Save the initial state.
		/// </summary>
		protected virtual void Awake()
		{
			_scale = transform.localScale;
			_audioSource = GetComponent<AudioSource>();
		}

		/// <inheritdoc />
		protected override void Start()
		{
			base.Start();

			_started = true;
		}

		/// <summary>
		///     Get the current scale of the button.
		/// </summary>
		private Vector3 GetScale()
		{
			return _scale * ShowProgress * (1f + HoverProgress * HoverScale +
											Mathf.Max(PressProgress, SelectProgress) * PressScale);
		}

		/// <summary>
		///     Update the scale of the button object.
		/// </summary>
		protected void UpdateScale()
		{
			transform.localScale = GetScale();
		}

		/// <inheritdoc />
		protected override void SetShowProgress(float progress)
		{
			base.SetShowProgress(progress);

			UpdateScale();
		}

		/// <inheritdoc />
		protected override void SetHoverProgress(float progress)
		{
			base.SetHoverProgress(progress);

			UpdateScale();
		}

		/// <inheritdoc />
		protected override void SetPressProgress(float progress)
		{
			base.SetPressProgress(progress);

			UpdateScale();
		}

		/// <inheritdoc />
		protected override void SetSelectProgress(float progress)
		{
			base.SetSelectProgress(progress);

			UpdateScale();
		}

		/// <inheritdoc />
		protected override void SetHover(bool state)
		{
			base.SetHover(state);

			if (state && _started)
				_audioSource.SafePlay(HoverSound);
		}

		/// <inheritdoc />
		protected override void SetPressed(bool state)
		{
			base.SetPressed(state);

			if (state && _started)
				_audioSource.SafePlay(PressSound);
		}
	}
}
