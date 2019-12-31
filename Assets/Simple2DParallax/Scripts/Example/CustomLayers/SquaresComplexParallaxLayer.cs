using System.Collections;
using System.Collections.Generic;
using m039.Common;
using UnityEngine;

namespace m039.Parallax
{

	[ExecuteInEditMode]
	[RequireComponent(typeof(SpriteRenderer))]
	public class SquaresComplexParallaxLayer : BaseParallaxLayer
	{

		#region Inspector

		[Header("Squares Parallax Settings")]
		[Range(0f, 360f)]
		public float angle;

		public float speed;

		public float horizontalSpacing = 0;

		public float verticalSpacing = 0;

		#endregion

		static readonly string FreeName = "Free".Decorate();

		static readonly string SpriteGroupName = "SpriteGroup".Decorate();

		readonly List<GameObject> _sprites = new List<GameObject>();

		SpriteRenderer _spriteRenderer;

		float _lastAspectRatio = float.NaN;

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

		bool _invalidate = false;

		Transform _spriteGroup;

		void OnValidate()
		{
			_invalidate = true;
		}

		private void OnEnable()
		{
			_spriteGroup = transform.Find(SpriteGroupName);

			// Create an object for holding sprites.

			if (_spriteGroup == null)
			{
				var obj = new GameObject(SpriteGroupName);
				obj.transform.SetParent(transform, worldPositionStays: false);
				_spriteGroup = obj.transform;
			}

			// Find already created objects.

			_sprites.Clear();

			for (int i = 0; i < _spriteGroup.childCount; i++)
			{
				_sprites.Add(_spriteGroup.GetChild(i).gameObject);
			}

			Regenerate();
		}

		private void LateUpdate()
		{
			if (_invalidate || _lastAspectRatio != Camera.main.aspect || transform.hasChanged)
			{
				Regenerate();
				_invalidate = false;
				_lastAspectRatio = Camera.main.aspect;
				transform.hasChanged = false;
			}
		}

		void Regenerate()
		{
			foreach (var sprite in _sprites)
			{
				sprite.name = FreeName;
				sprite.SetActive(false);
			}

			var mainSpriteCenter = transform.position;
			var mainSpriteSize = Matrix4x4.Scale(transform.lossyScale) * SpriteRenderer.sprite.bounds.size;
			var mainSpriteHalfSize = mainSpriteSize / 2;
			var cameraBounds = CameraUtils.GetMainCameraBounds();

			SpriteRenderer createOrUpdateSprite(int xIndex, int yIndex)
			{
				var obj = _sprites.Find((o) => o.name.Equals(FreeName));

				if (obj == null)
				{
					obj = new GameObject();
					obj.transform.SetParent(_spriteGroup, worldPositionStays: false);
					obj.AddComponent<SpriteRenderer>();

					_sprites.Add(obj);
				}

				obj.name = $"Sprite@({xIndex}, {yIndex})".Decorate();
				obj.SetActive(true);

				var spriteRenderer = obj.GetComponent<SpriteRenderer>();
				spriteRenderer.sprite = SpriteRenderer.sprite;
				spriteRenderer.color = SpriteRenderer.color;

				var position = mainSpriteCenter;

				position += xIndex * transform.right * (mainSpriteSize.x + horizontalSpacing);
				position += yIndex * transform.up * (mainSpriteSize.y + verticalSpacing);

				obj.transform.position = position;

				return spriteRenderer;
			}

			bool IsSpriteWithinBounds(SpriteRenderer spriteRenderer)
			{
				var spriteCenter = spriteRenderer.transform.position;

				// Check all coners.

				var topLeftCorner = spriteCenter -
					spriteRenderer.transform.right * mainSpriteHalfSize.x +
					spriteRenderer.transform.up * mainSpriteHalfSize.y;

				var topRightCorner = spriteCenter +
					spriteRenderer.transform.right * mainSpriteHalfSize.x +
					spriteRenderer.transform.up * mainSpriteHalfSize.y;

				var bottomLeftCorner = spriteCenter -
					spriteRenderer.transform.right * mainSpriteHalfSize.x -
					spriteRenderer.transform.up * mainSpriteHalfSize.y;

				var bottomRightCorner = spriteCenter +
					spriteRenderer.transform.right * mainSpriteHalfSize.x -
					spriteRenderer.transform.up * mainSpriteHalfSize.y;

				return cameraBounds.Contains(topLeftCorner) ||
					cameraBounds.Contains(topRightCorner) ||
					cameraBounds.Contains(bottomLeftCorner) ||
					cameraBounds.Contains(bottomRightCorner);
			}

			bool createRow(int yIndex, bool excludeCenter)
			{
				bool hasSpriteWithinBounds = false;
				int x = -1;
				Ray topRay, bottomRay;

				// Left.

				while (true)
				{
					var spriteRenderer = createOrUpdateSprite(x--, yIndex);

					topRay = new Ray(spriteRenderer.transform.position + transform.up * mainSpriteHalfSize.y, -transform.right);
					bottomRay = new Ray(spriteRenderer.transform.position - transform.up * mainSpriteHalfSize.y, -transform.right);

					if (!IsSpriteWithinBounds(spriteRenderer) && !cameraBounds.IntersectRay(topRay) && !cameraBounds.IntersectRay(bottomRay))
					{
						break;
					}

					hasSpriteWithinBounds = true;
				}

				// Center.

				if (!excludeCenter)
				{
					var spriteRenderer = createOrUpdateSprite(0, yIndex);
					if (IsSpriteWithinBounds(spriteRenderer))
					{
						hasSpriteWithinBounds = true;
					}
				}

				// Right

				x = 1;

				while (true)
				{
					var spriteRenderer = createOrUpdateSprite(x++, yIndex);

					topRay = new Ray(spriteRenderer.transform.position + transform.up * mainSpriteHalfSize.y, transform.right);
					bottomRay = new Ray(spriteRenderer.transform.position - transform.up * mainSpriteHalfSize.y, transform.right);

					if (!IsSpriteWithinBounds(spriteRenderer) && !cameraBounds.IntersectRay(topRay) && !cameraBounds.IntersectRay(bottomRay))
					{
						break;
					}

					hasSpriteWithinBounds = true;
				}

				return hasSpriteWithinBounds;
			}

			void cretateColumns()
			{
				int y = -1;

				// Bottom.

				while (createRow(y--, false))
				{
				}

				// Center.

				createRow(0, true);

				// Top

				y = 1;

				while (createRow(y++, false))
				{
				}
			}

			cretateColumns();
		}
	}

}