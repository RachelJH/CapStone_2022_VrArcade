using UnityEngine;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace RavingBots.MagicGestures.Controller
{
	/// <summary>
	///     The component responsible for rendering the laser pointer.
	/// </summary>
	/// <seealso cref="LaserPointer" />
	[RequireComponent(typeof(LaserPointer), typeof(LineRenderer))]
	public class LaserRenderer : MonoBehaviour
	{
		/// <summary>
		///     The length of the laser.
		/// </summary>
		[SerializeField] protected float Length = 100f;

		/// <summary>
		///     The duration of the show/hide animation.
		/// </summary>
		[SerializeField] protected float ShowDuration = 0.2f;

		/// <summary>
		///     The transform scaled in the show/hide animation.
		/// </summary>
		[SerializeField] protected Transform Scaled;

		/// <summary>
		///     The laser pointer component drawn.
		/// </summary>
		private LaserPointer _laserPointer;

		/// <summary>
		///     The <c>LineRenderer</c> component used.
		/// </summary>
		private LineRenderer _lineRenderer;

		/// <summary>
		///     The initial scale of the pointer.
		/// </summary>
		private Vector3 _initScale;

		/// <summary>
		///     The initial width of the pointer.
		/// </summary>
		private float _initWidth;

		/// <summary>
		///     The progress of the show/hide animation.
		/// </summary>
		private float _showProgress;

		/// <summary>
		///     Remember the initial width and scale.
		/// </summary>
		protected void Awake()
		{
			_laserPointer = GetComponent<LaserPointer>();
			_lineRenderer = GetComponent<LineRenderer>();

			_initScale = Scaled.localScale;
			_initWidth = _lineRenderer.widthMultiplier;

			SetShowProgress(_showProgress);
		}

		/// <summary>
		///     Update the visuals if the animation is playing.
		/// </summary>
		protected void Update()
		{
			var showProgress =
				Mathf.Clamp01(_showProgress + (_laserPointer.enabled ? 1f : -1f) * Time.deltaTime / ShowDuration);

			if (_showProgress != showProgress)
			{
				_showProgress = showProgress;
				SetShowProgress(_showProgress);
			}

			if (_laserPointer.enabled)
			{
				var position =
					transform.InverseTransformPoint(_laserPointer.HoverPosition ?? _laserPointer.Ray.GetPoint(Length));
				_lineRenderer.SetPosition(1, position);
			}
		}

		/// <summary>
		///     Update the progress of the animation.
		/// </summary>
		private void SetShowProgress(float p)
		{
			Scaled.localScale = _initScale * p;
			_lineRenderer.widthMultiplier = _initWidth * p;
		}
	}
}
