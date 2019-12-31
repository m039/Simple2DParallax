using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace m039.Parallax
{

	public class ComplexParallaxLayer : BaseParallaxLayer
	{

		#region Inspector

		[Header("Complex Parallax Settings")]
		[Range(0f, 360f)]
		public float angle;

		public float speed;

		#endregion


	}

}