using System.Linq;

namespace RavingBots.MagicGestures.AI.Common
{
	/// <summary>
	///     A single sample of training data.
	/// </summary>
	/// <remarks>
	///     Samples are created from preprocessed gestures in <see cref="GestureLearner.CreateSamples" />.
	/// </remarks>
	/// <seealso cref="GestureLearner" />
	/// <seealso cref="GesturePreprocessor" />
	public class SampleData
	{
		/// <summary>
		///     The array of the input values (in range <c>[0, 1]</c>).
		/// </summary>
		public readonly float[] Input;

		/// <summary>
		///     The array of the output values (in range <c>[0, 1]</c>).
		/// </summary>
		public readonly float[] Output;

		/// <summary>
		///     <see langword="false" /> if this sample is not valid.
		/// </summary>
		public bool IsValid
		{
			get { return (Input != null) && (Output != null) && (Input.Length > 0) && (Output.Length > 0); }
		}

		/// <summary>
		///     <see langword="true" /> if this sample is created with two hands.
		/// </summary>
		public bool IsTwoHanded;

		/// <summary>
		///     Create a new empty sample.
		/// </summary>
		public SampleData(int inputCount = 0, int outputCount = 0)
		{
			Input = new float[inputCount];
			Output = new float[outputCount];
		}

		/// <summary>
		///     Create a new sample from the existing data.
		/// </summary>
		/// <remarks>
		///     <note type="important">
		///         Arrays are not copied. Make sure you to make a copy yourself
		///         if you plan on modifying them afterwards.
		///     </note>
		/// </remarks>
		public SampleData(float[] input, float[] output)
		{
			Input = input;
			Output = output;
		}

		/// <summary>
		///     Verify whether the given set of samples can be used
		///     for training a network.
		/// </summary>
		/// <remarks>
		///     All samples must be valid and have the same number of inputs and outputs
		///     for the entire set to be valid.
		/// </remarks>
		/// <seealso cref="IsValid" />
		public static bool IsValidTrainingSet(SampleData[] samples)
		{
			if ((samples == null) || (samples.Length == 0))
				return false;

			//var first = samples[0];

			foreach (var s in samples)
			{
				if ((s == null) || !s.IsValid)
					return false;

				//if ((s.Input.Length != first.Input.Length) || (s.Output.Length != first.Output.Length))
				//	return false;
			}

			return true;
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return string.Format(
				"({0}) : ({1})",
				string.Join(", ", Input.Select(i => i.ToString("0.00")).ToArray()),
				string.Join(", ", Output.Select(i => i.ToString("0.00")).ToArray()));
		}

		/// <summary>
		///     Convert the entire set of samples to a string.
		/// </summary>
		public static string ToString(SampleData[] sampleData)
		{
			return string.Join("\n", sampleData.Select(s => s.ToString()).ToArray());
		}
	}
}
