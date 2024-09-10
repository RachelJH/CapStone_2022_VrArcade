using RavingBots.MagicGestures.Utils;

using UnityEngine;

namespace RavingBots.MagicGestures.UI.Elements
{
	/// <summary>
	///     A bubble button to select the spell color.
	/// </summary>
	public class ColorSlot : BubbleButton
	{
		/// <summary>
		///     The color of this slot.
		/// </summary>
		public Color Color
		{
			get { return _material.GetEmission(); }
			set { _material.SetEmission(value); }
		}

		/// <summary>
		///     Material of the renderer.
		/// </summary>
		private Material _material;

		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();

			_material = GetComponent<MeshRenderer>().material;
		}
	}
}
