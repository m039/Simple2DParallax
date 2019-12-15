using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LH
{

	public class ParallaxMovingObject : ParallaxBaseBackground
	{

		#region Inspector

		[Tooltip("Скорость с которой движется объект, на котурую влияет еще общая скорость параллакса.")]
		public float horizontalSpeed = 0f;

		#endregion

		float _parallaxOffset = 0.0f;

		Vector2 _lastPoint;

		protected override void OnMainBackgroundRegistered(IMainBackground mainBackground)
		{
			base.OnMainBackgroundRegistered(mainBackground);

			mainBackground.OnFocusPointChanged += OnFocusPointChanged;

			_lastPoint = transform.position;

			OnFocusPointChanged(mainBackground, _lastPoint);
		}

		void OnFocusPointChanged(IMainBackground mainBackground, Vector2 confinedPoint)
		{
			var focusPoint = confinedPoint;

			// Update position of the object.

			var deltaX = focusPoint.x - _lastPoint.x;

			_lastPoint = focusPoint;
			_parallaxOffset += deltaX * (mainBackground != null ? mainBackground.ReferenceSpeed : 1) * horizontalSpeed;

			var p = transform.position;

			p.x = focusPoint.x + _parallaxOffset;

			transform.position = p;
		}
	}

}