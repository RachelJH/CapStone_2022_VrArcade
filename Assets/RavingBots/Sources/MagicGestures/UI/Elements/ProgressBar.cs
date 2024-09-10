// ReSharper disable CompareOfFloatsByEqualityOperator

using UnityEngine;

namespace RavingBots.MagicGestures.UI.Elements
{
	/// <summary>
	///     A progress bar element.
	/// </summary>
	public class ProgressBar : UiElement
	{
		/// <summary>
		///     The scaled transform of the bar.
		/// </summary>
		[SerializeField]
		protected Transform Scaled;

		/// <summary>
		///     The current progress.
		/// </summary>
		/// <seealso cref="SetProgress" />
		private float _progress;

		/// <inheritdoc cref="_progress" />
		public float Progress
		{
			get { return _progress; }
			set
			{
				if (_progress == value)
					return;

				_progress = value;

				SetProgress(_progress);
			}
		}

		/// <summary>
		///     Set the initial state.
		/// </summary>
		protected void Awake()
		{
			SetProgress(_progress);
		}

		/// <summary>
		///     Called when the progress changes.
		/// </summary>
		private void SetProgress(float progress)
		{
			Scaled.localScale = new Vector3(progress, 1f, 1f);
		}
	}
}
