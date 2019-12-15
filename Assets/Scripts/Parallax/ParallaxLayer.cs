using System.Collections;
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

        public int depthOrder = 0;

        [Tooltip("Скорость с которой движется объект, на котурую влияет еще общая скорость параллакса.")]
        public float horizontalSpeed = 0f;

        public bool repeatBackground = true;

        #endregion

        Vector2 _lastPosition;

        float _parallaxOffset;

        List<GameObject> _leftBackgrounds = new List<GameObject>();

        List<GameObject> _rightBackgrounds = new List<GameObject>();

        private void OnEnable()
        {
            UpdateDepth();
            CreateBackgrounds();
        }

        private void OnDisable()
        {
            RemoveBackgrounds();
        }

        void CreateBackgrounds()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                return;
            }

            RemoveBackgrounds();

            void createBackground(List<GameObject> bgs, int offsetIndex)
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

                bgs.Add(obj);
            }

            createBackground(_leftBackgrounds, -1);
            createBackground(_rightBackgrounds, 1);
        }

        void RemoveBackgrounds()
        {
            void remove(List<GameObject> bgs)
            {
                foreach (var background in bgs)
                {
                    Destroy(background);
                }

                bgs.Clear();
            }

            remove(_leftBackgrounds);
            remove(_rightBackgrounds);
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