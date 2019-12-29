using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EntitiesBT.Components
{
    public static class Utilities
    {
        public static IEnumerable<T> Yield<T>(this T self)
        {
            yield return self;
        }

        public static IEnumerable<GameObject> Descendants(this GameObject root)
        {
            return Children(root).SelectMany(DescendantsWithSelf);
        }
        
        public static IEnumerable<GameObject> DescendantsWithSelf(this GameObject root)
        {
            return root.Yield().Concat(Descendants(root));
        }
        
        // will skip any branch which root node does not contains T component.
        public static IEnumerable<T> Descendants<T>(this GameObject root) where T : Component
        {
            return Children<T>(root).SelectMany(DescendantsWithSelf);
        }
        
        // will skip any branch which root node does not contains T component.
        public static IEnumerable<T> DescendantsWithSelf<T>(this T root) where T : Component
        {
            return root.Yield().Concat(Descendants<T>(root.gameObject));
        }

        public static IEnumerable<GameObject> Children(this GameObject parent)
        {
            var transform = parent.transform;
            for (var i = 0; i < transform.childCount; i++)
                yield return transform.GetChild(i).gameObject;
        }
        
        public static IEnumerable<T> Children<T>(this GameObject parent) where T : Component
        {
            return Children(parent).Select(child => child.GetComponent<T>()).Where(child => child != null);
        }
    }
}
