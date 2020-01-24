using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace m039.Parallax
{

	public class ComplexParallaxLayer : BaseParallaxLayer
	{

		#region Inspector

		[Range(-180f, 180f)]
		public float angle;

		public float speed;

		#endregion

		protected override float Speed => speed;

		protected override Vector2 Direction => Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;

	}

}