﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace m039.Parallax
{
    public interface IParallaxManager
    {
        /// <summary>
        /// Align all ParallaxLayers with the target.
        /// </summary>
        /// <param name="target">is the object which position will be used by ParallaxLayers.</param>
        void Follow(Transform target);

        /// <summary>
        /// Align all ParallaxLayers with this position.
        /// </summary>
        /// <param name="position">this position will be used by ParallaxLayers</param>
        void Follow(Vector2 position);

        /// <summary>
        /// The default speed which is used by all ParallaxLayers.
        /// </summary>
        float ReferenceSpeed { get; }

        /// <summary>
        /// The position to align with.
        /// </summary>
        Vector2 GetFollowPosition();

        /// <summary>
        /// Get Z position for ParallaxLayer.
        /// </summary>
        /// <param name="depthOrder">the order of a layer by which will be calculated Z position</param>
        /// <returns>calculated Z position</returns>
        float GetDepth(int depthOrder);
    }

    public class ParallaxManager : MonoBehaviour, IParallaxManager
    {
        enum FollowMode
        {
            Target, Position
        }

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

        [Tooltip("Common speed for all ParallaxLayers.")]
        public float referenceSpeed = 2;

        #endregion

        const float MaxDepth = 1f;

        const float MinDepth = -1f;

        const float CurrentDepth = 0f;

        float _currentMaxDepthOrder;

        float _currentMinDepthOrder;

        readonly List<ParallaxLayer> _layers = new List<ParallaxLayer>();

        bool _initialized = false;

        Vector2 _followPosition;

        Transform _followTarget;

        FollowMode _followMode = FollowMode.Target;

        public void OnEnable()
        {
            Init(force: true);
        }

        void Init(bool force = false)
        {
            if (!_initialized || force)
            {
                _followPosition = transform.position; // Just in case.

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
            if (_followMode == FollowMode.Target && _followTarget != null)
            {
                return _followTarget.transform.position;
            } else
            {
                return _followPosition;
            }
        }

        void IParallaxManager.Follow(Transform target)
        {
            _followTarget = target;
            _followMode = FollowMode.Target;
        }

        void IParallaxManager.Follow(Vector2 position)
        {
            _followPosition = position;
            _followMode = FollowMode.Target;
        }

        #endregion
    }

}