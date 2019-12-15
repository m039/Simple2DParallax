using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace m039.Parallax
{
    [ExecuteInEditMode]
    public class ParallaxLayer : MonoBehaviour
    {
        #region Inspector

        public int depthOrder = 0;

        #endregion

        private void OnEnable()
        {
            UpdateDepth();   
        }

        void UpdateDepth()
        {
            if (ParallaxManager.Instance == null)
                return;

            var position = transform.position;

            position.z = ParallaxManager.Instance.GetDepth(depthOrder);

            transform.position = position;
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