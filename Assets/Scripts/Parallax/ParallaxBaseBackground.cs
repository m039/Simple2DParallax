using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace m039.Parallax
{

	public abstract class ParallaxBaseBackground : MonoBehaviour
	{
		protected interface IMainBackground
		{
			event System.Action<IMainBackground, Vector2> OnFocusPointChanged;

			float ReferenceSpeed { get; }

			void NotifyFocusPointChanged(Vector2 vector);
		}

		protected void RegisterMainBackground(IMainBackground mainBackground)
		{
			if (_sMainBackground != null)
			{
				Debug.LogError("The mainBackground is already registered.");
				return;
			}

			_sMainBackground = mainBackground;

			var movingObjects = FindObjectsOfType<ParallaxBaseBackground>();
			if (movingObjects != null)
			{
				foreach (var obj in movingObjects)
				{
					obj.OnMainBackgroundRegistered(_sMainBackground);
				}
			}
		}

		protected void UnregisterMainBackground(IMainBackground mainBackground)
		{
			if (_sMainBackground != mainBackground && _sMainBackground != null)
			{
				Debug.LogError("Can't unregister the background");
				return;
			}

			_sMainBackground = null;
		}

		static IMainBackground _sMainBackground;

		static bool _sStopRegisterMainBackground;

		private SpriteRenderer _spriteRenderer;

		protected SpriteRenderer SpriteRenderer
		{
			get
			{
				if (_spriteRenderer == null)
				{
					_spriteRenderer = GetComponent<SpriteRenderer>();
				}

				return _spriteRenderer;
			}
		}

		public static void NotifyFocusPointChanged(Vector2 point)
		{
			if (!Application.isPlaying)
				return;

			if (_sMainBackground == null)
			{
				Debug.LogError("MainBackground is not initalized yet. Don't use this callback in Awake.");
			}
			else
			{
				_sMainBackground.NotifyFocusPointChanged(point);
			}
		}

		protected virtual void OnEnable()
		{
		}

		protected virtual void OnDisable()
		{
		}

		protected virtual void LateUpdate()
		{
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
#if false
			if (_sMainBackground != null)
			{
				OnMainBackgroundRegistered(_sMainBackground);
			}
			else if (!_sStopRegisterMainBackground)
			{
				Debug.LogError("The main background is not registerd, please add one to use a parallax feature.");
				_sStopRegisterMainBackground = true;
			}
#endif
		}

		protected virtual void OnMainBackgroundRegistered(IMainBackground mainBackground)
		{
		}

		protected virtual void OnDestroy()
		{
		}

	}

}