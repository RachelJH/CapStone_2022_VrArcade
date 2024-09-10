using RavingBots.MagicGestures.AI.Common;
using RavingBots.MagicGestures.Controller;
using RavingBots.MagicGestures.Utils.Geometry;
using UnityEngine;

namespace RavingBots.MagicGestures.AI
{
	/// <summary>
	///     The preprocessor that converts captured gestures into a discrete form.
	/// </summary>
	/// <seealso cref="GestureLearner" />
	/// <seealso cref="GestureData" />
	public class GesturePreprocessor
	{
		/// <summary>
		///     The 3D grid of gesture points.
		/// </summary>
		/// <remarks>
		///     Value is <see langword="true" /> if the point belongs to the gesture.
		/// </remarks>
		public readonly bool[,,,] Matrix;

		/// <summary>
		///     Construct a new instance.
		/// </summary>
		/// <param name="matrixSize">The size of the discrete 3D grid.</param>
		/// <param name="handCount"></param>
		public GesturePreprocessor(Vector3Int matrixSize, int handCount)
		{
			Matrix = new bool[handCount, matrixSize.x, matrixSize.y, matrixSize.z];
		}

		/// <summary>
		///     Convert a gesture to an array of input values for training.
		/// </summary>
		/// <remarks>
		///     The resulting array is a flattened <see cref="Matrix" /> where
		///     every <see langword="true" /> value is converted to <c>1.0f</c>.
		/// </remarks>
		/// <returns>
		///     An array usable as <see cref="SampleData.Input" />.
		/// </returns>
		/// <seealso cref="GestureLearner.CreateSamples" />
		/// <seealso cref="SampleData" />
		public float[] GetInput(GestureData gesture)
		{
			Extract(gesture);

			var size = GetMatrixSize();
			var resultCount = gesture.HandCount;
			var result = new float[resultCount * (int) size.y * (int) size.z * (int) size.w];
			var i = 0;
			for (var j = 0; j < resultCount; j++)
			for (var x = 0; x < size.y; x++)
			for (var y = 0; y < size.z; y++)
			for (var z = 0; z < size.w; z++)
				result[i++] = Matrix[j, x, y, z] ? 1f : 0f;

			return result;
		}

		/// <summary>
		///     Convert captured gesture points into discrete ones.
		/// </summary>
		/// <remarks>
		///     <note type="important">
		///         The gesture must be valid (<see cref="GestureData.IsValid" /> and have at least one point.
		///     </note>
		///     <para>
		///         First, every gesture point is normalized so that all of their coordinates are in
		///         range <c>[0, 1]</c>, where points <c>(0, 0, 0)</c> and <c>(1, 1, 1)</c> are
		///         corners of the gesture's bounding box (see <see cref="Limits.GetNormalized" />).
		///     </para>
		///     <para>
		///         After that normalized points are mapped into equivalent points on
		///         a discrete 3D grid (see <see cref="GetDiscretized" />). Note that this is
		///         inherently a lossy operation — multiple captured points might be mapped into
		///         a single discrete point.
		///     </para>
		/// </remarks>
		/// <seealso cref="GestureData.Limits" />
		/// <seealso cref="GestureData.Radius" />
		/// <seealso cref="Matrix" />
		private void Extract(GestureData gesture)
		{
			Debug.Assert(gesture.IsValid);

			System.Array.Clear(Matrix, 0, Matrix.Length);

			var sss = GetMatrixSize();
			var size = new Vector3Int((int) sss.y, (int) sss.z, (int) sss.w);
			var limits = new Limits();
			limits.SetCenterRadius(gesture.Limits.Center, gesture.Radius);

			for (var i = 0; i < gesture.HandCount; i++)
			for (var j = 0; j < gesture.Points[i].Length() - 1; j++)
			{
				var p1 = gesture.Points[i][j];
				var p2 = gesture.Points[i][j + 1];

				var coord1 = GetDiscretized(limits.GetNormalized(p1), size);
				var coord2 = GetDiscretized(limits.GetNormalized(p2), size);

				var steps = Mathf.Max(
					Mathf.Abs(coord1.x - coord2.x),
					Mathf.Abs(coord1.y - coord2.y),
					Mathf.Abs(coord1.z - coord2.z));

				if (steps > 1)
				{
					var v = (p2 - p1) / steps;

					for (var s = 0; s <= steps; s++)
					{
						var p = p1 + s * v;
						var coord = GetDiscretized(limits.GetNormalized(p), size);
						Matrix[i, coord.x, coord.y, coord.z] = true;
					}
				}
				else
				{
					Matrix[i, coord1.x, coord1.y, coord1.z] = true;
					Matrix[i, coord2.x, coord2.y, coord2.z] = true;
				}
			}
		}

		/// <summary>
		///     Convert a single normalized point into a discrete one.
		/// </summary>
		/// <param name="normalized">The point to map.</param>
		/// <param name="size">The size of the discrete grid.</param>
		/// <returns>The mapped point.</returns>
		private static Vector3Int GetDiscretized(Vector3 normalized, Vector3Int size)
		{
			Debug.Assert(IsNormalized(normalized));
			Debug.Assert(IsValidSize(size));

			return new Vector3Int
			{
				x = Mathf.Clamp(Mathf.FloorToInt(normalized.x * size.x), 0, size.x - 1),
				y = Mathf.Clamp(Mathf.FloorToInt(normalized.y * size.y), 0, size.y - 1),
				z = Mathf.Clamp(Mathf.FloorToInt(normalized.z * size.z), 0, size.z - 1)
			};
		}

		/// <summary>
		///     Return the size of <see cref="Matrix" /> as a vector.
		/// </summary>
		private Vector4 GetMatrixSize()
		{
			return new Vector4(Matrix.GetLength(0), Matrix.GetLength(1), Matrix.GetLength(2), Matrix.GetLength(3));
		}

		/// <summary>
		///     Check whether <paramref name="point" /> is normalized.
		/// </summary>
		private static bool IsNormalized(Vector3 point)
		{
			var min = -2f * Mathf.Epsilon;
			var max = 1f + 2f * Mathf.Epsilon;

			return
				(point.x >= min) && (point.x <= max) &&
				(point.y >= min) && (point.y <= max) &&
				(point.z >= min) && (point.z <= max);
		}

		/// <summary>
		///     Check whether <paramref name="size" /> is valid (all components are positive).
		/// </summary>
		private static bool IsValidSize(Vector3Int size)
		{
			return (size.x > 0) && (size.y > 0) && (size.z > 0);
		}

		/// <summary>
		///     Draw the discrete grid as a gizmo.
		/// </summary>
		/// <remarks>
		///     This method visualises what <see cref="Extract" /> does for debugging purposes.
		///     The resulting grid is not saved but rather drawn as gizmo cubes on the scene view
		///     (where filled cube is where a value in <see cref="Matrix" /> would be <see langword="true" />).
		/// </remarks>
		/// <param name="gesture">The gesture to discretize.</param>
		/// <param name="position">The desired position of the gizmo grid.</param>
		/// <param name="size">The desired size of the gizmo grid.</param>
		public void DrawGizmos(GestureData gesture, Vector3 position, float size)
		{
			Extract(gesture);

			var matrixSize = GetMatrixSize();
			var s = size / Mathf.Max(matrixSize.x, matrixSize.y, matrixSize.z);
			var c = s * Vector3.one;

			var colors = new[] {Color.magenta, Color.cyan, Color.yellow};

			for (var j = 0; j < matrixSize.x; j++)
			{
				colors[j].a = 0.5f;
				Gizmos.color = colors[j];

				for (var x = 0; x < matrixSize.y; x++)
				for (var y = 0; y < matrixSize.z; y++)
				for (var z = 0; z < matrixSize.w; z++)
				{
					var p = position + new Vector3(
								s * (x - (matrixSize.y - 1) / 2f),
								s * (y - (matrixSize.z - 1) / 2f),
								s * (z - (matrixSize.w - 1) / 2f));

					p += new Vector3(size * j, 0, 0);

					if (Matrix[j, x, y, z])
						Gizmos.DrawCube(p, c);
					else
						Gizmos.DrawWireCube(p, c);
				}
			}

			Gizmos.color = Color.white;
			for (var i = 0; i < gesture.HandCount; i++)
			for (var j = 0; j < gesture.Points[i].Length() - 1; j++)
			{
				var p1 = position + 0.5f * size * gesture.Points[i][j] + new Vector3(size * i, 0, 0);
				var p2 = position + 0.5f * size * gesture.Points[i][j + 1] + new Vector3(size * i, 0, 0);

				Gizmos.DrawLine(p1, p2);
			}
		}
	}
}
