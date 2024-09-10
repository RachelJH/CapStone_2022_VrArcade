using UnityEngine;

namespace RavingBots.MagicGestures.UI.Elements
{
	/// <summary>
	///     A canvas UI element.
	/// </summary>
	[RequireComponent(typeof(Canvas))]
	public abstract class CanvasElement : UiElement
	{
		/// <summary>
		///     The target scale when the canvas is pointed at.
		/// </summary>
		public float HoverScale = 0.1f;

		/// <summary>
		///     The current scale of the element.
		/// </summary>
		private Vector3 _scale;

		/// <summary>
		///     Save the initial state.
		/// </summary>
		protected virtual void Awake()
		{
			_scale = transform.localScale;
		}

		/// <inheritdoc />
		protected override void SetHoverProgress(float progress)
		{
			base.SetHoverProgress(progress);

			transform.localScale = _scale * (1f + HoverProgress * HoverScale);
		}
	}
}
