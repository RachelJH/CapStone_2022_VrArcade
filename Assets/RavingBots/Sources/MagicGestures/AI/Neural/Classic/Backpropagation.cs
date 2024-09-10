using System;
using System.Threading;

using RavingBots.MagicGestures.AI.Common;
using RavingBots.MagicGestures.Utils;

using UnityEngine;

namespace RavingBots.MagicGestures.AI.Neural.Classic
{
	/// <summary>
	///     The backpropagation (with inertia) training algorithm.
	/// </summary>
	/// <remarks>
	///     <para>
	///         We train the neural network using the backpropagation algorithm.
	///         For every sample in the training set the input values are propagated
	///         forwards through all of the layers (<see cref="MultilayerPerceptron.Process" />,
	///         then an error value is calculated (<see cref="CalcError" />) and propagated
	///         backwards from the output layer to the input layer (<see cref="PropagateError" />),
	///         and finally weights on the neurons are adjusted (<see cref="AdjustWeights(Layer[],float[],float,float)" />.
	///         This is the stochastic mode of learning, where every propagation results in the weight update.
	///     </para>
	/// </remarks>
	/// <seealso cref="Settings" />
	/// <seealso cref="RunTraining" />
	public class Backpropagation
	{
		/// <summary>
		///     The configuration of the algorithm.
		/// </summary>
		/// <remarks>
		///     The algorithm runs for a set number of iterations (<see cref="MaxIterations" />) or
		///     until the error value drops below a set target (<see cref="TargetError" />). The speed and
		///     the accuracy of the training are tuned by two parameters: the learning rate (<see cref="LearningRate" />)
		///     which determines whether the network should learn faster or more accurately, and the momentum
		///     (<see cref="Momentum" />) which determines how much a weight update should additionally depend on
		///     a previous change.
		/// </remarks>
		public struct Settings
		{
			/// <inheritdoc cref="GestureLearner.LearningRate" />
			public float LearningRate;
			/// <inheritdoc cref="GestureLearner.Momentum" />
			public float Momentum;
			/// <inheritdoc cref="GestureLearner.TargetError" />
			public float TargetError;
			/// <inheritdoc cref="GestureLearner.MaxIterations" />
			public int MaxIterations;
			/// <summary>
			///     The range from which the initial random weights are selected.
			/// </summary>
			public Vector2 RandomWeightRange;

			/// <summary>
			///     <see langword="true" /> if the chosen settings are correct.
			/// </summary>
			public bool IsValid
			{
				get
				{
					return (LearningRate >= 0f) && (Momentum >= 0f) && (TargetError >= 0f) && (MaxIterations >= 0) &&
							(RandomWeightRange.x <= RandomWeightRange.y);
				}
			}

			/// <summary>
			///     The default settings.
			/// </summary>
			public static Settings Default
			{
				get
				{
					Settings result;
					result.LearningRate = 0.3f;
					result.Momentum = 0.9f;
					result.TargetError = 0.2f;
					result.MaxIterations = 1000;
					result.RandomWeightRange = new Vector2(-1f, 1f);
					return result;
				}
			}
		}

		/// <summary>
		///     The description of the training status.
		/// </summary>
		public struct Status
		{
			/// <summary>
			///     <see langword="true" /> if the training finished successfully.
			/// </summary>
			public volatile bool Successful;
			/// <summary>
			///     The current iteration of the training algorithm.
			/// </summary>
			public volatile int Iteration;
			/// <summary>
			///     The current error value.
			/// </summary>
			public volatile float Error;
			/// <summary>
			///     <see langword="true" /> if the training is running.
			/// </summary>
			public volatile bool IsRunning;
			/// <summary>
			///     <see langword="true" /> if the training is being interrupted.
			/// </summary>
			public volatile bool IsStopping;
		}

		/// <summary>
		///     The random number generator used in the training.
		/// </summary>
		public readonly System.Random Random;

		/// <summary>
		///     The current status of the training.
		/// </summary>
		private Status _currentStatus;

		/// <inheritdoc cref="_currentStatus" />
		public Status CurrentStatus
		{
			get { return _currentStatus; }
		}

		/// <summary>
		///     The handle to the training thread.
		/// </summary>
		public Thread CurrentThread { get; private set; }
		/// <summary>
		///     The settings used in the current training.
		/// </summary>
		public Settings CurrentSettings { get; private set; }

		/// <summary>
		///     The network being trained.
		/// </summary>
		private MultilayerPerceptron _mlp;
		/// <summary>
		///     The training set.
		/// </summary>
		private SampleData[] _samples;

		/// <summary>
		///     Construct a new instance with the given RNG.
		/// </summary>
		public Backpropagation(System.Random random)
		{
			_currentStatus = new Status { Error = -1f };
			Random = random;
		}

		/// <summary>
		///     Start training the given network.
		/// </summary>
		/// <remarks>
		///     <note type="important">
		///         This method does not wait until the training completes. If the previous training is
		///         still running, it will be interrupted.
		///     </note>
		///     <note type="important">
		///         The training set must be valid (<see cref="SampleData.IsValidTrainingSet" />).
		///         The neural network instance must match the training set (<see cref="NeuralNetwork.IsMatching" />).
		///         The settings must be valid (<see cref="Settings.IsValid" />).
		///     </note>
		/// </remarks>
		/// <param name="mlp">The network to train. Must not be <see langword="null" />.</param>
		/// <param name="samples">The training set.</param>
		/// <param name="settings">The settings to use.</param>
		/// <seealso cref="StopAsyncTraining" />
		/// <seealso cref="RunTraining" />
		public void RunAsyncTraining(MultilayerPerceptron mlp, SampleData[] samples, Settings settings)
		{
			Debug.Assert(SampleData.IsValidTrainingSet(samples));
			Debug.Assert((mlp != null) && mlp.IsMatching(samples[0]));
			Debug.Assert(settings.IsValid);

			StopAsyncTraining();

			_mlp = mlp;
			_samples = samples;
			CurrentSettings = settings;

			_currentStatus = new Status { IsRunning = true, Error = -1f };
			CurrentThread = new Thread(RunTraining);
			CurrentThread.Start();
		}

		/// <summary>
		///     Request the training in progress to be cancelled.
		/// </summary>
		/// <remarks>
		///     Can be safely called when no training is currently running,
		///     in which case it will do nothing.
		/// </remarks>
		/// <param name="sync">
		///     If <see langword="true" />, the call won't return until the training
		///     thread terminates.
		/// </param>
		public void StopAsyncTraining(bool sync = true)
		{
			if (!_currentStatus.IsRunning)
				return;

			Debug.Assert(!_currentStatus.IsStopping);

			_currentStatus.IsStopping = true;

			if (sync)
				CurrentThread.Join();
		}

		/// <summary>
		///     Run the backpropagation algorithm.
		/// </summary>
		/// <remarks>
		///     <para>This method also updates <see cref="_currentStatus" />.</para>
		///     <note type="important">
		///         This method runs on a background thread.
		///     </note>
		/// </remarks>
		/// <seealso cref="TrainSample" />
		/// <seealso cref="ResetNeurons" />
		private void RunTraining()
		{
			try
			{
				var shuffledSamples = (SampleData[])_samples.Clone();

				ResetNeurons(_mlp.Layers, Random, CurrentSettings.RandomWeightRange);

				for (; _currentStatus.Iteration < CurrentSettings.MaxIterations; _currentStatus.Iteration++)
				{
					if (_currentStatus.IsStopping)
						break;

					shuffledSamples.Shuffle(Random);

					var errorSum = 0f;
					foreach (var sample in shuffledSamples)
						errorSum += TrainSample(_mlp, sample, CurrentSettings.LearningRate, CurrentSettings.Momentum);

					_currentStatus.Error = errorSum / _samples.Length;
					if (_currentStatus.Error <= CurrentSettings.TargetError)
					{
						_currentStatus.Successful = true;
						break;
					}
				}
			}
			catch (ThreadAbortException)
			{
				Debug.LogFormat("Thread {0} aborted.", this);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
			finally
			{
				_mlp = null;
				_samples = null;

				_currentStatus.IsStopping = false;
				_currentStatus.IsRunning = false;
			}
		}

		/// <summary>
		///     Reset every neuron on every layer in the given set.
		/// </summary>
		/// <param name="layers">The network layers.</param>
		/// <param name="random">The random number generator to use.</param>
		/// <param name="weightRange">The range from which new weight values will be selected.</param>
		/// <seealso cref="Perceptron.Reset" />
		private static void ResetNeurons(Layer[] layers, System.Random random, Vector2 weightRange)
		{
			foreach (var layer in layers)
			foreach (var neuron in layer.Neurons)
				neuron.Reset(random, weightRange);
		}

		/// <summary>
		///     Run a single iteration of the backpropagation.
		/// </summary>
		/// <param name="mlp">The network being trained.</param>
		/// <param name="sample">The sample being processed.</param>
		/// <param name="learningRate">The learning rate parameter.</param>
		/// <param name="momentum">The momentum parameter.</param>
		/// <returns>The new mean error value.</returns>
		/// <seealso cref="MultilayerPerceptron.Process" />
		/// <seealso cref="PropagateError" />
		/// <seealso cref="AdjustWeights(Layer[],float[],float,float)" />
		private static float TrainSample(
			MultilayerPerceptron mlp,
			SampleData sample,
			float learningRate,
			float momentum)
		{
			mlp.Process(sample.Input);

			var meanSampleError = CalcError(mlp, sample.Output);
			PropagateError(mlp);

			AdjustWeights(mlp.Layers, sample.Input, learningRate, momentum);

			return meanSampleError;
		}

		/// <summary>
		///     Calculate new error values for the output layer.
		/// </summary>
		/// <remarks>
		///     <para>
		///         The error value for each neuron on the output layer is
		///         set based on the derivative of that neuron's activation function
		///         (<see cref="Activation" />) and the difference between expected output
		///         value and the actual one.
		///     </para>
		/// </remarks>
		/// <param name="mlp">The network being trained.</param>
		/// <param name="sampleOutput">The expected output values.</param>
		/// <returns>The mean sample error.</returns>
		public static float CalcError(MultilayerPerceptron mlp, float[] sampleOutput)
		{
			var sampleErrorSum = 0f;
			var outputLayer = mlp.Layers[mlp.Layers.Length - 1];

			Debug.Assert(outputLayer.Neurons.Length == sampleOutput.Length);

			for (var n = 0; n < outputLayer.Neurons.Length; n++)
			{
				var neuron = outputLayer.Neurons[n];
				var output = outputLayer.Output[n];

				Debug.Assert(neuron.Func.InRange(sampleOutput[n]));

				var diff = sampleOutput[n] - output;
				neuron.Error = diff * neuron.Func.Derivative(output);

				sampleErrorSum += diff * diff;
			}

			return sampleErrorSum / sampleOutput.Length;
		}

		/// <summary>
		///     Propagate error values from the output layer backwards.
		/// </summary>
		/// <param name="mlp">The network being trained.</param>
		/// <seealso cref="CalcError" />
		private static void PropagateError(MultilayerPerceptron mlp)
		{
			var layers = mlp.Layers;

			for (var l = layers.Length - 1; l > 0; l--)
			{
				var layer = layers[l - 1];
				var current = layer.Neurons;
				var next = layers[l].Neurons;

				for (var c = 0; c < current.Length; c++)
				{
					var errorSum = 0f;
					foreach (var n in next)
					{
						Debug.Assert(current.Length == n.InputCount);
						errorSum += n.Error * n.Weights[c];
					}

					current[c].Error = errorSum * current[c].Func.Derivative(layer.Output[c]);
				}
			}
		}

		/// <summary>
		///     Adjust the weights of every neuron.
		/// </summary>
		/// <param name="layers">The network layers.</param>
		/// <param name="sampleInput">The current sample's input values.</param>
		/// <param name="learningRate">The learning rate parameter.</param>
		/// <param name="momentum">The momentum parameter.</param>
		/// <seealso cref="AdjustWeights(Perceptron,float[],float,float)" />
		private static void AdjustWeights(Layer[] layers, float[] sampleInput, float learningRate, float momentum)
		{
			for (var i = 0; i < layers.Length; i++)
			{
				var neuronInput = i > 0 ? layers[i - 1].Output : sampleInput;

				foreach (var neuron in layers[i].Neurons)
					AdjustWeights(neuron, neuronInput, learningRate, momentum);
			}
		}

		/// <summary>
		///     Adjust the weights of the given neuron.
		/// </summary>
		/// <param name="neuron">The neuron to update.</param>
		/// <param name="neuronInput">
		///     The neuron's input values. This is the sample's input values for the input layer,
		///     and the previous layer's output values for every other layer.
		/// </param>
		/// <param name="learningRate">The learning rate parameter.</param>
		/// <param name="momentum">The momentum parameter.</param>
		private static void AdjustWeights(Perceptron neuron, float[] neuronInput, float learningRate, float momentum)
		{
			Debug.Assert((neuron.Weights.Length - 1) == neuronInput.Length);

			for (var i = 0; i < neuron.Weights.Length; i++)
			{
				var input = i < neuronInput.Length ? neuronInput[i] : 1f;
				var delta = learningRate * neuron.Error * input + momentum * neuron.Momentum[i];

				neuron.Weights[i] += delta;
				neuron.Momentum[i] = delta;
			}
		}
	}
}
