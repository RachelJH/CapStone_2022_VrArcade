// ReSharper disable FieldCanBeMadeReadOnly.Local

using UnityEngine;

namespace RavingBots.MagicGestures.Utils.Geometry
{
	/// <summary>
	///     The replacement for the Unity's <c>Bounds</c> struct.
	/// </summary>
	/// <remarks>
	///     The <c>Bounds</c> provided by Unity misbehaves when its <c>SetMinMax</c> is used.
	///     This struct provides similar API (at least the subset we need) while correcting the behavior.
	/// </remarks>
	[System.Serializable]
	public struct Limits
	{
		[SerializeField]
		private Vector3 _min;
		[SerializeField]
		private Vector3 _max;
		[SerializeField]
		private Vector3 _center;
		[SerializeField]
		private Vector3 _size;

		public Vector3 Min
		{
			get { return _min; }
		}

		public Vector3 Max
		{
			get { return _max; }
		}

		public Vector3 Center
		{
			get { return _center; }
		}

		public Vector3 Size
		{
			get { return _size; }
		}

		public void SetMinMax(Vector3 min, Vector3 max)
		{
			_min = min;
			_max = max;
			_center = (min + max) / 2;
			_size = max - min;
		}

		public void SetCenterRadius(Vector3 center, float radius)
		{
			var r = radius * Vector3.one;

			_center = center;
			_size = 2f * r;
			_min = _center - r;
			_max = _center + r;
		}

		public bool Contains(Vector3 p)
		{
			return
				(p.x >= Min.x) && (p.y >= Min.y) && (p.z >= Min.z) &&
				(p.x <= Max.x) && (p.y <= Max.y) && (p.z <= Max.z);
		}

		public Vector3 GetNormalized(Vector3 point)
		{
			Debug.Assert(Contains(point));

			point -= _min;
			point.x /= _size.x;
			point.y /= _size.y;
			point.z /= _size.z;

			return point;
		}
	}
}
