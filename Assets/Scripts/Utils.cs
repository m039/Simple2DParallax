using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace m039.Parallax
{

    public static class Utils
    {
        static public void LogError(string message, ref bool guard)
        {
            if (!guard)
            {
                Debug.LogError(message);
                guard = true;
            }
        }

        public static string Decorate(this string str)
        {
            return $"<<{ str }>>"; ;
        }
    }

}