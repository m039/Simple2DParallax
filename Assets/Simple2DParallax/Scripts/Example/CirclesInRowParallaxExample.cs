using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace m039.Parallax
{

	public class CirclesInRowParallaxExample : MonoBehaviour
	{
		#region Inspector

		public Transform followTarget;

		public float targetSpeed = 1f;

		public float checkpointDistance = 0;

		#endregion

		float _angleDelta;

		float _angle;

		private void Awake()
		{
			ParallaxManager.Instance.Follow(followTarget);

			_angleDelta = 2 * Mathf.PI * checkpointDistance / targetSpeed;
		}

		private void LateUpdate()
		{
			if (followTarget == null)
				return;

			var deltaAngle = 360 / _angleDelta * Time.deltaTime;

			_angle += deltaAngle;

			followTarget.Rotate(new Vector3(0, 0, deltaAngle));

			if (_angle > 360)
			{
				Debug.Break();
			}
		}
	}

}