using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace m039.Parallax
{

	public abstract class BaseParallaxLayer : MonoBehaviour
	{

		#region Inspector

		[Tooltip("An order of the ParllaxLayer in Z direction.")]
		public int depthOrder = 0;

		#endregion

		Vector2 _lastPosition;

		Vector2 _parallaxOffset;

		protected virtual float Speed
		{
			get
			{
				return 0;
			}
		}

		protected virtual Vector2 Direction
		{

			get
			{
				return Vector2.right;
			}

		}

		protected Vector2 ParallaxOffset
		{
			get
			{
				return _parallaxOffset;
			}

			set
			{
				_parallaxOffset = value;
			}
		}

		protected Vector2 MovingDirection => Direction.normalized * Mathf.Sign(Speed);

		protected virtual void OnEnable()
		{
			UpdateDepth();
		}

		protected virtual void OnDisable()
		{

		}

		protected virtual void Start()
		{
			if (ParallaxManager.Instance == null)
				return;

			_parallaxOffset = Vector2.zero;
			_lastPosition = Vector2.zero;

			UpdateDepth();
			FollowPosition();
		}

		protected virtual void LateUpdate()
		{
			if (Application.isPlaying)
			{
				FollowPosition();
			}
		}

		void UpdateDepth()
		{
			if (ParallaxManager.Instance == null || !ParallaxManager.Instance.UseDepth)
				return;

			var position = transform.position;

			position.z = ParallaxManager.Instance.GetDepth(depthOrder);

			transform.position = position;
		}

		void FollowPosition()
		{
			if (ParallaxManager.Instance == null)
				return;

			var followPosition = ParallaxManager.Instance.GetFollowPosition();

			// Update position of the object.

			var delta = (followPosition - _lastPosition).magnitude;

			_parallaxOffset += delta * ParallaxManager.Instance.ReferenceSpeed * Mathf.Abs(Speed) * MovingDirection;
			_lastPosition = followPosition;

			// Set the position.

			Vector3 position = followPosition + _parallaxOffset;

			position.z = transform.position.z;

			transform.position = position;

			OnParallaxOffsetChanged();
		}

		protected virtual void OnParallaxOffsetChanged()
		{

		}

	}

}