using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace m039.Parallax
{

	[ExecuteInEditMode]
	public class CircleParallaxLayer : ComplexParallaxLayer
	{

		#region Inspector

		[Header("Circle Parallax Settings")]
		public float circleBorderSize;

		public Color circleColor = Color.black;

		public bool circleSolid = false;

		#endregion

		readonly static int BorderSizeId = Shader.PropertyToID("_BorderSize");

		readonly static string SolidKeyword = "USE_SOLID";

		Vector3 _prevScale;

		bool _validate;

		Renderer _renderer;

		MeshFilter _meshFilter;

		protected override void OnEnable()
		{
			base.OnEnable();

			_validate = true;

			if (!TryGetComponent(out _renderer))
			{
				_renderer = gameObject.AddComponent<MeshRenderer>();
				_renderer.sharedMaterial = new Material(Shader.Find("Unlit/Circle"));
				//_renderer.sharedMaterial.EnableKeyword(SolidKeyword);
			}

			if (!TryGetComponent(out _meshFilter))
			{
				_meshFilter = gameObject.AddComponent<MeshFilter>();
				_meshFilter.sharedMesh = MeshUtils.CreateQuad();
			}
		}

		private void OnValidate()
		{
			_validate = true;
		}

		protected override void Start()
		{
			base.Start();

			UpdateBorderSize();
		}

		protected override void LateUpdate()
		{
			base.LateUpdate();

			if (_prevScale != transform.lossyScale || _validate)
			{
				UpdateBorderSize();
				_prevScale = transform.lossyScale;
				_validate = false;
			}
		}

		void UpdateBorderSize()
		{
			if (_renderer == null || _meshFilter == null)
				return;

			_renderer.sharedMaterial.SetFloat(BorderSizeId, circleBorderSize / transform.lossyScale.x);

			if (circleSolid)
			{
				_renderer.sharedMaterial.EnableKeyword(SolidKeyword);
			}
			else
			{
				_renderer.sharedMaterial.DisableKeyword(SolidKeyword);
			}

			_meshFilter.sharedMesh.colors = new Color[]
			{
				circleColor,
				circleColor,
				circleColor,
				circleColor
			};
		}
	}

}
