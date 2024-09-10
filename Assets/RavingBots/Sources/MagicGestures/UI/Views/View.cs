using RavingBots.MagicGestures.UI.Elements;

using UnityEngine;

namespace RavingBots.MagicGestures.UI.Views
{
	/// <summary>
	///     The base class for the menu views.
	/// </summary>
	public abstract class View : MonoBehaviour
	{
		/// <summary>
		///     The children of the view.
		/// </summary>
		private UiElement[] _uiElements;

		/// <summary>
		///     Set the initial state.
		/// </summary>
		protected virtual void Awake()
		{
			_uiElements = GetComponentsInChildren<UiElement>();
		}

		/// <summary>
		///     Change the visibility of the view.
		/// </summary>
		/// <inheritdoc cref="UiElement.SetVisible" />
		public virtual void SetVisible(bool state, bool immediate = false)
		{
			foreach (var element in _uiElements)
				element.SetVisible(state, immediate);
		}

		/// <summary>
		///     Called when user presses the back button.
		/// </summary>
		public virtual void GoBack()
		{
		}

		/// <summary>
		///     Switch to a different view.
		/// </summary>
		/// <seealso cref="SetVisible" />
		public static void Transit(View current, View next, bool immediate = false)
		{
			if (current)
				current.SetVisible(false, immediate);

			if (next)
				next.SetVisible(true, immediate);
		}
	}
}
