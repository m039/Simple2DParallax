using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LH
{

	public class ParallaxMainBackground : ParallaxBaseBackground
	{
		public Bounds Bounds => SpriteRenderer.bounds;

		#region Inspector

		[Tooltip("Общая скорость для всех объектов, использующие параллакс.")]
		public float referenceSpeed = 2;

		#endregion

		MainBackgroundInternal _internal;

		protected override void Awake()
		{
			base.Awake();

			RegisterMainBackground(_internal = new MainBackgroundInternal(this));
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			UnregisterMainBackground(_internal);
		}

		class MainBackgroundInternal : IMainBackground
		{
			readonly ParallaxMainBackground _parent;

			public float ReferenceSpeed => _parent.referenceSpeed;

			public event Action<IMainBackground, Vector2> OnFocusPointChanged;

			public MainBackgroundInternal(ParallaxMainBackground parent)
			{
				_parent = parent;
			}

			public void NotifyFocusPointChanged(Vector2 point)
			{
                OnFocusPointChanged?.Invoke(this, point);
            }
        }
	}

}
