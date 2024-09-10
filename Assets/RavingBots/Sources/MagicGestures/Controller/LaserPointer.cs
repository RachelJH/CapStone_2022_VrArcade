using System.Collections.Generic;
using System.Linq;
using RavingBots.MagicGestures.Game;
using RavingBots.MagicGestures.UI.Elements;
using UnityEngine;

namespace RavingBots.MagicGestures.Controller
{
	/// <summary>
	///     The component responsible for the wand laser pointer.
	/// </summary>
	/// <remarks>
	///     This component is responsible for updating the hover and press
	///     states of UI elements the user is pointing at with the motion controller.
	///     This is done by casting a ray from the tip of the wand in the direction it's facing.
	/// </remarks>
	/// <seealso cref="MagicWand" />
	/// <seealso cref="UiElement" />
	/// <seealso cref="PhysicsUtils.RaycastUi" />
	public class LaserPointer : MonoBehaviour
	{
		/// <summary>
		///     Get the current ray for this pointer.
		/// </summary>
		public Ray Ray
		{
			get { return new Ray(transform.position, transform.up); }
		}

		/// <summary>
		///     The position of the element being pointed at, if any.
		/// </summary>
		public Vector3? HoverPosition { get; private set; }

		/// <summary>
		///     The UI element being pointed at, if any.
		/// </summary>
		public UiElement HoverElement { get; private set; }

		/// <summary>
		///     The UI element being pressed, if any.
		/// </summary>
		public UiElement PressElement { get; private set; }

		/// <summary>
		///     The current pressed state.
		/// </summary>
		/// <seealso cref="SetPressed" />
		private bool _pressed;

		/// <inheritdoc cref="_pressed" />
		public bool Pressed
		{
			get { return _pressed; }
			set
			{
				if ((_pressed == value) || !enabled)
					return;

				_pressed = value;

				SetPressed(_pressed);
			}
		}

		/// <summary>
		///     Clear the hover and pressed states when the pointer is disabled.
		/// </summary>
		protected void OnDisable()
		{
			_pressed = false;
			SetPressed(_pressed);

			HoverStop();
			HoverPosition = null;
		}

		/// <summary>
		///     Update the pressed state.
		/// </summary>
		/// <seealso cref="PressElement" />
		/// <seealso cref="HoverElement" />
		private void SetPressed(bool pressed)
		{
			if (pressed)
			{
				CastRay();

				if (!HoverElement)
					return;

				HoverElement.Pressed = true;
				if (HoverElement.Pressed)
					PressElement = HoverElement;
			}
			else
			{
				if (!PressElement)
					return;

				if (!StillPress(PressElement))
					PressElement.Pressed = false;

				PressElement = null;
			}
		}

		/// <summary>
		///     Clear the hover state.
		/// </summary>
		private void HoverStop()
		{
			if (!HoverElement)
				return;

			if (!StillHover(HoverElement))
				HoverElement.Hover = false;

			HoverElement = null;
		}

		/// <inheritdoc cref="CastRay" />
		protected void Update()
		{
			CastRay();
		}

		/// <summary>
		///     Update the hover state based on the pointer ray.
		/// </summary>
		/// <seealso cref="Ray" />
		private void CastRay()
		{
			UiElement hitElement;
			Vector3 hitPosition;

			HoverPosition = PhysicsUtils.RaycastUi(Ray, out hitElement, out hitPosition)
				? (Vector3?) hitPosition
				: null;

			if (HoverElement && !HoverElement.Hover)
				HoverElement = null;

			if (PressElement && !PressElement.Pressed)
				PressElement = null;

			if (HoverElement == hitElement)
				return;

			HoverStop();

			if (hitElement)
			{
				hitElement.Hover = true;
				if (hitElement.Hover)
					HoverElement = hitElement;
			}
		}

		/// <summary>
		///     Check if the element still has any pointers pointing at it.
		/// </summary>
		private bool StillHover(UiElement e)
		{
			return GetOtherPointers().Select(lp => lp.HoverElement).Contains(e);
		}

		/// <summary>
		///     Check if the element still has any pointers pressing it.
		/// </summary>
		private bool StillPress(UiElement e)
		{
			return GetOtherPointers().Select(lp => lp.PressElement).Contains(e);
		}

		/// <summary>
		///     Get the other pointers from <see cref="WandManager" />.
		/// </summary>
		private IEnumerable<LaserPointer> GetOtherPointers()
		{
			return WandManager.Instance.Wands.Select(w => w.LaserPointer).Where(lp => lp != this);
		}
	}
}
