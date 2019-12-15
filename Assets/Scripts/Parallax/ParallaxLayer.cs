using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace m039.Parallax
{

    public class ParallaxLayer : MonoBehaviour
    {
        #region Inspector

        [Tooltip("An order of the ParllaxLayer in Z direction.")]
        public int depthOrder = 0;

        [Tooltip("Speed used to parallax the background which is also affected by ReferenceSpeed")]
        public float horizontalSpeed = 0f;

        [Tooltip("Repeat the background sprite horizontally.")]
        public bool repeatBackground = true;

        #endregion

        Vector2 _lastPosition;

        float _parallaxOffset;

        List<GameObject> _backgrounds = new List<GameObject>();

        private void OnEnable()
        {
            UpdateDepth();

            if (repeatBackground)
            {
                CreateBackgrounds();
            }
        }

        private void OnDisable()
        {
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

            var width = Camera.main.orthographicSize * Camera.main.aspect;
            var times = Mathf.CeilToInt(width / spriteRenderer.bounds.size.x) + 1;

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

        private void Start()
        {
            if (ParallaxManager.Instance == null)
                return;

            _parallaxOffset = 0.0f;
            _lastPosition = ParallaxManager.Instance.GetFollowPosition();

            FollowPosition();
        }

        private void LateUpdate()
        {
            FollowPosition();
            UpdateBackgrounds();
        }

        void UpdateDepth()
        {
            if (ParallaxManager.Instance == null)
                return;

            var position = transform.position;

            position.z = ParallaxManager.Instance.GetDepth(depthOrder);

            transform.position = position;
        }

        void FollowPosition()
        {
            if (ParallaxManager.Instance == null)
                return;

            var followPosition = ParallaxManager.Instance.GetFollowPosition();

            // Update position of the object.

            var deltaX = followPosition.x - _lastPosition.x;

            _lastPosition = followPosition;
            _parallaxOffset += deltaX * ParallaxManager.Instance.ReferenceSpeed * horizontalSpeed;

            var p = transform.position;

            p.x = followPosition.x + _parallaxOffset;

            transform.position = p;
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

            _parallaxOffset %= width;
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(ParallaxLayer), true)]
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
            var t = (ParallaxLayer)target;

            if (t.GetComponent<SpriteRenderer>() == null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(
                    $"<color=maroon>There is should be {nameof(SpriteRenderer)} attached " +
                    $"for {nameof(ParallaxLayer)} to work.</color>",
                    _errorLabelStyle);
                EditorGUILayout.Space();
            } else
            {
                base.OnInspectorGUI();
            }

            
        }

    }

#endif

}