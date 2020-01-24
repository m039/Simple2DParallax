using System.Collections;
using System.Collections.Generic;
using m039.Common;
using UnityEngine;

namespace m039.Parallax
{

	[ExecuteInEditMode]
	[RequireComponent(typeof(SpriteRenderer))]
	public class SquaresComplexParallaxLayer : ComplexParallaxLayer
	{

		#region Inspector

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

		bool _scheduleRegenerate = false;

		void OnValidate()
		{
			_invalidate = true;
		}

		protected override void OnEnable()
		{
			base.OnEnable();

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

		protected override void LateUpdate()
		{
			base.LateUpdate();

			if (_invalidate || _lastAspectRatio != Camera.main.aspect || transform.hasChanged || _scheduleRegenerate)
			{
				Regenerate();
				_invalidate = false;
				_lastAspectRatio = Camera.main.aspect;
				transform.hasChanged = false;
				_scheduleRegenerate = false;
			}
		}

		protected override void OnParallaxOffsetChanged()
		{
			base.OnParallaxOffsetChanged();

			_scheduleRegenerate = true;
		}

		void Regenerate()
		{
			foreach (var sprite in _sprites)
			{
				sprite.name = FreeName;
				sprite.SetActive(false);
			}

			var mainSpriteSize = Matrix4x4.Scale(transform.lossyScale) * SpriteRenderer.sprite.bounds.size;
			var mainSpriteHalfSize = mainSpriteSize / 2;
			var cameraBounds = CameraUtils.GetMainCameraBounds();

			bool IsSpriteWithinBounds(SpriteRenderer spriteRenderer)
			{
				var spriteCenter = spriteRenderer.transform.position;

				spriteCenter.z = 0;

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

			Vector3 getSpritePosition(Vector3 center, int xIndex, int yIndex)
			{
				var position = center;

				position += xIndex * transform.right * (mainSpriteSize.x + horizontalSpacing);
				position += yIndex * transform.up * (mainSpriteSize.y + verticalSpacing);

				return position;
			}

			if (!IsSpriteWithinBounds(SpriteRenderer))
			{
				var spriteCenter = (Vector2)transform.position;
				var movingDirectionRay = new Ray(spriteCenter, MovingDirection);
				var oppositeMovingDirectionRay = new Ray(spriteCenter, -MovingDirection);
				var leftPlane = new Plane(Vector3.right, cameraBounds.min);
				var topPlane = new Plane(Vector3.down, cameraBounds.max);
				var rightPlane = new Plane(Vector3.left, cameraBounds.max);
				var bottomPlane = new Plane(Vector3.up, cameraBounds.min);

				void clampSpritePosition(Vector2 direction, System.Func<int, Vector3> getPosition)
				{
					var ray = new Ray(spriteCenter, direction);
					var bounds = cameraBounds;
					var size = mainSpriteSize;

					size.x += horizontalSpacing;
					size.y += verticalSpacing;

					bounds.Expand(size * 2);

					if (!cameraBounds.IntersectRay(movingDirectionRay) &&
						bounds.IntersectRay(ray) &&
						bounds.IntersectRay(oppositeMovingDirectionRay) &&
						Vector3.Dot(ray.direction, oppositeMovingDirectionRay.direction) > 0)
					{
						int i = 1;

						while (true)
						{
							ray = new Ray(getPosition(i++), direction);

							if (!bounds.IntersectRay(ray))
							{
								ParallaxOffset += (Vector2)ray.origin - spriteCenter;
								break;
							}
						}
					}
				}

				// Bottom to up
				clampSpritePosition(transform.up, (y) => getSpritePosition(spriteCenter, 0, y));

				// Up to bottom
				clampSpritePosition(-transform.up, (y) => getSpritePosition(spriteCenter, 0, -y));

				// Left to right
				clampSpritePosition(transform.right, (x) => getSpritePosition(spriteCenter, x, 0));

				// Right to left
				clampSpritePosition(-transform.right, (x) => getSpritePosition(spriteCenter, -x, 0));
			}

			var mainSpriteCenter = transform.position;

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

				obj.transform.position = getSpritePosition(mainSpriteCenter, xIndex, yIndex);

				return spriteRenderer;
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
					var spritePosition = spriteRenderer.transform.position;

					spritePosition.z = 0;

					topRay = new Ray(spritePosition + transform.up * mainSpriteHalfSize.y, -transform.right);
					bottomRay = new Ray(spritePosition - transform.up * mainSpriteHalfSize.y, -transform.right);

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
					var spritePosition = spriteRenderer.transform.position;

					spritePosition.z = 0;

					topRay = new Ray(spritePosition + transform.up * mainSpriteHalfSize.y, transform.right);
					bottomRay = new Ray(spritePosition - transform.up * mainSpriteHalfSize.y, transform.right);

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

				createRow(y--, false);

				while (createRow(y--, false))
				{
				}

				// Center.

				createRow(0, true);

				// Top

				y = 1;

				createRow(y++, false);

				while (createRow(y++, false))
				{
				}
			}

			cretateColumns();
		}
	}

}