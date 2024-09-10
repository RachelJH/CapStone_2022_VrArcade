using System.Collections.Generic;
using System.Linq;
using RavingBots.MagicGestures.Utils.ObjectPooling;
using UnityEngine;

namespace RavingBots.MagicGestures.Controller
{
	/// <summary>
	///     The component that captures gesture points.
	/// </summary>
	public class GestureTracker : MonoBehaviour
	{
		/// <summary>
		///     The size of the initial <see cref="GestureTrail" /> pool.
		/// </summary>
		private const int Precache = 5;

		/// <summary>
		///     The prefab for the visual trail of the gesture.
		/// </summary>
		[SerializeField] protected GestureTrail TrailPrefab;

		/// <summary>
		///     The pool of <see cref="GestureTrail" /> objects.
		/// </summary>
		/// <seealso cref="GameObjectPool{T}" />
		private GameObjectPool<GestureTrail> _trailPool;

		/// <summary>
		///     The minimum number of points before the gesture is considered complete.
		/// </summary>
		public int MinPoints = 5;

		/// <summary>
		///     The maximum number of captured points.
		/// </summary>
		public int MaxPoints = 50;

		/// <summary>
		///     The minimum distance before a new point is captured.
		/// </summary>
		public float MinPointsDistance = 0.05f;

		/// <summary>
		///     The transform that is currently being tracked.
		/// </summary>
		/// <remarks>
		///     The transform of the 3D pointer that the motion controller is manipulating.
		/// </remarks>
		/// <seealso cref="MagicWand" />
		private Transform[] _tracked;

		/// <inheritdoc cref="_tracked" />
		public Transform[] Tracked
		{
			get { return _tracked; }
			set
			{
				if ((value == null && _tracked == null) || !enabled)
					return;


				if (_tracked != null)
				{
					var same = true;

					for (var i = 0; i < _tracked.Length; i++)
						if (_tracked[i] != value[i])
						{
							same = false;
							break;
						}

					if (same) return;
				}


				_tracked = value;

				SetTracked(_tracked);

			}
		}

		/// <summary>
		///     The visual trail of the gesture.
		/// </summary>
		/// <seealso cref="GestureTrail" />
		public GestureTrail[] Trail { get; private set; }

		public int HandCount
		{
			get { return _handCount; }
			set
			{
				_handCount = value;

				_points = new List<Vector3>[_handCount];
				_lastPosition = new Vector3[_handCount];
				Trail = new GestureTrail[_handCount];
			}
		}

		/// <summary>
		///     Already captured points.
		/// </summary>
		private List<Vector3>[] _points;

		/// <summary>
		///     The previous position of the <see cref="Tracked" />.
		/// </summary>
		private Vector3[] _lastPosition;

		private int _handCount;

		/// <summary>
		///     Setup the component.
		/// </summary>
		/// <seealso cref="GameObjectPool{T}" />
		protected void Awake()
		{
			_trailPool = new GameObjectPool<GestureTrail>(TrailPrefab, Precache);
			_points = new List<Vector3>[HandCount];
			_lastPosition = new Vector3[HandCount];
			Trail = new GestureTrail[HandCount];
		}

		/// <summary>
		///     Return a complete gesture, if enough points are captured.
		/// </summary>
		/// <returns>
		///     <see langword="null" /> if not enough points have been captured yet,
		///     a <see cref="GestureData" /> instance otherwise.
		/// </returns>
		public GestureData GetGesture()
		{
			if (_points.Any(p => p != null && p.Count >= MinPoints))
			{
				var nonZeroPoints = _points.Select(a => a.ToArray()).Where(p => p.Length > 1 && p[0] != Vector3.zero)
					.ToArray();

				return new GestureData(nonZeroPoints);
			}

			return null;
		}

		/// <summary>
		///     Change the tracked transform.
		/// </summary>
		/// <param name="tracked">The new transform to track. May be <see langword="null" />.</param>
		private void SetTracked(Transform[] tracked)
		{
			for (var i = 0; i < Trail.Length; i++)
				if (Trail[i])
				{
					Trail[i].SetLine(_points[i]);
					Trail[i].Release();
					Trail[i] = null;
				}

			if (tracked != null)
				for (var i = 0; i < tracked.Length; i++)
				{
					if (_points.Length <= i)
					{
						return;
					}

					if (_points[i] == null) _points[i] = new List<Vector3>();
					_points[i].Clear();

					if (tracked[i])
					{
						_points[i].Add(tracked[i].position);
						_lastPosition[i] = tracked[i].position;

						Trail[i] = _trailPool.TakeInstance();
						Trail[i].SetLine(_points[i]);
						Trail[i].SetTip(_lastPosition[i]);
						Trail[i].gameObject.SetActive(true);
					}
				}
		}

		/// <summary>
		///     Track the pointer and capture gesture points.
		/// </summary>
		protected void FixedUpdate()
		{
			if (_tracked != null)
				for (var i = 0; i < _tracked.Length; i++)
				{
					if (!_tracked[i])
						continue;

					var point = _tracked[i].position;

					if ((_lastPosition[i] - point).sqrMagnitude >= MinPointsDistance * MinPointsDistance)
					{
						_points[i].Add(point);
						if (_points[i].Count > MaxPoints)
							_points[i].RemoveAt(0);

						Trail[i].SetLine(_points[i]);

						_lastPosition[i] = point;
					}

					Trail[i].SetTip(point);
				}
		}

		/// <summary>
		///     Draw the position of the tracked object as a gizmo.
		/// </summary>
		protected void OnDrawGizmos()
		{
			for (var i = 0; i < HandCount; i++)
				if (_tracked[i])
				{
					Gizmos.color = Color.magenta;
					Gizmos.DrawSphere(_tracked[i].position, 0.025f);
				}
		}
	}
}
