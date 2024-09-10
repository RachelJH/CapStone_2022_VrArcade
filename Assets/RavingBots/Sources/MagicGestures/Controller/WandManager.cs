using System.Collections.Generic;
using System.Linq;
using RavingBots.MagicGestures.AI;
using RavingBots.MagicGestures.Utils;
using RavingBots.MagicGestures.Utils.Geometry;
using UnityEngine;

namespace RavingBots.MagicGestures.Controller
{
	/// <summary>
	///     The component responsible for tracking wand instances,
	///     and dispatching actions based on the UI state and the input
	///     received from the wands.
	/// </summary>
	/// <seealso cref="MagicWand.ActionStart" />
	/// <seealso cref="MagicWand.ActionStop" />
	public class WandManager : MonoSingleton<WandManager>
	{
		/// <inheritdoc cref="OnMenuButtonEvent" />
		/// <param name="wand">The wand the button belongs to.</param>
		public delegate void MenuButtonDelegate(MagicWand wand);

		/// <inheritdoc cref="OnGestureEvent" />
		/// <param name="wand">The wand used to perform the gesture.</param>
		/// <param name="gesture">The performed gesture.</param>
		public delegate void GestureDelegate(MagicWand wand, GestureData gesture);

		/// <inheritdoc cref="OnGestureRecognizedEvent" />
		/// <param name="wand">The wand used to perform the gesture.</param>
		/// <param name="gesture">The performed gesture.</param>
		/// <param name="spellId">The index of the recognized gesture.</param>
		/// <seealso cref="GestureLearner.Recognize" />
		public delegate void GestureRecognizedDelegate(MagicWand wand, GestureData gesture, int spellId);

		/// <inheritdoc cref="OnGestureNotRecognizedEvent" />
		/// <param name="wand">The wand used to perform the gesture.</param>
		/// <param name="gesture">The performed gesture.</param>
		public delegate void GestureNotRecognizedDelegate(MagicWand wand, GestureData gesture);

		/// <summary>
		///     Raised when the menu button is pressed.
		/// </summary>
		public event MenuButtonDelegate OnMenuButtonEvent;

		/// <summary>
		///     Raised when the user performs a gesture.
		/// </summary>
		public event GestureDelegate OnGestureEvent;

		/// <summary>
		///     Raised when the performed gesture is recognized by the network.
		/// </summary>
		public event GestureRecognizedDelegate OnGestureRecognizedEvent;

		/// <summary>
		///     Raised when the performed gesture is not recognized by the network.
		/// </summary>
		public event GestureNotRecognizedDelegate OnGestureNotRecognizedEvent;

		/// <summary>
		///     Set to <see langword="true" /> if gestures should be mirrored
		///     on the X-axis before recognition.
		/// </summary>
		/// <remarks>
		///     This allows left-handed players to more easily use the
		///     existing gestures, without retraining the network.
		/// </remarks>
		public bool RecognizeMirroredGestureX;

		/// <summary>
		///     The registered wands.
		/// </summary>
		public readonly MonitoredList<MagicWand> Wands;

		public GestureTracker GestureTracker { get; private set; }

		/// <summary>
		///     The spell effect manager of this wand.
		/// </summary>
		/// <summary>
		///     The state of the camera transform when the action starts.
		/// </summary>
		public TransformData CameraTransformOnActionStart { get; private set; }

		/// <summary>
		///     The state of the camera transform when the action ends.
		/// </summary>
		public TransformData CameraTransformOnActionStop { get; private set; }

		/// <summary>
		///     An averaged camera orientantion during the action.
		/// </summary>
		public Quaternion CameraRotationReference
		{
			get
			{
				return Quaternion.Lerp(
					CameraTransformOnActionStart.WorldRotation,
					CameraTransformOnActionStop.WorldRotation,
					0.5f);
			}
		}


		/// <summary>
		///     <see langword="true" /> if the menu is open.
		/// </summary>
		/// <seealso cref="MagicWand.MenuMode" />
		private bool _menuMode;

		/// <inheritdoc cref="_menuMode" />
		public bool MenuMode
		{
			get { return _menuMode; }
			set
			{
				if (_menuMode == value)
					return;

				_menuMode = value;

				foreach (var wand in Wands)
					wand.MenuMode = _menuMode;
			}
		}

		protected override void Awake()
		{
			base.Awake();

			GestureTracker = GetComponent<GestureTracker>();
		}

		/// <summary>
		///     Construct a new instance.
		/// </summary>
		public WandManager()
		{
			OnGestureEvent += Recognize;
			Wands = new MonitoredList<MagicWand>(new List<MagicWand>(), OnAdded, null, null);
		}

		/// <inheritdoc cref="MagicWand.MenuButtonClicked" />
		public void OnMenuButton(MagicWand wand)
		{
			if (OnMenuButtonEvent != null)
				OnMenuButtonEvent(wand);
		}

		/// <summary>
		///     Called after the user performs a gesture.
		/// </summary>
		public void OnGesture(MagicWand wand, GestureData gesture)
		{
			if (OnGestureEvent != null)
				OnGestureEvent(wand, gesture);
		}

		/// <summary>
		///     Try to recognize the performed gesture.
		/// </summary>
		/// <remarks>
		///     This is registered as a handler for the <see cref="OnGesture" /> event.
		/// </remarks>
		/// <param name="wand">The wand used to perform the gesture.</param>
		/// <param name="gesture">The performed gesture.</param>
		public void Recognize(MagicWand wand, GestureData gesture)
		{
			if (!gesture.IsValid) return;

			var normalizedGesture = new GestureData(gesture);
			normalizedGesture.Normalize(Quaternion.Inverse(CameraRotationReference));

			if (RecognizeMirroredGestureX)
				normalizedGesture.MirrorX();

			float output;
			var spellId = GestureLearner.Instance.Recognize(normalizedGesture, out output);

			if (spellId < 0)
			{
				Debug.LogFormat("Not recognized {0} handed", gesture.HandCount);

				if (OnGestureNotRecognizedEvent != null)
					OnGestureNotRecognizedEvent(wand, gesture);

				return;
			}

			Debug.LogFormat("Recognized {0} handed spell {1} with stimulation {2}", gesture.HandCount, spellId, output);

			wand.OnMagic();

			if (OnGestureRecognizedEvent != null)
				OnGestureRecognizedEvent(wand, gesture, spellId);

			if (Wands.Count > 1 && gesture.HandCount>1)
			{
				var otherWand = Wands.First(w => w != wand);
				var index = Wands.IndexOf(otherWand);

				GestureTracker.Tracked[index] = null;

				if (OnGestureRecognizedEvent != null)
					OnGestureRecognizedEvent(otherWand, gesture, spellId);
			}
		}

		/// <summary>
		///     Called when a new wand is registered.
		/// </summary>
		private void OnAdded(MagicWand wand)
		{
			wand.MenuMode = MenuMode;

			GestureLearner.Instance.HandCount = Wands.Count;
			GestureTracker.HandCount = Wands.Count;
			GestureTracker.Tracked = new Transform[2];
		}

		/// <summary>
		///     Called when the user presses the trigger.
		/// </summary>
		public void OnActionStart(MagicWand source)
		{
			CameraTransformOnActionStart = Camera.main.transform;

			if (MenuMode)
				source.LaserPointer.Pressed = true;
			else if (source.SpellCaster.IsLoaded)
				source.SpellCaster.Pressed = true;
			else
			{
				var index = Wands.IndexOf(source);

				if (index >= 0 && GestureTracker.Tracked.Length > index)
				{
					var trackedNew = GestureTracker.Tracked.Clone() as Transform[];
					trackedNew[index] = source.SpellCaster.transform;
					GestureTracker.Tracked = trackedNew;
				}
			}
		}

		/// <summary>
		///     Called when the user releases the trigger.
		/// </summary>
		public void OnActionStop(MagicWand source)
		{
			CameraTransformOnActionStop = Camera.main.transform;

			source.SpellCaster.Pressed = false;
			source.LaserPointer.Pressed = false;

			var gesture = GestureTracker.GetGesture();
			if (gesture != null) OnGesture(source, gesture);

			var index = Wands.IndexOf(source);
			if (index >= 0 && GestureTracker.Tracked != null && GestureTracker.Tracked.Length > index)
			{
				var trackedNew = GestureTracker.Tracked.Clone() as Transform[];
				trackedNew[index] = null;
				GestureTracker.Tracked = trackedNew;
			}
		}
	}
}
