// ReSharper disable FieldCanBeMadeReadOnly.Local

using RavingBots.MagicGestures.AI.Neural.Classic;
using UnityEngine;

namespace RavingBots.MagicGestures.AI.Neural
{
	/// <summary>
	///     The base class for neuron implementations.
	/// </summary>
	/// <seealso cref="Perceptron" />
	public abstract class Neuron
	{
		/// <summary>
		///     The inputs count of this neuron.
		/// </summary>
		[SerializeField] private int _inputCount;

		/// <inheritdoc cref="_inputCount" />
		public int InputCount
		{
			get { return _inputCount; }
		}

		/// <summary>
		///     Construct a new neuron with the given number of inputs.
		/// </summary>
		protected Neuron(int inputCount)
		{
			_inputCount = inputCount;
		}

		/// <summary>
		///     Process a set of inputs.
		/// </summary>
		/// <returns>The output signal of the neuron.</returns>
		public abstract float Process(float[] input);
	}
}
