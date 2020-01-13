using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using UnityEngine;

namespace EntitiesBT.Components
{
    public static class Utilities
    {
        public static IEnumerable<GameObject> Children(this GameObject parent)
        {
            var transform = parent.transform;
            for (var i = 0; i < transform.childCount; i++)
                yield return transform.GetChild(i).gameObject;
        }
        
        public static IEnumerable<T> Children<T>(this T parent) where T : Component
        {
            return Children(parent.gameObject).Select(child => child.GetComponent<T>()).Where(child => child != null);
        }
        
        public static IEnumerable<T> Children<T>(this GameObject parent) where T : Component
        {
            return Children(parent).Select(child => child.GetComponent<T>()).Where(child => child != null);
        }
    }
}
