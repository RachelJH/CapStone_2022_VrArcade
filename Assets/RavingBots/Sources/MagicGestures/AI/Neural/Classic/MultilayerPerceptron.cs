// ReSharper disable FieldCanBeMadeReadOnly.Local

using System;
using System.Linq;
using RavingBots.MagicGestures.AI.Common;
using UnityEngine;

namespace RavingBots.MagicGestures.AI.Neural.Classic
{
	/// <summary>
	///     The implementation of a multilayer perceptron neural network.
	/// </summary>
	/// <remarks>
	///     MLP is a network formed by at least 2 layers (input and output, no hidden layers),
	///     each containing <see cref="Perceptron" /> neurons. By default networks are created
	///     with 1 hidden layer (3 in total).
	/// </remarks>
	/// <seealso cref="Layer.Create" />
	[Serializable]
	public class MultilayerPerceptron : NeuralNetwork
	{
		/// <summary>
		///     The neural network settings.
		/// </summary>
		[Serializable]
		public struct Settings : IEquatable<Settings>
		{
			/// <summary>
			///     The number of the network's inputs. Must be non-negative.
			/// </summary>
			/// <seealso cref="Layer.Create" />
			public int InputCount;

			/// <summary>
			///     The number of the network's outputs. Must be non-negative.
			/// </summary>
			/// <seealso cref="Layer.Create" />
			public int OutputCount;

			/// <summary>
			///     The specification of the network's hidden layers.
			/// </summary>
			/// <remarks>
			///     The number of elements in this array indicates the number of hidden layers.
			///     Every element is the number of neurons on that layer (must be positive).
			/// </remarks>
			public int[] HiddenLayers;

			/// <summary>
			///     The activation function used by the network's neurons.
			/// </summary>
			/// <seealso cref="Activation" />
			public Activation.FuncType FuncType;

			/// <summary>
			///     <see langword="true" /> if the settings are correct.
			/// </summary>
			public bool IsValid
			{
				get
				{
					return (InputCount >= 0) && (OutputCount >= 0) && (HiddenLayers != null) &&
							!HiddenLayers.Any(h => h <= 0);
				}
			}

			/// <summary>
			///     Create default settings based on a sample.
			/// </summary>
			/// <remarks>
			///     Every hidden layer will have <c>(InputCount + OutputCount) / 2</c> neurons.
			/// </remarks>
			/// <param name="sampleData">The sample to use.</param>
			/// <param name="hiddenLayers">The number of hidden layers to create. Must be non-negative.</param>
			/// <seealso cref="Layer.Create" />
			public static Settings GetDefault(SampleData sampleData, int hiddenLayers = 1)
			{
				Debug.Assert(sampleData.IsValid);
				Debug.Assert(hiddenLayers >= 0);

				var inputCount = sampleData.Input.Length;
				var outputCount = sampleData.Output.Length;

				var avg = Mathf.RoundToInt((inputCount + outputCount) / 2f);

				Settings result;
				result.InputCount = inputCount;
				result.OutputCount = outputCount;
				result.HiddenLayers = Enumerable.Repeat(avg, hiddenLayers).ToArray();
				result.FuncType = Activation.FuncType.Sigmoid;

				return result;
			}

			/// <summary>
			///     Compare two <see cref="Settings" /> objects for equality.
			/// </summary>
			public bool Equals(Settings other)
			{
				return
					(InputCount == other.InputCount) &&
					(OutputCount == other.OutputCount) &&
					HiddenLayers.SequenceEqual(other.HiddenLayers) &&
					(FuncType == other.FuncType);
			}

			/// <inheritdoc cref="Equals(Settings)" />
			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj))
					return false;

				return obj is Settings && Equals((Settings) obj);
			}

			/// <inheritdoc />
			public override int GetHashCode()
			{
				unchecked
				{
					// ReSharper disable NonReadonlyMemberInGetHashCode
					var hashCode = InputCount;
					hashCode = (hashCode * 397) ^ OutputCount;
					hashCode = (hashCode * 397) ^ HiddenLayers.GetHashCode();
					hashCode = (hashCode * 397) ^ (int) FuncType;
					return hashCode;
					// ReSharper restore NonReadonlyMemberInGetHashCode
				}
			}
		}

		/// <summary>
		///     The layers of this network.
		/// </summary>
		[SerializeField] private Layer[] _layers;

		/// <inheritdoc cref="_layers" />
		public Layer[] Layers
		{
			get { return _layers; }
		}

		/// <summary>
		///     The number of hidden layers in this network.
		/// </summary>
		public int HiddenCount
		{
			get { return _layers.Length - 1; }
		}

		/// <summary>
		///     The settings object used to create this network.
		/// </summary>
		[SerializeField] private Settings _initSettings;

		/// <inheritdoc cref="_initSettings" />
		public Settings InitSettings
		{
			get { return _initSettings; }
		}

		/// <summary>
		///     Outputs of this network.
		/// </summary>
		/// <remarks>
		///     Updated when <see cref="Process" /> is called.
		/// </remarks>
		/// <seealso cref="Process" />
		public override float[] Output
		{
			get { return _layers[_layers.Length - 1].Output; }
		}

		/// <summary>
		///     Construct an empty (no inputs, no layers) network.
		/// </summary>
		public MultilayerPerceptron()
			: base(0)
		{
			_layers = new Layer[0];
		}

		/// <summary>
		///     Construct a new network with the given settings.
		/// </summary>
		public MultilayerPerceptron(Settings settings)
			: base(settings.InputCount)
		{
			Debug.Assert(settings.IsValid);

			_initSettings = settings;

			_layers = Layer.Create(
				_initSettings.InputCount,
				_initSettings.OutputCount,
				_initSettings.HiddenLayers,
				_initSettings.FuncType);
		}

		/// <summary>
		///     Process a set of inputs and update <see cref="Output" />.
		/// </summary>
		/// <seealso cref="Layer.Process(Layer[],float[])" />
		public override void Process(float[] input)
		{
			Debug.Assert(input.Length == InputCount);

			Layer.Process(_layers, input);
		}

		/// <summary>
		///     Create a string representation of this network, for debugging.
		/// </summary>
		public override string ToString()
		{
			var neuronCount = Layers.Sum(l => l.Neurons.Length);
			var weightCount = Layers.Select(l => l.Neurons).Sum(neurons => neurons.Sum(n => n.Weights.Length));

			return string.Format(
				"MLP (Inputs: {0}, Outputs: {1}, Layers: {2}, Neurons: {3}, Weights: {4}, Activation: {5})",
				InputCount,
				InitSettings.OutputCount,
				Layers.Length,
				neuronCount,
				weightCount,
				InitSettings.FuncType);
		}
	}
}
