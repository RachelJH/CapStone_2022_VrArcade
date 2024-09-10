// ReSharper disable FieldCanBeMadeReadOnly.Local

using System;
using RavingBots.MagicGestures.Utils;
using UnityEngine;

namespace RavingBots.MagicGestures.AI.Neural.Classic
{
	/// <summary>
	///     The implementation of a perceptron.
	/// </summary>
	/// <seealso cref="Backpropagation" />
	/// <seealso cref="MultilayerPerceptron" />
	/// <seealso cref="Activation" />
	[Serializable]
	public class Perceptron : Neuron
	{
		/// <summary>
		///     The type of the activation function of this neuron.
		/// </summary>
		/// <seealso cref="Activation" />
		[SerializeField] private Activation.FuncType _funcType;

		/// <inheritdoc cref="_funcType" />
		public Activation.FuncType FuncType
		{
			get { return _funcType; }
		}

		/// <summary>
		///     The instance of the activation function.
		/// </summary>
		/// <seealso cref="FuncType" />
		/// <seealso cref="Activation" />
		[NonSerialized] private Activation.Func _func;

		/// <inheritdoc cref="_func" />
		public Activation.Func Func
		{
			get { return _func ?? (_func = Activation.GetFunc(FuncType)); }
		}

		/// <summary>
		///     The stored weights.
		/// </summary>
		/// <remarks>
		///     On a layer with <c>N</c> inputs, every neuron stores <c>N + 1</c> weights.
		///     The extra weight is added to the sum.
		/// </remarks>
		/// <seealso cref="Process" />
		[SerializeField] internal float[] Weights;

		/// <summary>
		///     The array of changes.
		/// </summary>
		/// <remarks>
		///     This array is filled and used during the training, where every element
		///     is a last value added to the corresponding weight (<c>Weights[i] += delta; Momentum[i] = delta;</c>).
		/// </remarks>
		/// <seealso cref="Backpropagation.AdjustWeights(Perceptron,float[],float,float)" />
		/// <seealso cref="Weights" />
		[NonSerialized] internal float[] Momentum;

		/// <summary>
		///     The current error value.
		/// </summary>
		/// <remarks>
		///     This value is used during the training to propagate the error backwards.
		/// </remarks>
		[NonSerialized] internal float Error;

		/// <summary>
		///     Construct a new neuron with the given input count and the given activation function.
		/// </summary>
		public Perceptron(int inputCount, Activation.FuncType funcType)
			: base(inputCount)
		{
			Weights = new float[inputCount + 1];
			_funcType = funcType;
		}

		/// <summary>
		///     Process a set of inputs.
		/// </summary>
		/// <remarks>
		///     <para>
		///         The output signal is calculated by summing all of the inputs scaled by the corresponding
		///         weights stored in the neuron (which are set during the training), adding the final weight to that sum,
		///         and then passing that value into the activation function.
		///     </para>
		/// </remarks>
		/// <seealso cref="Func" />
		/// <seealso cref="Weights" />
		public override float Process(float[] input)
		{
			Debug.Assert(input.Length == InputCount);

			var sum = 0f;
			for (var i = 0; i < input.Length; i++)
			{
				Debug.Assert(Func.InRange(input[i]));
				sum += Weights[i] * input[i];
			}

			return Func.Calculate(sum + Weights[input.Length]);
		}

		/// <summary>
		///     Reset the neuron.
		/// </summary>
		/// <remarks>
		///     <para>
		///         This method sets every weight to a random one (from the given range),
		///         and clears <see cref="Momentum" /> and <see cref="Error" />, thereby forgetting
		///         the training.
		///     </para>
		/// </remarks>
		internal void Reset(System.Random random, Vector2 weightRange)
		{
			for (var i = 0; i < Weights.Length; i++)
				Weights[i] = (float) random.NextDouble(weightRange.x, weightRange.y);

			if ((Momentum == null) || (Momentum.Length != Weights.Length))
				Momentum = new float[Weights.Length];
			else
				Array.Clear(Momentum, 0, Momentum.Length);

			Error = 0f;
		}
	}
}
