using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace m039.Parallax
{

    public class BasicParallaxExample : MonoBehaviour
    {

        public Transform FollowTarget;

        private void Start()
        {
            ParallaxManager.Instance.FollowTarget = FollowTarget;
        }

    }

}