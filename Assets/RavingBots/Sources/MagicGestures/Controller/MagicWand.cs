using RavingBots.MagicGestures.Game;
using RavingBots.MagicGestures.UI;
using RavingBots.MagicGestures.Utils;
using UnityEngine;
using UnityEngine.XR;

namespace RavingBots.MagicGestures.Controller
{
	/// <summary>
	///     The component that handles motion controller input.
	/// </summary>
	/// <remarks>
	///     The transform of the object is updated on movement
	///     by components provided by the SteamVR plugin, which
	///     is why there's no <c>Update</c> method here.
	/// </remarks>
	/// <seealso cref="LaserPointer" />
	/// <seealso cref="GestureTracker" />
	/// <seealso cref="SpellCaster" />
	public class MagicWand : MonoBehaviour
	{
		/// <summary>
		///     <see langword="true" /> if the menu is open.
		/// </summary>
		/// <seealso cref="SetMenuMode" />
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

				SetMenuMode(_menuMode);
			}
		}

		/// <summary>
		///     The laser pointer component of this wand.
		/// </summary>
		public LaserPointer LaserPointer { get; private set; }

		/// <summary>
		///     The gesture tracker of this wand.
		/// </summary>

		public SpellCaster SpellCaster { get; private set; }

		/// <summary>
		///     All renderers this wand consists of.
		/// </summary>
		private Renderer[] _renderers;

		/// <summary>
		///     The audio source attached to this wand.
		/// </summary>
		private AudioSource _audioSource;

		/// <summary>
		///     <see langword="true" /> if the application is exiting.
		/// </summary>
		private bool _quitting;

		/// <summary>
		///     Left hand/right hand node.
		/// </summary>
		private XRNode _xrNode;

		private bool _wasTriggerOn;

		/// <summary>
		///     Get and configure the child components.
		/// </summary>
		protected void Awake()
		{
			LaserPointer = GetComponentInChildren<LaserPointer>();

			SpellCaster = GetComponentInChildren<SpellCaster>();

			_renderers = GetComponentsInChildren<Renderer>();
			_audioSource = GetComponentInChildren<AudioSource>();
		}

		/// <summary>
		///     Register this wand with the <see cref="WandManager" />.
		/// </summary>
		protected void Start()
		{
			SetMenuMode(false);
			_xrNode = WandManager.Instance.Wands.Count == 0 ? XRNode.LeftHand : XRNode.RightHand;
			WandManager.Instance.Wands.Add(this);
		}

		/// <summary>
		///     Short-circuit the destruction logic on application quit.
		/// </summary>
		protected void OnApplicationQuit()
		{
			_quitting = true;
		}

		/// <summary>
		///     Stop the current action.
		/// </summary>
		/// <remarks>
		///     Turns into no-op when <see cref="_quitting" /> is <see langword="true" />.
		/// </remarks>
		protected void OnDisable()
		{
			if (_quitting)
				return;

			WandManager.Instance.OnActionStop(this);
		}

		/// <summary>
		///     Play the casting sound.
		/// </summary>
		public void OnMagic()
		{
			_audioSource.SafePlay();
		}

		/// <summary>
		///     Update function called every frame. Updates wand position and rotation, collects input.
		/// </summary>
		public void Update()
		{
			transform.localPosition = InputTracking.GetLocalPosition(_xrNode);
			transform.localRotation = InputTracking.GetLocalRotation(_xrNode);

			if (Input.GetButtonDown("VrMenuLeft") && _xrNode == XRNode.LeftHand||
				Input.GetButtonDown("VrMenuRight") && _xrNode == XRNode.RightHand) MenuButtonClicked(this);
			if (Input.GetAxis("VrLeftTrigger") > 0.9f && _xrNode == XRNode.RightHand  && !_wasTriggerOn ||
				Input.GetAxis("VrRightTrigger") > 0.9f && _xrNode == XRNode.LeftHand  && !_wasTriggerOn )
			{
				TriggerClicked(this);
				_wasTriggerOn = true;
			}

			if (Input.GetAxis("VrLeftTrigger") < 0.1f && _xrNode == XRNode.RightHand && _wasTriggerOn ||
				Input.GetAxis("VrRightTrigger") < 0.1f && _xrNode == XRNode.LeftHand && _wasTriggerOn)
			{
				TriggerUnclicked(this);
				_wasTriggerOn = false;
			}
		}

		/// <summary>
		///     Update the menu state.
		/// </summary>
		/// <remarks>
		///     <para>
		///         When the menu is shown, the laser pointer is enabled, the gesture tracker
		///         disabled, and the wand is put on the UI layer.
		///     </para>
		///     <para>
		///         When the menu is hidden, the laser pointer is disabled, the gesture tracker enabled,
		///         and the wand is put back on the default layer.
		///     </para>
		/// </remarks>
		private void SetMenuMode(bool menuMode)
		{
			if (menuMode)
				SpellCaster.ClearSpell();

			WandManager.Instance.OnActionStop(this);

			LaserPointer.enabled = menuMode;
			WandManager.Instance.GestureTracker.enabled = !menuMode;

			var menuVisible = MagicMenu.Instance.Shown;

			foreach (var r in _renderers)
				r.gameObject.layer = menuVisible ? PhysicsUtils.LayerUI : PhysicsUtils.LayerDefault;
		}

		/// <summary>
		///     Called when the user presses the menu button on the controller.
		/// </summary>
		private void MenuButtonClicked(object sender)
		{
			WandManager.Instance.OnMenuButton(this);
		}

		/// <inheritdoc cref="ActionStart" />
		/// <seealso cref="ActionStart" />
		private void TriggerClicked(object sender)
		{
			WandManager.Instance.OnActionStart(this);
		}

		/// <inheritdoc cref="ActionStop" />
		/// <seealso cref="ActionStop" />
		private void TriggerUnclicked(object sender)
		{
			WandManager.Instance.OnActionStop(this);
		}
	}
}
