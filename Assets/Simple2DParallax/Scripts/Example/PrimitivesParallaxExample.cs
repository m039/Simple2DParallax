using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace m039.Parallax
{

	public class PrimitivesParallaxExample : MonoBehaviour
	{

		#region Inspector

		public Transform followTarget;

		public float targetSpeed = 1f;

		#endregion

		private void Start()
		{
			ParallaxManager.Instance.Follow(followTarget);
		}

		private void LateUpdate()
		{
			if (followTarget == null)
				return;

			followTarget.position += Vector3.right * targetSpeed * Time.deltaTime;
		}

	}

}