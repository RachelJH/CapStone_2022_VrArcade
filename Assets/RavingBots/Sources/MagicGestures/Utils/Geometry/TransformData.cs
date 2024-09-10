using System;

using UnityEngine;

namespace RavingBots.MagicGestures.Utils.Geometry
{
	/// <summary>
	///     A serializable form of <c>Transform</c>.
	/// </summary>
	[Serializable]
	public struct TransformData
	{
		/// <summary>
		///     The stored <c>Transform.parent</c>.
		/// </summary>
		public Transform Parent;

		/// <summary>
		///     The stored <c>Transform.localPosition</c>.
		/// </summary>
		public Vector3 LocalPosition;
		/// <summary>
		///     The stored <c>Transform.localRotation</c>.
		/// </summary>
		public Quaternion LocalRotation;
		/// <summary>
		///     The stored <c>Transform.localScale</c>.
		/// </summary>
		public Vector3 LocalScale;

		/// <summary>
		///     The stored <c>Transform.position</c>.
		/// </summary>
		public Vector3 WorldPosition;
		/// <summary>
		///     The stored <c>Transform.rotation</c>.
		/// </summary>
		public Quaternion WorldRotation;
		/// <summary>
		///     The stored <c>Transform.lossyScale</c>.
		/// </summary>
		public Vector3 WorldScale;

		/// <summary>
		///     Create a new instance based on the given transform.
		/// </summary>
		public TransformData(Transform t)
		{
			Parent = t.parent;

			LocalPosition = t.localPosition;
			LocalRotation = t.localRotation;
			LocalScale = t.localScale;

			WorldPosition = t.position;
			WorldRotation = t.rotation;
			WorldScale = t.lossyScale;
		}

		/// <summary>
		///     Create a new instance based on the given transform.
		/// </summary>
		public static implicit operator TransformData(Transform t)
		{
			return new TransformData(t);
		}
	}

	/// <summary>
	///     The extension methods for <c>Transform</c>.
	/// </summary>
	public static class TransformExt
	{
		/// <summary>
		///     Copy the stored data back to the transform.
		/// </summary>
		public static void Set(this Transform t, TransformData transformData)
		{
			t.parent = transformData.Parent;

			t.localPosition = transformData.LocalPosition;
			t.localRotation = transformData.LocalRotation;
			t.localScale = transformData.LocalScale;
		}
	}
}
