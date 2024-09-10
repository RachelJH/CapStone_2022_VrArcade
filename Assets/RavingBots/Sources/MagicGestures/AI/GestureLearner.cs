using System.Collections.Generic;
using RavingBots.MagicGestures.AI.Common;
using RavingBots.MagicGestures.AI.Neural.Classic;
using RavingBots.MagicGestures.Controller;
using RavingBots.MagicGestures.Game;
using RavingBots.MagicGestures.UI.Views;
using RavingBots.MagicGestures.Utils;
using UnityEngine;

namespace RavingBots.MagicGestures.AI
{
	/// <summary>
	///     The configuration store and the API to learn and recognize the gestures.
	/// </summary>
	/// <remarks>
	///     <para>
	///         This component keeps the neural network and the gesture preprocessor instance (both of which
	///         are configured through the public fields) and exposes them so they can be used by the rest
	///         of the project.
	///     </para>
	///     <para>
	///         Most of the actual work is done in the linked classes (a notable exception is
	///         the work done by <see cref="Recognize" /> method, which finds a gesture based on
	///         the input and the detection threshold set in <see cref="RecognitionThreshold" />).
	///     </para>
	/// </remarks>
	/// <seealso cref="MultilayerPerceptron" />
	/// <seealso cref="GesturePreprocessor" />
	/// <seealso cref="Backpropagation" />
	public class GestureLearner : MonoSingleton<GestureLearner>
	{
		/// <summary>
		///     The number of gesture repeats used for training a single spell. Each repeated gesture is
		///     added to the training set. The neural network recognises spells by generalising patterns provided
		///     in the training set.
		/// </summary>
		/// <remarks>
		///     The bigger the training set, the better tolerance and accuracy of recognition.
		/// </remarks>
		/// <seealso cref="SpellTrainView" />
		[Tooltip("The number of gesture repeats used for training a single spell.")] [Range(1, 10)]
		public int SamplesPerGesture = 5;

		/// <summary>
		///     The training process is stopped when the training error reaches the specified value.
		/// </summary>
		/// <remarks>
		///     The smaller the error value, the better accuracy of correctly recognising gestures provided in the training set.
		/// </remarks>
		[Tooltip("The training process is stopped when the training error reaches the specified value.")]
		[Range(0f, 1f)]
		public float TargetError = 0.025f;

		/// <summary>
		///     The training is continued for the specified number of iterations unless the target error is reached
		///     earlier.
		/// </summary>
		/// <remarks>
		///     The more iterations, the better chance to reach the target error.
		/// </remarks>
		/// <seealso cref="Backpropagation" />
		[Tooltip(
			"The training is continued for the specified number of " +
			"iterations unless the target error was reached earlier.")]
		[Range(1, 5000)]
		public int MaxIterations = 100;

		/// <summary>
		///     The learning rate is used by the backpropagation training algorithm. The parameter determines the speed
		///     of the training.
		/// </summary>
		/// <remarks>
		///     If the value is too small, the training may require a large number of training iterations. However,
		///     if it is too high, the training error may oscillate and never reach the target value.
		/// </remarks>
		/// <seealso cref="Backpropagation" />
		[Tooltip("The learning rate used by the backpropagation training algorithm.")] [Range(0f, 1f)]
		public float LearningRate = 0.3f;

		/// <summary>
		///     The momentum parameter is used by the backpropagation training algorithm. The parameter accelerates
		///     the training process.
		/// </summary>
		/// <remarks>
		///     It is usually safe to use a high value, but in some cases, it may prevent the algorithm from reaching
		///     a minimum training error.
		/// </remarks>
		/// <seealso cref="Backpropagation" />
		[Tooltip("The momentum parameter used by the backpropagation training algorithm.")] [Range(0f, 1f)]
		public float Momentum = 0.8f;

		/// <summary>
		///     The number of hidden layers in the neural network.
		/// </summary>
		/// <remarks>
		///     In most cases, one or two hidden layers are enough to obtain satisfying results. The bigger the number of layers,
		///     the more neurons and connections must be processed by the neural network. As the number grows, the neural network
		///     loses its generalisation capabilities, and a larger training set must be provided.
		/// </remarks>
		/// <seealso cref="MultilayerPerceptron" />
		[Tooltip("The number of hidden layers in a neural network.")] [Range(0, 5)]
		public int HiddenLayers = 1;

		/// <summary>
		///     The threshold that must be crossed by one of the outputs of the neural network to identify a valid gesture.
		/// </summary>
		/// <remarks>
		///     Increase the value to ignore gestures that poorly resemble any of the spells. Lower the threshold if you want
		///     the recognition system to be less restrictive.
		/// </remarks>
		/// <seealso cref="Recognize" />
		[Tooltip(
			"A threshold that must be crossed by one of the outputs of a neural network to identify a valid gesture.")]
		[Range(0f, 1f)]
		public float RecognitionThreshold = 0.4f;

		/// <summary>
		///     The width of the grid onto which the gesture is projected during the preprocessing phase.
		/// </summary>
		/// <remarks>
		///     The lower resolution of the grid, the better performance of the recognition system. However, a low resolution
		///     may prevent effectively training complex gestures. You can increase the resolution of the grid, but remember
		///     to provide a sufficient number of gesture repeats for the neural network to capture the pattern.
		/// </remarks>
		/// <seealso cref="GesturePreprocessor" />
		[Tooltip("The width of a grid onto which a gesture is projected during the preprocessing phase.")]
		[Range(1, 20)]
		public int GridResolutionX = 6;

		/// <summary>
		///     The height of the grid onto which the gesture is projected during the preprocessing phase.
		/// </summary>
		/// <remarks>
		///     The lower resolution of the grid, the better performance of the recognition system. However, a low resolution
		///     may prevent effectively training complex gestures. You can increase the resolution of the grid, but remember
		///     to provide a sufficient number of gesture repeats for the neural network to capture the pattern.
		/// </remarks>
		/// <seealso cref="GesturePreprocessor" />
		[Tooltip("The height of a grid onto which a gesture is projected during the preprocessing phase.")]
		[Range(1, 20)]
		public int GridResolutionY = 6;

		/// <summary>
		///     The depth of the grid onto which the gesture is projected during the preprocessing phase.
		/// </summary>
		/// <remarks>
		///     The lower resolution of the grid, the better performance of the recognition system. However, a low resolution
		///     may prevent effectively training complex gestures. You can increase the resolution of the grid, but remember
		///     to provide a sufficient number of gesture repeats for the neural network to capture the pattern.
		/// </remarks>
		/// <seealso cref="GesturePreprocessor" />
		[Tooltip("The depth of a grid onto which a gesture is projected during the preprocessing phase.")]
		[Range(1, 20)]
		public int GridResolutionZ = 1;

		/// <summary>
		///     The current status of the training.
		/// </summary>
		/// <seealso cref="Backpropagation" />
		public Backpropagation.Status Status
		{
			get { return _backpropagation.CurrentStatus; }
		}

		/// <summary>
		///     The neural network used for recognizing the gestures.
		/// </summary>
		/// <remarks>
		///     <para>
		///         Because the training happens in the background, and the network
		///         is not usable until the training is complete, we distinguish between
		///         the network being trained and the network being used to recognize the gestures.
		///         After the training is complete, this instance will be replaced by the
		///         other one.
		///     </para>
		/// </remarks>
		private MultilayerPerceptron[] _mlp;

		/// <inheritdoc cref="_mlp" />
		public MultilayerPerceptron[] MLP
		{
			get
			{
				if (!Status.IsRunning && (_trainedMLP != null))
				{
					_mlp[HandCount - 1] = _trainedMLP;
					_trainedMLP = null;
				}

				return _mlp;
			}
			set { _mlp = value; }
		}


		/// <summary>
		///     The preprocessor instance used.
		/// </summary>
		private GesturePreprocessor _gesturePreprocessor;

		/// <summary>
		///     The training algorithm used.
		/// </summary>
		private Backpropagation _backpropagation;

		/// <summary>
		///     The most recently input gesture, used to draw the debug grid.
		/// </summary>
		/// <seealso cref="GesturePreprocessor.DrawGizmos" />
		/// <seealso cref="OnDrawGizmos" />
		private GestureData _gizmoGesture;

		/// <summary>
		///     The neural network in training.
		/// </summary>
		/// <remarks>
		///     May be <see langword="null" /> if no training is in progress.
		/// </remarks>
		/// <seealso cref="StartTraining" />
		private MultilayerPerceptron _trainedMLP;

		/// <summary>
		///     Amount of hands used for the gesture.
		/// </summary>
		[HideInInspector]
		public int HandCount = 1;

		/// <summary>
		///     Configure the component.
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			_gesturePreprocessor = CreateGesturePreprocessor(HandCount);
			_backpropagation = new Backpropagation(new System.Random(0));
		}

		/// <summary>
		///     Start the training process.
		/// </summary>
		/// <remarks>
		///     <note type="important">
		///         The training process is asynchronous. This method will not block until the training
		///         is complete.
		///     </note>
		///     <para>
		///         This method creates a new neural network and trains it using the <see cref="_backpropagation" />
		///         object. The network instance is assigned to <see cref="_trainedMLP" /> and is not used
		///         for recognition until the training completes.
		///     </para>
		///     <para>
		///         Note that <paramref name="spells" /> must contain all known spells, not just the most
		///         recently configured one, because a fresh network is being created every time this
		///         method is called.
		///     </para>
		/// </remarks>
		/// <param name="spells">The spells to teach the network.</param>
		/// <param name="handCount">Amount of hands the gesture is performed with</param>
		/// <returns>Whether the settings were valid and the training was started.</returns>
		/// <seealso cref="MultilayerPerceptron" />
		/// <seealso cref="Backpropagation.RunAsyncTraining" />
		/// <seealso cref="CreateSamples" />
		/// <seealso cref="SpellData" />
		public bool StartTraining(SpellData[] spells)
		{
			for (var i = 0; i <  MLP.Length; i++)
			{
				_gesturePreprocessor = CreateGesturePreprocessor(i+1);

				var samples = CreateSamples(spells, _gesturePreprocessor);
				if (!SampleData.IsValidTrainingSet(samples))
					continue;

				Debug.Log("Training set:\n" + SampleData.ToString(samples));

				var mlpSettings = MultilayerPerceptron.Settings.GetDefault(samples[0], HiddenLayers);
				if (!mlpSettings.IsValid)
					continue;

				var bpSettings = GetBPSettings();
				if (!bpSettings.IsValid)
					continue;

				_trainedMLP = new MultilayerPerceptron(mlpSettings);
				Debug.Log("Created:\n" + _trainedMLP);
				_backpropagation.RunAsyncTraining(_trainedMLP, samples, bpSettings);

				HandCount = i+1;
				_mlp[i] = _trainedMLP;
				_trainedMLP = null;
			}

			return true;
		}

		/// <summary>
		///     Request the training in progress to be cancelled.
		/// </summary>
		/// <remarks>
		///     Can be safely called when no training is currently running,
		///     in which case it will do nothing.
		/// </remarks>
		/// <seealso cref="Backpropagation.StopAsyncTraining" />
		public void StopTraining()
		{
			_backpropagation.StopAsyncTraining();
		}

		/// <summary>
		///     Try to recognize a gesture.
		/// </summary>
		/// <remarks>
		///     <para>
		///         This method preprocesses the input with <see cref="GesturePreprocessor" />,
		///         and then feeds it to the <see cref="MultilayerPerceptron" />. The network has
		///         1 output for each configured spell, which denotes how close the input gesture
		///         is to that spell.
		///     </para>
		///     <para>
		///         After calling <see cref="MultilayerPerceptron.Process" />
		///         the output which has the highest value and is above <see cref="RecognitionThreshold" />
		///         corresponds to the recognized spell.
		///     </para>
		/// </remarks>
		/// <param name="gesture">The gesture to recognize.</param>
		/// <param name="output">
		///     Set to the output of the neural network, or 0 if none of the outputs
		///     are above <see cref="RecognitionThreshold" />.
		/// </param>
		/// <returns>
		///     The index of the gesture in the spell array given to <see cref="StartTraining" />.
		///     -1 if the input gesture has not been recognized or the network is not yet ready.
		/// </returns>
		public int Recognize(GestureData gesture, out float output)
		{
			output = 0f;
			HandCount = gesture.HandCount;
			var index = gesture.HandCount - 1;

			_gesturePreprocessor = CreateGesturePreprocessor(HandCount);
			_gizmoGesture = gesture;

			var input = _gesturePreprocessor.GetInput(gesture);

			if ((MLP == null) || (MLP[index].InputCount != input.Length))
			{
				Debug.Log("Neural network not ready");
				return -1;
			}

			MLP[index].Process(input);

			var bestIndex = -1;

			for (var i = 0; i < MLP[index].Output.Length; i++)
			{
				var o = MLP[index].Output[i];
				if (o < RecognitionThreshold)
					continue;

				if ((bestIndex < 0) || (output < o))
				{
					bestIndex = i;
					output = o;
				}
			}

			return bestIndex;
		}

		/// <summary>
		///     Convert a list of spells to a list of training samples.
		/// </summary>
		/// <remarks>
		///     <para>
		///         This method preprocesses all configured spells into a form usable
		///         by the neural network training algorithm.
		///     </para>
		///     <para>
		///         Every sample has N inputs and M outputs, where N is the number of
		///         the grid cells (see <see cref="GesturePreprocessor.GetInput" />) and M
		///         is the number of items in the <paramref name="spells" /> array.
		///         The neural network will be created with the same number of inputs and outputs.
		///     </para>
		/// </remarks>
		/// <returns>
		///     A training set that can be used with <see cref="Backpropagation.RunAsyncTraining" />.
		/// </returns>
		/// <seealso cref="Backpropagation" />
		/// <seealso cref="SampleData" />
		/// <seealso cref="GesturePreprocessor" />
		private static SampleData[] CreateSamples(SpellData[] spells, GesturePreprocessor preprocessor)
		{
			if ((spells == null) || (preprocessor == null))
				return null;

			var result = new List<SampleData>();

			for (var i = 0; i < spells.Length; i++)
			{
				var spell = spells[i];
				foreach (var gesture in spell.Gestures)
				{
					if (gesture.HandCount != preprocessor.Matrix.GetLength(0)) continue;
					var input = preprocessor.GetInput(gesture);
					var output = new float[spells.Length];
					output[i] = 1f;

					result.Add(new SampleData(input, output));
				}
			}

			return result.ToArray();
		}

		/// <summary>
		///     Create a preprocessor instance using the stored configuration.
		/// </summary>
		private GesturePreprocessor CreateGesturePreprocessor(int handCount)
		{
			return new GesturePreprocessor(new Vector3Int(GridResolutionX, GridResolutionY, GridResolutionZ),
				handCount);
		}

		/// <summary>
		///     Create backpropagation settings using the stored configuration.
		/// </summary>
		private Backpropagation.Settings GetBPSettings()
		{
			Backpropagation.Settings result;

			result.LearningRate = LearningRate;
			result.Momentum = Momentum;
			result.TargetError = TargetError;
			result.MaxIterations = MaxIterations;
			result.RandomWeightRange = new Vector2(-1f, 1f);

			return result;
		}

		/// <summary>
		///     Cancel the training when this component is disabled.
		/// </summary>
		protected void OnDisable()
		{
			StopTraining();
		}

		/// <summary>
		///     Draw the debug grid based on most recently attempted gesture.
		/// </summary>
		/// <seealso cref="GesturePreprocessor.DrawGizmos" />
		protected void OnDrawGizmos()
		{
			if ((_gesturePreprocessor == null) || (_gizmoGesture == null) || _gesturePreprocessor.Matrix.GetLength(0) != _gizmoGesture.HandCount)
				return;

			_gesturePreprocessor.DrawGizmos(_gizmoGesture, transform.position, transform.localScale.x);
		}
	}
}
