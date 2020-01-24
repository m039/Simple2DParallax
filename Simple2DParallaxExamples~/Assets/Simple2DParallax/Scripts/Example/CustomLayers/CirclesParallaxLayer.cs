using System.Collections;
using System.Collections.Generic;
using m039.Common;
using UnityEngine;

namespace m039.Parallax
{

	[ExecuteInEditMode]
	public class CirclesParallaxLayer : MonoBehaviour
	{

		#region Inspector

		public int numberOfCircles = 5;

		public float circleBorderSize = 0.1f;

		public float speed = 1;

		public Color circleBorderColorMax = Color.white;

		public Color circleBorderColorMin = Color.white;

		[CurveRange]
		public AnimationCurve colorCurve = AnimationCurve.Linear(0, 0, 1, 1);

		#endregion

		bool _validated = false;

		private void OnEnable()
		{
			GenerateCircles();
		}

		private void OnValidate()
		{
			_validated = true;
		}

		private void LateUpdate()
		{
			if (_validated)
			{
				GenerateCircles();
				_validated = false;
			}
		}

		void GenerateCircles()
		{
			Common.CommonUtils.DestroyAllChildren(transform);

			numberOfCircles.Times((i) =>
			{
				var position = i + 1;

				var obj = new GameObject($"Circle {position}".Decorate());
				obj.transform.SetParent(transform, worldPositionStays: false);
				obj.transform.localScale = Vector3.one * (float)position / numberOfCircles;

				var parallaxLayer = obj.AddComponent<CircleParallaxLayer>();

				parallaxLayer.depthOrder = i;
				parallaxLayer.circleBorderSize = circleBorderSize;
				parallaxLayer.speed = (float)position / numberOfCircles * speed;
				parallaxLayer.circleColor = Color.Lerp(circleBorderColorMin, circleBorderColorMax, colorCurve.Evaluate(i / (float)numberOfCircles));
			});
		}

	}

}