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

	}

}