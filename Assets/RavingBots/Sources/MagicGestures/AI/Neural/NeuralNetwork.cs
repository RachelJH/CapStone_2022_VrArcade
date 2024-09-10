// ReSharper disable FieldCanBeMadeReadOnly.Local

using RavingBots.MagicGestures.AI.Common;
using RavingBots.MagicGestures.AI.Neural.Classic;
using UnityEngine;

namespace RavingBots.MagicGestures.AI.Neural
{
	/// <summary>
	///     Base class for neural network implementations.
	/// </summary>
	/// <seealso cref="MultilayerPerceptron" />
	public abstract class NeuralNetwork
	{
		/// <summary>
		///     Number of inputs of this network.
		/// </summary>
		[SerializeField] private int _inputCount;

		/// <inheritdoc cref="_inputCount" />
		public int InputCount
		{
			get { return _inputCount; }
		}

		/// <summary>
		///     Outputs of this network.
		/// </summary>
		/// <remarks>
		///     Updated after <see cref="Process" /> is called with a set of inputs.
		/// </remarks>
		public abstract float[] Output { get; }

		/// <summary>
		///     Construct a new network with a given number of inputs.
		/// </summary>
		protected NeuralNetwork(int inputCount)
		{
			_inputCount = inputCount;
		}

		/// <summary>
		///     Process a set of inputs and update the outputs.
		/// </summary>
		/// <seealso cref="Output" />
		public abstract void Process(float[] input);

		/// <summary>
		///     Check whether given sample can be processed with this network.
		/// </summary>
		/// <remarks>
		///     For this to be possible, sample's inputs and outputs count must
		///     match those of the network.
		/// </remarks>
		public bool IsMatching(SampleData sample)
		{
			Debug.Assert((sample != null) && sample.IsValid);

			return (sample.Input.Length == InputCount) && (sample.Output.Length == Output.Length);
		}
	}
}
