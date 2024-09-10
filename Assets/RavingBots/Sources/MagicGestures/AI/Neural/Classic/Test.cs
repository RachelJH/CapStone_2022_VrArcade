using System.Linq;
using RavingBots.MagicGestures.AI.Common;
using UnityEngine;

// ReSharper disable MissingXmlDoc
// ReSharper disable UnusedMember.Global

namespace RavingBots.MagicGestures.AI.Neural.Classic
{
	public static class Test
	{
		private const int DefaultRepeats = 10;

		public static SampleData[] GetXorTrainingSet()
		{
			return new[]
			{
				new SampleData(new[] {0f, 0f}, new[] {0f}),
				new SampleData(new[] {0f, 1f}, new[] {1f}),
				new SampleData(new[] {1f, 0f}, new[] {1f}),
				new SampleData(new[] {1f, 1f}, new[] {0f})
			};
		}

		public static Backpropagation.Status[] Train(
			SampleData[] samples,
			Backpropagation.Settings bpSettings,
			int repeats = DefaultRepeats)
		{
			Debug.Assert(SampleData.IsValidTrainingSet(samples));
			Debug.Assert(bpSettings.IsValid);
			Debug.Assert(repeats > 0);

			var mlpSettings = MultilayerPerceptron.Settings.GetDefault(samples[0]);

			Debug.Assert(mlpSettings.IsValid);

			var mlp = new MultilayerPerceptron(mlpSettings);
			var backpropagation = new Backpropagation(new System.Random(0));
			var result = new Backpropagation.Status[repeats];

			for (var i = 0; i < repeats; i++)
			{
				backpropagation.RunAsyncTraining(mlp, samples, bpSettings);
				backpropagation.CurrentThread.Join();
				result[i] = backpropagation.CurrentStatus;
			}

			return result;
		}

		public static void TrainXor()
		{
			Debug.LogFormat("Xor training test:");

			var result = Train(GetXorTrainingSet(), Backpropagation.Settings.Default);

			var avgError = result.Average(r => r.Error);
			var avgIter = result.Average(r => r.Iteration);
			var successful = result.Count(r => r.Successful);

			Debug.LogFormat(
				"\tavgError = {0}, avgIter = {1}, successful = {2}/{3}",
				avgError,
				avgIter,
				successful,
				DefaultRepeats);
		}
	}
}
