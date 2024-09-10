
using System.Collections;
using System.Collections.Generic;
using RavingBots.MagicGestures.UI.Elements;
using UnityEngine;

namespace RavingBots.MagicGestures.UI.Views
{
	public static class ViewUtils
	{
		public static IEnumerable<T> InstantiateInCircle<T>(T prefab, Transform parent, int count, float radius, Vector3 center, Vector3 forward, Vector3 up) where T : UiElement
		{
			var rot = Quaternion.AngleAxis(360f / count, forward);
			var vector = up * radius;

			for (var i = 0; i < count; i++)
			{
				var o = Object.Instantiate(prefab, parent);
				o.transform.localPosition = center + vector;
				vector = rot * vector;

				yield return o;
			}
		}

		public static IEnumerable<T> InstantiateCurvedGrid<T>(T prefab, Transform parent, int width, int height, float radius, float spacing, Vector3 center) where T : UiElement
		{
			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					var o = Object.Instantiate(prefab, parent);

					var r = radius * o.transform.forward +
												new Vector3(
													spacing * (x - (width - 1) / 2f),
													spacing * (y - (height - 1) / 2f));

					o.transform.localPosition = center + radius * r.normalized;
					o.transform.localRotation = Quaternion.FromToRotation(o.transform.forward, r) * o.transform.localRotation;

					yield return o;
				}
			}
		}

		/*public static IEnumerable<T> InstantiateOnSphere<T>(T prefab, Transform parent, int count, float totalPoints, float radius, Vector3 center) where T : UiElement
		{
			var inc = Mathf.PI * (3f - Mathf.Sqrt(5));
			var off = 2f / totalPoints;

			for (var i = 0; i < count; i++)
			{
				var y = i * off - 1f + off / 2f;
				var r = Mathf.Sqrt(1f - y * y);
				var phi = i * inc;

				var vector = new Vector3(Mathf.Cos(phi) * r, y, Mathf.Sin(phi) * r);

				var o = Object.Instantiate(prefab, parent);
				o.transform.localPosition = center + radius * vector;


				yield return o;
			}
		}*/
	}
}
