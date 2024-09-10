using System.Collections.Generic;

using RavingBots.MagicGestures.UI.Elements;

using UnityEngine;

namespace RavingBots.MagicGestures.Game
{
	/// <summary>
	///     Utility methods for physics based calculations.
	/// </summary>
	public static class PhysicsUtils
	{
		/// <summary>
		///     The index of the default layer.
		/// </summary>
		public const int LayerDefault = 0;
		/// <summary>
		///     The index of the layer that ignores raycasts.
		/// </summary>
		public const int LayerIgnoreRaycast = 2;
		/// <summary>
		///     The index of the UI layer.
		/// </summary>
		public const int LayerUI = 5;

		/// <summary>
		///     The layer mask for <see cref="LayerDefault" />.
		/// </summary>
		public const int MaskDefault = 1 << LayerDefault;
		/// <summary>
		///     The layer mask for <see cref="LayerUI" />.
		/// </summary>
		public const int MaskUI = 1 << LayerUI;

		/// <summary>
		///     Cast a ray to find an UI element.
		/// </summary>
		/// <returns><see langword="true" /> if element was found.</returns>
		public static bool RaycastUi(Ray ray, out UiElement hitElement, out Vector3 hitPosition)
		{
			hitElement = null;
			hitPosition = Vector3.zero;

			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, float.MaxValue, MaskUI))
			{
				hitElement = hit.transform.GetComponent<UiElement>();
				if (hitElement)
				{
					hitPosition = hit.point;
					return true;
				}
			}

			return false;
		}

		/// <summary>
		///     Create an explosion at the given position.
		/// </summary>
		/// <remarks>
		///     This adds a force impulse to all objects in the explosion radius,
		///     away from the explosion center, with strength based on how close to the
		///     explosion center that object is.
		/// </remarks>
		public static void Explode(Vector3 position, float radius, float power)
		{
			var rigidbodies = new Dictionary<int, List<Collider>>();
			var colliders = Physics.OverlapSphere(position, radius);

			foreach (var collider in colliders)
			{
				if (!collider.attachedRigidbody)
					continue;

				var id = collider.attachedRigidbody.GetInstanceID();

				List<Collider> list;
				if (rigidbodies.TryGetValue(id, out list))
					list.Add(collider);
				else
					rigidbodies.Add(id, new List<Collider> { collider });
			}

			foreach (var pair in rigidbodies)
			foreach (var collider in pair.Value)
			{
				var c = collider.ClosestPointOnBounds(position);
				var v = c - position;
				var d = v.magnitude;

				if (d < radius)
				{
					v /= d;
					var p = Mathf.Lerp(power, 0f, d / radius) / pair.Value.Count;
					collider.attachedRigidbody.AddForceAtPosition(p * v, c, ForceMode.Impulse);
				}
			}
		}
	}
}
