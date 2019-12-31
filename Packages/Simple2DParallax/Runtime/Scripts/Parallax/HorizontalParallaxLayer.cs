using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace m039.Parallax
{

	public class HorizontalParallaxLayer : BaseParallaxLayer
	{
		#region Inspector

		[Header("Horizontal Parallax Settings")]
		[Tooltip("Speed used to parallax the background which is also affected by ReferenceSpeed")]
		public float horizontalSpeed = 0f;

		[Tooltip("Repeat the background sprite horizontally.")]
		public bool repeatBackground = true;

		#endregion

		List<GameObject> _backgrounds = new List<GameObject>();

		protected override Vector2 Direction => Vector2.right;

		protected override float Speed => horizontalSpeed;

		protected override void OnEnable()
		{
			base.OnEnable();

			if (repeatBackground)
			{
				CreateBackgrounds();
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			if (repeatBackground)
			{
				RemoveBackgrounds();
			}
		}

		void CreateBackgrounds()
		{
			var spriteRenderer = GetComponent<SpriteRenderer>();
			if (spriteRenderer == null)
			{
				return;
			}

			RemoveBackgrounds();

			void createBackground(int offsetIndex)
			{
				// Create the object.

				var obj = new GameObject($"Background[{offsetIndex}]".Decorate());

				obj.transform.parent = transform;

				// Init sprite settings.

				var sr = obj.AddComponent<SpriteRenderer>();
				sr.sprite = spriteRenderer.sprite;

				// Init position.

				var bounds = spriteRenderer.bounds;
				obj.transform.position = transform.position + offsetIndex * Vector3.right * bounds.size.x;

				// Add to the list.

				_backgrounds.Add(obj);
			}

			var width = Camera.main.orthographicSize * 2 * Camera.main.aspect;
			var times = Mathf.CeilToInt(width / (spriteRenderer.bounds.size.x)) + 1;

			for (int i = 1; i < times; i++)
			{
				createBackground(-i);
				createBackground(i);
			}
		}

		void RemoveBackgrounds()
		{
			foreach (var background in _backgrounds)
			{
				Destroy(background);
			}

			_backgrounds.Clear();
		}

		protected override void LateUpdate()
		{
			base.LateUpdate();

			UpdateBackgrounds();
		}

		void UpdateBackgrounds()
		{
			var spriteRenderer = GetComponent<SpriteRenderer>();
			if (spriteRenderer == null)
			{
				return;
			}

			var width = spriteRenderer.bounds.size.x;
			if (width == 0)
				return;

			var offset = ParallaxOffset;

			offset.x %= width;

			ParallaxOffset = offset;
		}
	}

#if UNITY_EDITOR

	[CustomEditor(typeof(HorizontalParallaxLayer), true)]
	public class ParallaxLayerEditor : Editor
	{
		GUIStyle _errorLabelStyle;

		private void OnEnable()
		{
			_errorLabelStyle = new GUIStyle();
			_errorLabelStyle.fontStyle = FontStyle.Bold;
			_errorLabelStyle.richText = true;
			_errorLabelStyle.wordWrap = true;
			_errorLabelStyle.alignment = TextAnchor.MiddleCenter;
		}

		public override void OnInspectorGUI()
		{
			var t = (HorizontalParallaxLayer)target;

			if (t.GetComponent<SpriteRenderer>() == null)
			{
				EditorGUILayout.Space();
				EditorGUILayout.LabelField(
					$"<color=maroon>There is should be {nameof(SpriteRenderer)} attached " +
					$"for {nameof(HorizontalParallaxLayer)} to work.</color>",
					_errorLabelStyle);
				EditorGUILayout.Space();
			}
			else
			{
				base.OnInspectorGUI();
			}


		}

	}

#endif

}