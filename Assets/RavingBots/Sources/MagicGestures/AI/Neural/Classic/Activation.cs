using System;
using UnityEngine;

namespace RavingBots.MagicGestures.AI.Neural.Classic
{
	/// <summary>
	///     The neuron activation functions.
	/// </summary>
	/// <remarks>
	///     Activation functions transform input of a neuron into an output signal.
	/// </remarks>
	public static class Activation
	{
		/// <summary>
		///     The type of the activation function.
		/// </summary>
		public enum FuncType
		{
			/// <summary>
			///     The logistic function (sigmoid).
			/// </summary>
			/// <seealso cref="FuncSigmoid" />
			Sigmoid,

			/// <summary>
			///     The hyperbolic tangent function (tanh).
			/// </summary>
			/// <seealso cref="FuncTanh" />
			Tanh
		}

		/// <summary>
		///     The base class for activation function implementations.
		/// </summary>
		public abstract class Func
		{
			/// <summary>
			///     The output range of the function.
			/// </summary>
			public readonly Vector2 Range;

			/// <summary>
			///     Construct a function with the given output range.
			/// </summary>
			/// <remarks>
			///     <note type="implement">
			///         Called by implementations to set the <see langword="readonly" />
			///         fields.
			///     </note>
			/// </remarks>
			protected Func(Vector2 range)
			{
				Range = range;
			}

			/// <summary>
			///     Check whether the value is in the output range of the function.
			/// </summary>
			public bool InRange(float y)
			{
				return (y >= Range.x) && (y <= Range.y);
			}

			/// <summary>
			///     Return the value of the function for the given input.
			/// </summary>
			public abstract float Calculate(float x);

			/// <summary>
			///     Return the derivative of the function for the given input.
			/// </summary>
			public abstract float Derivative(float x);
		}

		/// <summary>
		///     Implementation of the <see cref="FuncType.Sigmoid" /> function.
		/// </summary>
		/// <remarks>
		///     Implements the <c>f(x) = 1 / (1 + e^(-x * β))</c> function.
		///     The output is in range <c>[0, 1]</c>.
		/// </remarks>
		public class FuncSigmoid : Func
		{
			/// <summary>
			///     The scaling factor β of the input.
			/// </summary>
			public readonly float Beta;

			/// <summary>
			///     Construct a new instance with the given β value.
			/// </summary>
			public FuncSigmoid(float beta = 1f)
				: base(new Vector2(0f, 1f))
			{
				Beta = beta;
			}

			/// <inheritdoc />
			public override float Calculate(float x)
			{
				return 1f / (1f + (float) Math.Exp(-x * Beta));
			}

			/// <inheritdoc />
			public override float Derivative(float y)
			{
				return Beta * y * (1f - y);
			}
		}

		/// <summary>
		///     Implementation of the <see cref="FuncType.Tanh" /> function.
		/// </summary>
		/// <remarks>
		///     Implements the <c>f(x) = tanh(x)</c> function. The output is
		///     in range <c>[-1, 1]</c>.
		/// </remarks>
		public class FuncTanh : Func
		{
			/// <summary>
			///     Construct a new instance.
			/// </summary>
			public FuncTanh()
				: base(new Vector2(-1f, 1f))
			{
			}

			/// <inheritdoc />
			public override float Calculate(float x)
			{
				return (float) Math.Tanh(x);
			}

			/// <inheritdoc />
			public override float Derivative(float y)
			{
				return 1f - y * y;
			}
		}

		/// <summary>
		///     The shared <see cref="FuncSigmoid" /> instance with β = 1.
		/// </summary>
		public static readonly FuncSigmoid Sigmoid = new FuncSigmoid();

		/// <summary>
		///     The shared <see cref="FuncTanh" /> instance.
		/// </summary>
		public static readonly FuncTanh Tanh = new FuncTanh();

		/// <summary>
		///     Return a function instance based on an enumerator.
		/// </summary>
		/// <returns>
		///     A function instance, or <see langword="null" /> if
		///     <paramref name="func" /> is not a valid enumerator.
		/// </returns>
		public static Func GetFunc(FuncType func)
		{
			switch (func)
			{
				case FuncType.Sigmoid:
					return Sigmoid;
				case FuncType.Tanh:
					return Tanh;
				default:
					return null;
			}
		}
	}
}
