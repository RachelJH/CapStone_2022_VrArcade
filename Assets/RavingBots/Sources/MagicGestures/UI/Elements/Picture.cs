using UnityEngine;
using UnityEngine.UI;

namespace RavingBots.MagicGestures.UI.Elements
{
	/// <summary>
	///     An image UI element.
	/// </summary>
	public class Picture : CanvasElement
	{
		/// <summary>
		///     The image to render.
		/// </summary>
		public Image Image;

		/// <summary>
		///     The color of the image.
		/// </summary>
		private Color _color;

		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();

			_color = Image.color;
		}

		/// <inheritdoc />
		protected override void SetShowProgress(float progress)
		{
			base.SetShowProgress(progress);

			var c = _color;
			c.a *= progress;

			Image.color = c;
		}
	}
}
