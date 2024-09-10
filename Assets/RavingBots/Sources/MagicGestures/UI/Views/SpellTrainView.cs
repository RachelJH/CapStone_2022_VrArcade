using System.Collections.Generic;

using RavingBots.MagicGestures.AI;
using RavingBots.MagicGestures.Controller;
using RavingBots.MagicGestures.Game;
using RavingBots.MagicGestures.UI.Elements;

using UnityEngine;

namespace RavingBots.MagicGestures.UI.Views
{
	/// <summary>
	///     The gesture training view.
	/// </summary>
	public class SpellTrainView : View
	{
		/// <summary>
		///     The gesture preview slot.
		/// </summary>
		[SerializeField]
		protected SpellSlot PreviewSlot;

		/// <summary>
		///     The label for the number of captured samples.
		/// </summary>
		[SerializeField]
		protected Label SamplesLabel;
		/// <summary>
		///     The progress bar for captured samples.
		/// </summary>
		[SerializeField]
		protected ProgressBar SamplesProgress;
		/// <summary>
		///     The label for the training progress.
		/// </summary>
		[SerializeField]
		protected Label TrainingLabel;
		/// <summary>
		///     The progress bar for the training.
		/// </summary>
		[SerializeField]
		protected ProgressBar TrainingProgress;

		/// <summary>
		///     The captured samples.
		/// </summary>
		private readonly List<GestureData> _samples = new List<GestureData>();

		/// <summary>
		///     <see langword="true" /> if the gesture is ready.
		/// </summary>
		private bool IsReady
		{
			get { return _samples.Count == GestureLearner.Instance.SamplesPerGesture; }
		}

		/// <inheritdoc />
		public override void SetVisible(bool state, bool immediate = false)
		{
			if (state)
				ResetTraining();

			base.SetVisible(state, immediate);
		}

		/// <summary>
		///     Called after the user performs a gesture, to capture a sample.
		/// </summary>
		public void OnGesture(GestureData gesture)
		{
			if (IsReady)
				return;

			if (gesture.Points.Length == 0 || _samples.Count > 0 && gesture.HandCount != _samples[0].HandCount)
			{
				return;
			}

			_samples.Add(gesture);

			UpdateSamplesProgress();

			var archetype = MagicMenu.Instance.SpellBookView.SelectedSlot.SpellData;
			PreviewSlot.SpellData = new SpellData(archetype, _samples);

			if (IsReady)
				RunTraining();
		}

		/// <summary>
		///     Reset the training progress.
		/// </summary>
		private void ResetTraining()
		{
			_samples.Clear();
			PreviewSlot.SpellData = null;

			UpdateSamplesProgress();
			UpdateTrainingProgress();
		}

		/// <summary>
		///     Save the spell in the book and start the training.
		/// </summary>
		private void RunTraining()
		{
			WandManager.Instance.MenuMode = true;

			var spellBook = MagicMenu.Instance.SpellBookView;
			spellBook.SelectedSlot.SpellData = PreviewSlot.SpellData;

			var result = GestureLearner.Instance.StartTraining(spellBook.GetTrainingData());
			Debug.Assert(result);
		}

		/// <summary>
		///     Get the current training progress.
		/// </summary>
		private float GetTrainingProgress()
		{
			if (!IsReady)
				return 0f;

			var learner = GestureLearner.Instance;
			var status = learner.Status;

			if (status.Successful)
				return 1f;

			var iterationProgress = status.Iteration / (float)learner.MaxIterations;

			if (status.Error < 0f)
				return iterationProgress;

			var errorProgress = Mathf.Clamp01(Mathf.InverseLerp(1f, learner.TargetError, status.Error));

			return Mathf.Max(iterationProgress, errorProgress);
		}

		/// <summary>
		///     Update the text and the progress bar for the captured samples.
		/// </summary>
		private void UpdateSamplesProgress()
		{
			SamplesLabel.Text.text = string.Format(
				"Training Samples: {0}/{1}",
				_samples.Count,
				GestureLearner.Instance.SamplesPerGesture);
			SamplesProgress.Progress = _samples.Count / (float)GestureLearner.Instance.SamplesPerGesture;
		}

		/// <summary>
		///     Update the text and the progress bar for the training.
		/// </summary>
		private void UpdateTrainingProgress()
		{
			var progress = GetTrainingProgress();

			TrainingLabel.Text.text = string.Format("Training Progress: {0}%", Mathf.FloorToInt(100f * progress));
			TrainingProgress.Progress = progress;
		}

		/// <summary>
		///     Update the visual state when the training is running.
		/// </summary>
		protected void Update()
		{
			if (IsReady)
			{
				UpdateTrainingProgress();

				if (!GestureLearner.Instance.Status.IsRunning)
				{
					var status = GestureLearner.Instance.Status;
					Debug.LogFormat("Training error: {0}, iterations: {1}", status.Error, status.Iteration);

					_samples.Clear();
					GoBack();
				}
			}
		}

		/// <inheritdoc />
		public override void GoBack()
		{
			if (IsReady)
				return;

			base.GoBack();

			var menu = MagicMenu.Instance;
			var spellBook = menu.SpellBookView;

			menu.CurrentView = spellBook.SelectedSlot.IsValid ? (View)menu.SpellEditView : menu.SpellBookView;
		}
	}
}
