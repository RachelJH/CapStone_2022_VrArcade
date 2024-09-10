using System;
using System.Linq;
using RavingBots.MagicGestures.Utils.Geometry;
using UnityEngine;

// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace RavingBots.MagicGestures.Controller
{
	/// <summary>
	///     A wrapper class needed for jagged array to be serialized properly by Unity.
	/// </summary>
	[Serializable]
	public class ArrayWrapper
	{
		public Vector3[] Array;

		public int Length()
		{
			return Array.Length;
		}

		public Vector3 this[int key]
		{
			get { return Array[key]; }
			set { Array[key] = value; }
		}
	}

	/// <summary>
	///     A single captured gesture.
	/// </summary>
	[Serializable]
	public class GestureData
	{
		/// <summary>
		///     The points of the gesture.
		/// </summary>
		public ArrayWrapper[] Points;

		/// <summary>
		///     The bounding box of the gesture.
		/// </summary>
		/// <seealso cref="Recalculate" />
		public Limits Limits;

		/// <summary>
		///     The total length of the gesture
		///     (the sum of the distances between points).
		/// </summary>
		/// <seealso cref="GetResampled" />
		public float[] Length;

		/// <summary>
		///     The radius of a sphere containing all of the gesture points.
		/// </summary>
		public float Radius;

		/// <summary>
		///     Count of hands used to perform gesture.
		/// </summary>
		public int HandCount;

		/// <summary>
		///     <see langword="true" /> if this gesture is valid (has at least one point).
		/// </summary>
		public bool IsValid
		{
			get { return (Points != null) && (Points.Length > 0); }
		}

		/// <summary>
		///     Create an empty gesture.
		/// </summary>
		public GestureData(int size, int handCount)
		{
			HandCount = handCount;
			Points = new ArrayWrapper[HandCount];
			for (var i = 0; i < Points.Length; i++) Points[i] = new ArrayWrapper {Array = new Vector3[size]};
			Length = new float[handCount];
		}

		/// <summary>
		///     Create a new gesture from existing points.
		/// </summary>
		public GestureData(Vector3[][] points)
		{
			Points = new ArrayWrapper[points.Length];
			for (var i = 0; i < Points.Length; i++)
				Points[i] = new ArrayWrapper {Array = (Vector3[]) points[i].Clone()};
			HandCount = points.Length;
			Length = new float[HandCount];

			Recalculate();
		}

		/// <summary>
		///     Create a new gesture from existing points.
		/// </summary>
		public GestureData(ArrayWrapper[] points)
		{
			Points = points;
			HandCount = points.Length;
			Length = new float[HandCount];
			Recalculate();
		}

		/// <summary>
		///     Create an identical copy of an existing gesture.
		/// </summary>
		public GestureData(GestureData other)
		{
			Points = (ArrayWrapper[]) other.Points.Clone();
			HandCount = other.HandCount;
			Limits = other.Limits;

			Length = other.Length.Length > 0 ? other.Length : new float[HandCount];

			Radius = other.Radius;
		}

		/// <summary>
		///     Normalize and then rotate all gesture points.
		/// </summary>
		/// <remarks>
		///     <note type="important">
		///         The gesture must be valid.
		///     </note>
		/// </remarks>
		public void Normalize(Quaternion rotation)
		{
			Debug.Assert(IsValid);

			if (!IsValid)
				return;
			for (var i = 0; i < Points.Length; i++)
				for (var j = 0; j < Points[i].Length(); j++)
				{
					Points[i][j] = (Points[i][j] - Limits.Center) / Radius;
					Points[i][j] = rotation * Points[i][j];
				}


			Recalculate();
		}

		/// <summary>
		///     Mirror the gesture on the X axis.
		/// </summary>
		/// <remarks>
		///     This mutates the gesture. Clone the gesture if you want to preserve the original.
		/// </remarks>
		public void MirrorX()
		{
			Debug.Assert(IsValid);

			if (!IsValid)
				return;

			for (var i = 0; i < Points.Length; i++)
				for (var j = 0; j < Points[i].Length(); j++)
					Points[i].Array[j].x = -Points[i][j].x;

			Recalculate();
		}

		/// <summary>
		///     Mirror the gesture on the Y axis.
		/// </summary>
		/// <remarks>
		///     This mutates the gesture. Clone the gesture if you want to preserve the original.
		/// </remarks>
		public void MirrorY()
		{
			Debug.Assert(IsValid);

			if (!IsValid)
				return;

			for (var i = 0; i < Points.Length; i++)
				for (var j = 0; j < Points[i].Length(); j++)
					Points[i].Array[j].y = -Points[i][j].y;

			Recalculate();
		}

		/// <summary>
		///     Mirror the gesture on the Z axis.
		/// </summary>
		/// <remarks>
		///     This mutates the gesture. Clone the gesture if you want to preserve the original.
		/// </remarks>
		public void MirrorZ()
		{
			Debug.Assert(IsValid);

			if (!IsValid)
				return;

			for (var i = 0; i < Points.Length; i++)
				for (var j = 0; j < Points[i].Length(); j++)
					Points[i].Array[j].z = -Points[i][j].z;

			Recalculate();
		}

		/// <summary>
		///     Recalculate the bounding box, the bounding sphere, and the length of the gesture.
		/// </summary>
		/// <remarks>
		///     <note type="important">
		///         The gesture must be valid. Additionally, the distance between
		///         any two points must be greater than zero.
		///     </note>
		/// </remarks>
		public void Recalculate()
		{
			Debug.Assert(IsValid);

			if (!IsValid)
				return;

			Vector3 min, max;

			min = max = Points[0][0];
			Length = new float[HandCount];


			for (var i = 0; i < Points.Length; i++)
			{
				for (var j = 1; j < Points[i].Length(); j++)
				{
					var a = Points[i][j - 1];
					var b = Points[i][j];

					min.x = Mathf.Min(min.x, b.x);
					min.y = Mathf.Min(min.y, b.y);
					min.z = Mathf.Min(min.z, b.z);

					max.x = Mathf.Max(max.x, b.x);
					max.y = Mathf.Max(max.y, b.y);
					max.z = Mathf.Max(max.z, b.z);

					var d = (b - a).magnitude;
					Debug.Assert(d > 0);
					Length[i] += d;
				}

				Limits.SetMinMax(min, max);
				Radius = 0f;
			}

			foreach (var points in Points)
				for (var i = 0; i < points.Length(); i++)
				{
					var r = (points[i] - Limits.Center).sqrMagnitude;
					if (Radius < r)
						Radius = r;
				}

			Radius = Mathf.Sqrt(Radius);
		}

		/// <summary>
		///     Interpolate a point of the gesture.
		/// </summary>
		/// <remarks>
		///     <note type="important">
		///         The gesture must be valid.
		///     </note>
		/// </remarks>
		public Vector3? Lerp(float t, int i)
		{
			Debug.Assert(IsValid);

			if (!IsValid)
				return null;

			t = Mathf.Clamp01(t);
			var dist = t * Length[i];
			var totalDist = 0f;
			for (var j = 1; j < Points[i].Length(); j++)
			{
				var a = Points[i][j - 1];
				var b = Points[i][j];
				var v = b - a;

				var len = v.magnitude;
				totalDist += len;

				if (totalDist >= dist)
					return b - v * (totalDist - dist) / len;
			}

			return Points[i][Points[i].Length() - 1];
		}

		/// <summary>
		///     Resize the gesture with resampling.
		/// </summary>
		/// <remarks>
		///     <note type="important">
		///         The gesture must be valid.
		///     </note>
		/// </remarks>
		/// <param name="size">The new size (as a number of points, must be greater than 1).</param>
		/// <returns>The resized gesture.</returns>
		public GestureData GetResampled(int size)
		{
			var good = IsValid && (size > 1);

			Debug.Assert(good);

			if (!good)
				return null;

			if (Points.Length == size * HandCount)
				return new GestureData(this);

			var points = new ArrayWrapper[HandCount];

			for (var i = 0; i < Points.Length; i++)
			{
				points[i] = new ArrayWrapper();
				points[i].Array = new Vector3[size];
				for (var j = 0; j < points[i].Length(); j++)
					points[i][j] = Lerp(j / (size - 1f), i).Value;
			}

			return new GestureData(points);
		}

		/// <summary>
		///     Average a set of captured gestures.
		/// </summary>
		/// <remarks>
		///     <note type="important">
		///         All of the gestures in <paramref name="gestures" /> must be non-null and valid.
		///     </note>
		/// </remarks>
		/// <param name="gestures">The input gestures. Must not be null or empty.</param>
		/// <param name="resampleSize">The final gesture size (must be greater than 1).</param>
		/// <seealso cref="GetResampled" />
		public static GestureData GetAveraged(GestureData[] gestures, int resampleSize)
		{
			var good = (resampleSize > 1) && (gestures != null) && (gestures.Length > 0) &&
						!gestures.Any(g => (g == null) || !g.IsValid || g.HandCount != gestures[0].HandCount);

			Debug.Assert(good);


			if (!good)
				return null;

			var resampled = gestures.Select(g => g.GetResampled(resampleSize)).ToArray();
			var result = new GestureData(resampleSize, gestures[0].HandCount);
			for (var j = 0; j < result.HandCount; j++)
			for (var i = 0; i < resampleSize; i++)
			{
				var p = Vector3.zero;
				foreach (var g in resampled)
					p += g.Points[j][i];

				result.Points[j][i] = p / resampled.Length;
			}

			result.Recalculate();

			return result;
		}

		/// <summary>
		///     Draw the gesture with gizmo lines for debugging.
		/// </summary>
		public void DrawGizmos()
		{
			if (Points != null)
			{
				Gizmos.color = Color.magenta;
				for (var i = 0; i < Points.Length; i++)
				for (var j = 1; j < Points.Length; j++)
					Gizmos.DrawLine(Points[i][j - 1], Points[i][j]);
			}
		}
	}
}
