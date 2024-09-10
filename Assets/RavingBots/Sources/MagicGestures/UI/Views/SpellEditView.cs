using System.Linq;
using RavingBots.MagicGestures.Game;
using RavingBots.MagicGestures.UI.Elements;
using UnityEngine;

namespace RavingBots.MagicGestures.UI.Views
{
	/// <summary>
	///     The spell configuration view.
	/// </summary>
	public class SpellEditView : View
	{
		/// <summary>
		///     The prefab for the color slots.
		/// </summary>
		[SerializeField] protected ColorSlot ColorSlotPrefab;

		/// <summary>
		///     The number of color slots.
		/// </summary>
		[SerializeField] protected int ColorSlotCount = 12;

		/// <summary>
		///     The radius of the circle on which slots will be laid out.
		/// </summary>
		[SerializeField] protected float ColorSlotRadius = 1f;

		/// <summary>
		///     Slot colors are interpolated between
		///     three colors based on the position of the slot.
		/// </summary>
		[SerializeField] protected Color SlotColor1 = Color.red;

		/// <inheritdoc cref="SlotColor1" />
		[SerializeField] protected Color SlotColor2 = Color.green;

		/// <inheritdoc cref="SlotColor1" />
		[SerializeField] protected Color SlotColor3 = Color.blue;

		/// <summary>
		///     The prefab for the effect slots.
		/// </summary>
		[SerializeField] protected EffectSlot EffectSlotPrefab;

		/// <summary>
		///     The radius of the effect slots.
		/// </summary>
		[SerializeField] protected float EffectSlotRadius = 2f;

		/// <summary>
		///     The gesture preview slot.
		/// </summary>
		[SerializeField] protected SpellSlot PreviewSlot;

		/// <summary>
		///     All colors slots.
		/// </summary>
		protected ColorSlot[] ColorSlots { get; private set; }

		/// <summary>
		///     All effects slots.
		/// </summary>
		protected EffectSlot[] EffectSlots { get; private set; }

		/// <inheritdoc />
		protected override void Awake()
		{
			ColorSlots = CreateColorSlots();
			EffectSlots = CreateEffectSlots();

			base.Awake();
		}

		protected void Start()
		{
			PreviewSlot.OnPress += () => MagicMenu.Instance.CurrentView = MagicMenu.Instance.SpellTrainView;
		}

		/// <summary>
		///     Get a random color slot.
		/// </summary>
		public ColorSlot GetRandomColorSlot()
		{
			return ColorSlots[Random.Range(0, ColorSlots.Length)];
		}

		/// <summary>
		///     Create all of the color slots in a circle.
		/// </summary>
		private ColorSlot[] CreateColorSlots()
		{
			var t = PreviewSlot.transform;

			var circle = ViewUtils.InstantiateInCircle(
				ColorSlotPrefab,
				transform,
				ColorSlotCount,
				ColorSlotRadius,
				t.localPosition,
				t.forward,
				t.up);
			var result = circle.ToArray();

			const float third = 360f / 3;
			var angle = 360f / ColorSlotCount;

			for (var i = 0; i < result.Length; i++)
			{
				var slot = result[i];
				var a = angle * i;
				if ((a >= 0f) && (a < third))
					slot.Color = Color.Lerp(SlotColor1, SlotColor2, a / third);
				else if ((a >= third) && (a < 2 * third))
					slot.Color = Color.Lerp(SlotColor2, SlotColor3, (a - third) / third);
				else
					slot.Color = Color.Lerp(SlotColor3, SlotColor1, (a - 2 * third) / third);

				slot.OnPress += () => SetColor(slot.Color);
			}

			return result;
		}

		/// <summary>
		///     Create all of the effect slots.
		/// </summary>
		private EffectSlot[] CreateEffectSlots()
		{
			var t = PreviewSlot.transform;

			var circle = ViewUtils.InstantiateInCircle(
				EffectSlotPrefab,
				transform,
				MagicGame.Instance.EffectCount,
				EffectSlotRadius,
				t.localPosition,
				t.forward,
				(-t.right + t.up).normalized);
			var result = circle.ToArray();

			for (var i = 0; i < result.Length; i++)
			{
				var slot = result[i];
				slot.EffectId = i;

				slot.OnPress += () => SetEffect(slot.EffectId);
			}

			return result;
		}

		/// <inheritdoc />
		public override void SetVisible(bool state, bool immediate = false)
		{
			if (state)
			{
				PreviewSlot.SpellData = MagicMenu.Instance.SpellBookView.SelectedSlot.SpellData;
				UpdateSelectedColor();
				UpdateSelectedEffect();
			}
			else
				MagicMenu.Instance.SpellBookView.SelectedSlot.SpellData = PreviewSlot.SpellData;

			base.SetVisible(state, immediate);
		}

		/// <summary>
		///     Change the selected color.
		/// </summary>
		private void SetColor(Color color)
		{
			PreviewSlot.SpellData = new SpellData(PreviewSlot.SpellData) {Color = color};
			UpdateSelectedColor();
		}

		/// <summary>
		///     Change the selected effect.
		/// </summary>
		private void SetEffect(int id)
		{
			PreviewSlot.SpellData = new SpellData(PreviewSlot.SpellData) {EffectId = id};
			UpdateSelectedEffect();
		}

		/// <summary>
		///     Update the slots after a new color is selected.
		/// </summary>
		private void UpdateSelectedColor()
		{
			foreach (var colorSlot in ColorSlots)
				colorSlot.Selected = colorSlot.Color == PreviewSlot.SpellData.Color;

			foreach (var effectSlot in EffectSlots)
				effectSlot.MagicEffect.Color = PreviewSlot.SpellData.Color;
		}

		/// <summary>
		///     Update the slots after a new effect is selected.
		/// </summary>
		private void UpdateSelectedEffect()
		{
			foreach (var effectSlot in EffectSlots)
				effectSlot.Selected = effectSlot.EffectId == PreviewSlot.SpellData.EffectId;
		}

		/// <inheritdoc />
		public override void GoBack()
		{
			base.GoBack();

			MagicMenu.Instance.CurrentView = MagicMenu.Instance.SpellBookView;
		}
	}
}
