using RavingBots.MagicGestures.UI.Elements;
using UnityEngine;

namespace RavingBots.MagicGestures.UI.Views
{
	/// <summary>
	///     The welcome splash screen.
	/// </summary>
	public class WelcomeView : View
	{
		/// <summary>
		///     The position of the splash.
		/// </summary>
		public Vector3 Position = new Vector3(0f, 1f, 4f);

		/// <summary>
		///     The splash screen image.
		/// </summary>
		[SerializeField] protected Picture Picture;

		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();

			Picture.OnPress += GoBack;
		}

		/// <inheritdoc />
		public override void GoBack()
		{
			base.GoBack();

			MagicMenu.Instance.CurrentView = null;
		}
	}
}
