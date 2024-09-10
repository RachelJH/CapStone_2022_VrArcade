// ReSharper disable FieldCanBeMadeReadOnly.Local

using System;

using UnityEngine;

namespace RavingBots.MagicGestures.AI.Neural.Classic
{
	/// <summary>
	///     A single layer in a neural network.
	/// </summary>
	/// <remarks>
	///     Every layer has a set number of neurons (equal to the number of the layer's outputs),
	///     each of which has the same number of weights (equal to the number of they layer's inputs).
	/// </remarks>
	/// <seealso cref="Perceptron" />
	/// <seealso cref="MultilayerPerceptron" />
	[Serializable]
	public class Layer
	{
		/// <summary>
		///     The array of neurons on this layer.
		/// </summary>
		[SerializeField]
		private Perceptron[] _neurons;

		/// <inheritdoc cref="_neurons" />
		public Perceptron[] Neurons
		{
			get { return _neurons; }
		}

		/// <summary>
		///     The array of this layer's outputs.
		/// </summary>
		/// <remarks>
		///     Updated after <see cref="Process(float[])" /> is called
		///     with a set of inputs.
		/// </remarks>
		[SerializeField]
		private float[] _output;

		/// <inheritdoc cref="_output" />
		public float[] Output
		{
			get { return _output; }
		}

		/// <summary>
		///     Construct a new layer with the given number of neurons and inputs, and
		///     the given activation function.
		/// </summary>
		/// <seealso cref="Activation" />
		public Layer(int neuronCount, int inputCount, Activation.FuncType funcType)
		{
			_neurons = new Perceptron[neuronCount];
			_output = new float[neuronCount];

			for (var i = 0; i < _neurons.Length; i++)
				_neurons[i] = new Perceptron(inputCount, funcType);
		}

		/// <summary>
		///     Process the input values.
		/// </summary>
		/// <seealso cref="Perceptron.Process" />
		public void Process(float[] input)
		{
			for (var i = 0; i < _neurons.Length; i++)
				_output[i] = _neurons[i].Process(input);
		}

		/// <summary>
		///     Process the input values with a set of layers.
		/// </summary>
		/// <remarks>
		///     The inputs are chained: <paramref name="input" /> is used as the first layer's inputs,
		///     the second layer's inputs are the first layer's outputs, and so on.
		/// </remarks>
		/// <param name="layers">The set of layers to use.</param>
		/// <param name="input">The inputs to feed to the first layer.</param>
		public static void Process(Layer[] layers, float[] input)
		{
			for (var i = 0; i < layers.Length; i++)
				layers[i].Process(i > 0 ? layers[i - 1]._output : input);
		}

		/// <summary>
		///     Construct a set of layers usable as a network.
		/// </summary>
		/// <remarks>
		///     Because the inputs are chained (<see cref="Process(Layer[],float[])" />),
		///     the middle layers (called "hidden layers") must have a specific number of inputs
		///     and outputs. This method makes it easy to create an entire set of layers that
		///     can be used together as a network.
		/// </remarks>
		/// <param name="inputCount">The number of inputs.</param>
		/// <param name="outputCount">The number of outputs.</param>
		/// <param name="hiddenLayers">
		///     The specification of the hidden layers. See <see cref="MultilayerPerceptron.Settings.HiddenLayers" />.
		/// </param>
		/// <param name="funcType">The activation function to use for each neuron.</param>
		/// <seealso cref="Activation" />
		/// <seealso cref="MultilayerPerceptron" />
		public static Layer[] Create(int inputCount, int outputCount, int[] hiddenLayers, Activation.FuncType funcType)
		{
			var result = new Layer[hiddenLayers.Length + 1];

			for (var i = 0; i < hiddenLayers.Length; i++)
			{
				result[i] = new Layer(hiddenLayers[i], inputCount, funcType);
				inputCount = result[i]._neurons.Length;
			}

			result[hiddenLayers.Length] = new Layer(outputCount, inputCount, funcType);

			return result;
		}
	}
}
