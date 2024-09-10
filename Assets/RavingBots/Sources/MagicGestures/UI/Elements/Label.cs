using UnityEngine;
using UnityEngine.UI;

namespace RavingBots.MagicGestures.UI.Elements
{
	/// <summary>
	///     A static label UI element.
	/// </summary>
	public class Label : CanvasElement
	{
		/// <summary>
		///     The text to render.
		/// </summary>
		public Text Text;

		/// <summary>
		///     The text color.
		/// </summary>
		private Color _color;

		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();

			_color = Text.color;
		}

		/// <inheritdoc />
		protected override void SetShowProgress(float progress)
		{
			base.SetShowProgress(progress);

			var c = _color;
			c.a *= progress;

			Text.color = c;
		}
	}
}
