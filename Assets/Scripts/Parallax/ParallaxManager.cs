using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace m039.Parallax
{
    public interface IParallaxManager
    {
        Transform FollowTarget { get; set; }

        float ReferenceSpeed { get; }

        Vector2 GetFollowPosition();

        float GetDepth(int depthOrder);
    }

    [ExecuteInEditMode]
    public class ParallaxManager : MonoBehaviour, IParallaxManager
    {
        private static ParallaxManager _sInstance;

        private static bool _sErrorMessageShown = false;

        public static IParallaxManager Instance
        {
            get
            {
                if (_sInstance == null)
                {
                    var managers = FindObjectsOfType<ParallaxManager>();

                    if ((managers == null || managers.Length <= 0) && Application.isPlaying)
                    {
                        // Create a new object for the manager.

                        var obj = new GameObject(nameof(ParallaxManager));

                        _sInstance = obj.AddComponent<ParallaxManager>();

                    } else if (managers == null || managers.Length != 1)
                    {
                        // Too many managers. Not supported.

                        Utils.LogError($"There is should be one {nameof(ParallaxManager)} in the scene.", ref _sErrorMessageShown);
                        
                        return null;
                    } else
                    {
                        // All ok.

                        _sInstance = managers[0];
                    }
                }

                if (_sInstance == null)
                {
                    Utils.LogError($"Can't find the {nameof(ParallaxManager)} in the scene", ref _sErrorMessageShown);
                    
                    return null;
                }

                return _sInstance;
            }
        }

        #region Inspector

        public Transform followTarget;

        [Tooltip("Общая скорость для всех объектов, использующие параллакс.")]
        public float referenceSpeed = 2;

        #endregion

        const float MaxDepth = 1f;

        const float MinDepth = -1f;

        const float CurrentDepth = 0f;

        float _currentMaxDepthOrder;

        float _currentMinDepthOrder;

        readonly List<ParallaxLayer> _layers = new List<ParallaxLayer>();

        bool _initialized = false;

        public void OnEnable()
        {
            Init(force: true);
        }

        void Init(bool force = false)
        {
            if (_initialized || force)
            {
                _layers.Clear();
                _layers.AddRange(FindObjectsOfType<ParallaxLayer>());

                _currentMaxDepthOrder = 0;
                _currentMinDepthOrder = 0;

                foreach (var layer in _layers)
                {
                    _currentMaxDepthOrder = Mathf.Max(layer.depthOrder, _currentMaxDepthOrder);
                    _currentMinDepthOrder = Mathf.Min(layer.depthOrder, _currentMinDepthOrder);
                }

                _initialized = true;
            }
        }

        #region IParallaxManager

        Transform IParallaxManager.FollowTarget { 

            get => followTarget;

            set => followTarget = value;

        }

        float IParallaxManager.ReferenceSpeed => referenceSpeed;

        float IParallaxManager.GetDepth(int depthOrder)
        {
            Init();

            if (depthOrder > 0 && _currentMaxDepthOrder != 0)
            {
                return CurrentDepth + (float)depthOrder / Mathf.Abs(_currentMaxDepthOrder) * MaxDepth;
            } else if (depthOrder < 0 && _currentMinDepthOrder != 0)
            {
                return CurrentDepth + (float)depthOrder / Mathf.Abs(_currentMinDepthOrder) * MinDepth;
            } else
            {
                return CurrentDepth;
            }
        }

        Vector2 IParallaxManager.GetFollowPosition()
        {
            if (followTarget != null)
            {
                return followTarget.position;
            } else
            {
                return transform.position;
            }
        }

        #endregion
    }

}