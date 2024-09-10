using System.Collections;
using System.Linq;
using RavingBots.MagicGestures.Controller;
using RavingBots.MagicGestures.Game;
using RavingBots.MagicGestures.UI.Elements;
using UnityEngine;

namespace RavingBots.MagicGestures.UI.Views
{
	/// <summary>
	///     The spellbook view.
	/// </summary>
	public class SpellBookView : View
	{
		/// <summary>
		///     The prefab for the spell slots.
		/// </summary>
		[SerializeField] protected SpellSlot SlotPrefab;

		/// <summary>
		///     The width of the spell slot grid.
		/// </summary>
		[SerializeField] protected int SlotWidth = 3;

		/// <summary>
		///     The height of the spell slot grid.
		/// </summary>
		[SerializeField] protected int SlotHeight = 3;

		/// <summary>
		///     Spacing between the slots in the grid.
		/// </summary>
		[SerializeField] protected float SlotSpacing = 2f;

		/// <summary>
		///     The currently selected slot.
		/// </summary>
		public SpellSlot SelectedSlot { get; private set; }

		/// <summary>
		///     Slots where <see cref="SpellSlot.IsValid" /> is <see langword="true" />.
		/// </summary>
		public SpellSlot[] ValidSlots { get; private set; }

		/// <summary>
		///     The array of known spells.
		/// </summary>
		public SpellData[] Spells
		{
			get { return _spellSlots.Select(s => s.SpellData).ToArray(); }
			set
			{
				for (var i = 0; i < _spellSlots.Length; i++)
					if (i < value.Length)
						_spellSlots[i].SpellData = value[i];
					else
					{
						var spellData = new SpellData();

						spellData.Color = MagicMenu.Instance.SpellEditView.GetRandomColorSlot().Color;
						spellData.EffectId = MagicGame.Instance.GetRandomEffectId();

						_spellSlots[i].SpellData = spellData;
					}

				UpdateValidSlots();
			}
		}

		/// <summary>
		///     Created spell slots.
		/// </summary>
		private SpellSlot[] _spellSlots;

		/// <inheritdoc />
		protected override void Awake()
		{
			StartCoroutine(WaitMenu());

		}

		/// <summary>
		///     Register the event handlers.
		/// </summary>
		protected void Start()
		{
			WandManager.Instance.OnGestureRecognizedEvent += CastSpell;
		}

		/// <summary>
		///     Update <see cref="ValidSlots" />.
		/// </summary>
		private void UpdateValidSlots()
		{
			ValidSlots = _spellSlots.Where(s => s.IsValid).ToArray();
		}

		/// <summary>
		///     Get the spell data from valid slots.
		/// </summary>
		public SpellData[] GetTrainingData()
		{
			UpdateValidSlots();

			return ValidSlots.Select(s => s.SpellData).ToArray();
		}

		/// <summary>
		///     Wait for menu to be ready before creating spell slots.
		/// </summary>
		IEnumerator WaitMenu()
		{
			yield return new WaitUntil(() => MagicMenu.Instance != null);

			_spellSlots = CreateSpellSlots();
			base.Awake();
		}

		/// <summary>
		///     Create all of the slots.
		/// </summary>
		SpellSlot[] CreateSpellSlots()
		{
			var menu = MagicMenu.Instance;

			var grid = ViewUtils.InstantiateCurvedGrid(
				SlotPrefab,
				transform,
				SlotWidth,
				SlotHeight,
				menu.DistanceToCamera,
				SlotSpacing,
				-menu.DistanceToCamera * menu.transform.forward);
			var result = grid.ToArray();

			// ReSharper disable once ForCanBeConvertedToForeach
			for (var i = 0; i < result.Length; i++)
			{
				var slot = result[i];
				slot.OnPress += () =>
				{
					SelectedSlot = slot;
					menu.CurrentView = slot.IsValid ? (View) menu.SpellEditView : menu.SpellTrainView;
				};
			}

			return _spellSlots = result;
		}

		/// <summary>
		///     Cast the recognized spell.
		/// </summary>
		/// <remarks>
		///     This is registered as a handler for the <see cref="WandManager.OnGestureRecognizedEvent" /> event.
		/// </remarks>
		/// <param name="wand">The wand used to perform the gesture.</param>
		/// <param name="gesture">The performed gesture.</param>
		/// <param name="spellId">The index of the recognized gesture.</param>
		public void CastSpell(MagicWand wand, GestureData gesture, int spellId)
		{
			Debug.Assert((spellId >= 0) && (spellId < ValidSlots.Length));

			var slot = ValidSlots[spellId];

			MorphTrail(WandManager.Instance.GestureTracker.Trail, slot, gesture,
				WandManager.Instance.CameraRotationReference);
			var result = wand.SpellCaster.LoadSpell(slot.SpellData.EffectId, slot.SpellData.Color);

			Debug.Assert(result);
		}

		/// <summary>
		///     Morph the drawn gesture into the recognized shape.
		/// </summary>
		private void MorphTrail(GestureTrail[] trails, SpellSlot slot, GestureData gesture,
			Quaternion rotationReference)
		{
			for (var index = 0; index < trails.Length; index++)
			{
				var trail = trails[index];
				if (trail == null) continue;

				var morphGesture = slot.PreviewGesture.GetResampled(trail.LineRenderer.positionCount);
				if (morphGesture != null)
				{
					foreach (var points in morphGesture.Points)
						for (var j = 0; j < points.Length(); j++)
							points[j] = rotationReference * points[j] * gesture.Radius +
										gesture.Limits.Center;

					trail.SetMorph(morphGesture.Points[Mathf.Min(index, morphGesture.Points.Length - 1)],
						slot.SpellData.Color);
				}
			}
		}

		/// <inheritdoc />
		public override void GoBack()
		{
			base.GoBack();

			MagicMenu.Instance.CurrentView = null;
		}
	}
}
