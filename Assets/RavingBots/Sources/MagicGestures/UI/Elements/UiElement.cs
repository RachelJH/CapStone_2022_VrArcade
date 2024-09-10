// ReSharper disable CompareOfFloatsByEqualityOperator

using System;

using RavingBots.MagicGestures.Controller;

using UnityEngine;

namespace RavingBots.MagicGestures.UI.Elements
{
	/// <summary>
	///     The visibility state of the UI element.
	/// </summary>
	public enum State
	{
		/// <summary>
		///     The element is hidden.
		/// </summary>
		Hidden,
		/// <summary>
		///     The element is being shown (during animation).
		/// </summary>
		Showing,
		/// <summary>
		///     The element is visible.
		/// </summary>
		Shown,
		/// <summary>
		///     The element is being hidden (during animation).
		/// </summary>
		Hiding
	}

	/// <summary>
	///     The base class for the immersive UI elements.
	/// </summary>
	public abstract class UiElement : MonoBehaviour
	{
		/// <summary>
		///     The duration of the press animation.
		/// </summary>
		public float PressDuration = 0.1f;

		/// <summary>
		///     The duration of the hover on animation.
		/// </summary>
		public float HoverOnDuration = 0.1f;
		/// <summary>
		///     The duration of the hover out animation.
		/// </summary>
		public float HoverOutDuration = 0.2f;

		/// <summary>
		///     The duration of the show animation.
		/// </summary>
		public float ShowDuration = 0.2f;
		/// <summary>
		///     The duration of the hide animation.
		/// </summary>
		public float HideDuration = 0.1f;

		/// <summary>
		///     The delegate called when the element is pressed.
		/// </summary>
		/// <seealso cref="MagicWand" />
		public event Action OnPress;

		/// <summary>
		///     The progress of the show/hide animation.
		/// </summary>
		/// <seealso cref="SetShowProgress" />
		private float _showProgress;

		/// <inheritdoc cref="_showProgress" />
		public float ShowProgress
		{
			get { return _showProgress; }
			private set
			{
				if (_showProgress == value)
					return;

				_showProgress = value;

				SetShowProgress(_showProgress);
			}
		}

		/// <summary>
		///     The progress of the hover animation.
		/// </summary>
		/// <seealso cref="SetHoverProgress" />
		private float _hoverProgress;

		/// <inheritdoc cref="_hoverProgress" />
		public float HoverProgress
		{
			get { return _hoverProgress; }
			private set
			{
				if (_hoverProgress == value)
					return;

				_hoverProgress = value;

				SetHoverProgress(_hoverProgress);
			}
		}

		/// <summary>
		///     The progress of the press animation.
		/// </summary>
		/// <seealso cref="SetPressProgress" />
		private float _pressProgress;

		/// <inheritdoc cref="_pressProgress" />
		public float PressProgress
		{
			get { return _pressProgress; }
			private set
			{
				if (_pressProgress == value)
					return;

				_pressProgress = value;

				SetPressProgress(_pressProgress);
			}
		}

		/// <summary>
		///     The progress of the select animation.
		/// </summary>
		/// <seealso cref="SetSelectProgress" />
		private float _selectProgress;

		/// <inheritdoc cref="_selectProgress" />
		public float SelectProgress
		{
			get { return _selectProgress; }
			private set
			{
				if (_selectProgress == value)
					return;

				_selectProgress = value;

				SetSelectProgress(_selectProgress);
			}
		}

		/// <summary>
		///     The current visibility state of this element.
		/// </summary>
		/// <seealso cref="SetState" />
		private State _state;

		/// <inheritdoc cref="_state" />
		public State State
		{
			get { return _state; }
			private set
			{
				if (_state == value)
					return;

				var previous = _state;
				_state = value;

				SetState(_state, previous);
			}
		}

		/// <summary>
		///     The current hover state of this element.
		/// </summary>
		/// <seealso cref="SetHover" />
		private bool _hover;

		/// <inheritdoc cref="_hover" />
		public bool Hover
		{
			get { return _hover; }
			set
			{
				if ((_hover == value) || (State != State.Shown) || Disabled)
					return;

				_hover = value;

				SetHover(_hover);
			}
		}

		/// <summary>
		///     The current pressed state of this element.
		/// </summary>
		/// <seealso cref="SetPressed" />
		private bool _pressed;

		/// <inheritdoc cref="_pressed" />
		public bool Pressed
		{
			get { return _pressed; }
			set
			{
				if ((_pressed == value) || (State != State.Shown) || Disabled)
					return;

				_pressed = value;

				SetPressed(_pressed);
			}
		}

		/// <summary>
		///     The current selected state of this element.
		/// </summary>
		/// <seealso cref="SetSelected" />
		private bool _selected;

		/// <inheritdoc cref="_selected" />
		public bool Selected
		{
			get { return _selected; }
			set
			{
				if (_selected == value)
					return;

				_selected = value;

				SetSelected(_selected);
			}
		}

		/// <summary>
		///     The current disabled state of this element.
		/// </summary>
		/// <seealso cref="SetDisabled" />
		private bool _disabled;

		/// <inheritdoc cref="_disabled" />
		public bool Disabled
		{
			get { return _disabled; }
			set
			{
				if (_disabled == value)
					return;

				_disabled = value;

				SetDisabled(_disabled);
			}
		}

		/// <summary>
		///     Set the initial state of this element.
		/// </summary>
		protected virtual void Start()
		{
			SetHover(_hover);
			SetPressed(_pressed);
			SetSelected(_selected);
			SetDisabled(_disabled);

			SetSelectProgress(_selectProgress);
			SetPressProgress(_pressProgress);
			SetHoverProgress(_hoverProgress);
			SetShowProgress(_showProgress);

			SetState(State.Hidden, State.Shown);
		}

		/// <summary>
		///     Change the visibility of this element.
		/// </summary>
		/// <param name="state"><see langword="true" /> if the element should be visible.</param>
		/// <param name="immediate"><see langword="true" /> if the transition animation should be skipped.</param>
		public void SetVisible(bool state, bool immediate = false)
		{
			if (immediate)
			{
				State = state ? State.Shown : State.Hidden;
				return;
			}

			if (state)
			{
				if (State != State.Shown)
					State = State.Showing;
			}
			else
			{
				if (State != State.Hidden)
					State = State.Hiding;
			}
		}

		/// <summary>
		///     Update the element state.
		/// </summary>
		protected virtual void Update()
		{
			var t = Time.deltaTime;

			if (State == State.Showing)
			{
				ShowProgress = ShowDuration > 0 ? Mathf.Clamp01(ShowProgress + t / ShowDuration) : 1f;
				if (ShowProgress == 1f)
					State = State.Shown;
			}
			else if (State == State.Hiding)
			{
				ShowProgress = ShowDuration > 0 ? Mathf.Clamp01(ShowProgress - t / ShowDuration) : 0f;
				if (ShowProgress == 0f)
					State = State.Hidden;
			}

			if (Hover)
				HoverProgress = HoverOnDuration > 0 ? Mathf.Clamp01(HoverProgress + t / HoverOnDuration) : 1f;
			else
				HoverProgress = HoverOutDuration > 0 ? Mathf.Clamp01(HoverProgress - t / HoverOutDuration) : 0f;

			if (Pressed)
				PressProgress = 1f;
			else
				PressProgress = PressDuration > 0 ? Mathf.Clamp01(PressProgress - t / PressDuration) : 0f;

			SelectProgress = Selected ? 1f : 0f;
		}

		/// <summary>
		///     Called when the show/hide animation progress changes.
		/// </summary>
		protected virtual void SetShowProgress(float progress)
		{
		}

		/// <summary>
		///     Called when the hover animation progress changes.
		/// </summary>
		protected virtual void SetHoverProgress(float progress)
		{
		}

		/// <summary>
		///     Called when the press animation progress changes.
		/// </summary>
		protected virtual void SetPressProgress(float progress)
		{
		}

		/// <summary>
		///     Called when the select animation progress changes.
		/// </summary>
		protected virtual void SetSelectProgress(float progress)
		{
		}

		/// <summary>
		///     Called when the visibility state changes.
		/// </summary>
		protected virtual void SetState(State currentState, State previousState)
		{
			if (previousState == State.Hidden)
				gameObject.SetActive(true);
			else if (currentState == State.Hidden)
			{
				PressProgress = 0f;
				HoverProgress = 0f;
				ShowProgress = 0f;

				gameObject.SetActive(false);
			}

			if (previousState == State.Shown)
			{
				if (_hover)
				{
					_hover = false;
					SetHover(_hover);
				}

				if (_pressed)
				{
					_pressed = false;
					SetPressed(_pressed);
				}
			}
		}

		/// <summary>
		///     Called when the hover state changes.
		/// </summary>
		protected virtual void SetHover(bool state)
		{
		}

		/// <summary>
		///     Called when the pressed state changes.
		/// </summary>
		protected virtual void SetPressed(bool state)
		{
			if ((OnPress != null) && !Disabled && (State == State.Shown) && !state)
				OnPress();
		}

		/// <summary>
		///     Called when the selected state changes.
		/// </summary>
		protected virtual void SetSelected(bool state)
		{
		}

		/// <summary>
		///     Called when the disabled state changes.
		/// </summary>
		protected virtual void SetDisabled(bool state)
		{
			if (state)
			{
				if (_hover)
				{
					_hover = false;
					SetHover(_hover);
				}

				if (_pressed)
				{
					_pressed = false;
					SetPressed(_pressed);
				}
			}
		}
	}
}
