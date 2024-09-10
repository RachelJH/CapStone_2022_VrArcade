using RavingBots.MagicGestures.Controller;
using RavingBots.MagicGestures.UI.Views;
using RavingBots.MagicGestures.Utils;
using UnityEngine;

namespace RavingBots.MagicGestures.UI
{
	/// <summary>
	///     The component that implements the immersive menu.
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class MagicMenu : MonoSingleton<MagicMenu>
	{
		/// <summary>
		///     The distance from the camera to the UI.
		/// </summary>
		public float DistanceToCamera = 2f;

		/// <summary>
		///     The currently shown view.
		/// </summary>
		/// <seealso cref="SetCurrentView" />
		private View _currentView;

		/// <inheritdoc cref="_currentView" />
		public View CurrentView
		{
			get { return _currentView; }
			set
			{
				if (_currentView == value)
					return;

				var previous = _currentView;
				_currentView = value;

				SetCurrentView(_currentView, previous);
			}
		}

		/// <summary>
		///     <see langword="true" /> if the menu is shown.
		/// </summary>
		public bool Shown
		{
			get { return CurrentView != null; }
		}

		/// <summary>
		///     The instance of the welcome view.
		/// </summary>
		public WelcomeView WelcomeView { get; private set; }

		/// <summary>
		///     The instance of the spellbook view.
		/// </summary>
		public SpellBookView SpellBookView { get; private set; }

		/// <summary>
		///     The instance of the spell edit view.
		/// </summary>
		public SpellEditView SpellEditView { get; private set; }

		/// <summary>
		///     The instance of the gesture training view.
		/// </summary>
		public SpellTrainView SpellTrainView { get; private set; }

		/// <summary>
		///     The sound played when the menu is shown.
		/// </summary>
		[SerializeField] protected AudioClip ShowSound;

		/// <summary>
		///     The sound played when the menu is hidden.
		/// </summary>
		[SerializeField] protected AudioClip HideSound;

		/// <summary>
		///     The menu's audio source.
		/// </summary>
		private AudioSource _audioSource;

		/// <summary>
		///     Find the children components.
		/// </summary>
		protected override void Awake()
		{
			base.Awake();

			_audioSource = GetComponent<AudioSource>();

			WelcomeView = GetComponentInChildren<WelcomeView>();
			SpellBookView = GetComponentInChildren<SpellBookView>();
			SpellEditView = GetComponentInChildren<SpellEditView>();
			SpellTrainView = GetComponentInChildren<SpellTrainView>();
		}

		/// <summary>
		///     Register the event handlers.
		/// </summary>
		protected void Start()
		{
			WandManager.Instance.OnMenuButtonEvent += OnMenuButton;
		}

		/// <summary>
		///     Called when the user presses the menu button.
		/// </summary>
		private void OnMenuButton(MagicWand wand)
		{
			if (CurrentView)
				CurrentView.GoBack();
			else
				CurrentView = SpellBookView;
		}

		/// <summary>
		///     Called when the user performs a gesture.
		/// </summary>
		public void OnGesture(MagicWand wand, GestureData gesture)
		{
			if (CurrentView == SpellTrainView)
			{
				gesture = new GestureData(gesture);
				gesture.Normalize(Quaternion.Inverse(transform.rotation));
				SpellTrainView.OnGesture(gesture);
			}
		}

		/// <summary>
		///     Called when the active view changes.
		/// </summary>
		private void SetCurrentView(View currentView, View previousView)
		{
			if (previousView == null)
			{
				WandManager.Instance.OnGestureEvent -= WandManager.Instance.Recognize;
				WandManager.Instance.OnGestureEvent += OnGesture;

				AlignTo(Camera.main.transform, DistanceToCamera);
				_audioSource.SafePlay(ShowSound);
			}
			else if (currentView == null)
			{
				WandManager.Instance.OnGestureEvent -= OnGesture;
				WandManager.Instance.OnGestureEvent += WandManager.Instance.Recognize;

				_audioSource.SafePlay(HideSound);
			}

			if ((previousView == null) || (previousView == SpellTrainView))
				WandManager.Instance.MenuMode = true;
			else if ((currentView == null) || (currentView == SpellTrainView))
				WandManager.Instance.MenuMode = false;

			View.Transit(previousView, currentView);
		}

		/// <summary>
		///     Position the menu based on the camera transform.
		/// </summary>
		/// S
		private void AlignTo(Transform t, float dist)
		{
			transform.rotation = t.rotation;
			transform.position = t.position + dist * t.forward;
		}
	}
}
