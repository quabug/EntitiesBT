using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using JetBrains.Annotations;
using UnityEngine;

namespace EntitiesBT.Components
{
    public static class Utilities
    {
        [Pure]
        public static IEnumerable<GameObject> Children(this GameObject parent)
        {
            var transform = parent.transform;
            for (var i = 0; i < transform.childCount; i++)
                yield return transform.GetChild(i).gameObject;
        }

        [Pure]
        public static IEnumerable<T> Children<T>(this Component parent)
        {
            return Children(parent.gameObject).Select(child => child.GetComponent<T>()).Where(child => child != null);
        }

        [Pure]
        public static IEnumerable<T> Children<T>(this T parent) where T : Component
        {
            return Children(parent.gameObject).Select(child => child.GetComponent<T>()).Where(child => child != null);
        }
        
        [Pure]
        public static IEnumerable<T> Children<T>(this GameObject parent)
        {
            return Children(parent).Select(child => child.GetComponent<T>()).Where(child => child != null);
        }
        
        [Pure]
        public static bool IsPrefab([NotNull] this GameObject gameObject)
        {
            return !gameObject.scene.IsValid() && !gameObject.scene.isLoaded;
        }

        [Pure]
        public static IEnumerable<GameObject> DescendantsAndSelf(this GameObject gameObject)
        {
            yield return gameObject;

            foreach (var descendant in
                from child in Children(gameObject)
                from descendant in DescendantsAndSelf(child)
                select descendant
            ) yield return descendant;
        }

        [Pure]
        public static IEnumerable<GameObject> Descendants(this GameObject gameObject)
        {
            return DescendantsAndSelf(gameObject).Skip(1); // skip self
        }

        [Pure, CanBeNull]
        public static GameObject Parent(this GameObject gameObject)
        {
            var parent = gameObject.transform.parent;
            return parent == null ? null : parent.gameObject;
        }

        [Pure]
        public static IEnumerable<GameObject> AncestorsAndSelf(this GameObject gameObject)
        {
            var transform = gameObject == null ? null : gameObject.transform;
            while (transform != null)
            {
                yield return transform.gameObject;
                transform = transform.parent;
            }
        }

        [Pure]
        public static IReadOnlyList<IGlobalValuesBuilder> FindGlobalValuesList(this GameObject root)
        {
            return root.DescendantsAndSelf()
                .Select(obj => obj.GetComponent<IGlobalValuesBuilder>())
                .Where(scopeValues => scopeValues != null)
                .ToArray()
            ;
        }

        [Pure]
        public static IReadOnlyList<IGlobalValuesBuilder> FindGlobalValuesList(this Component root)
        {
            return root.gameObject.FindGlobalValuesList();
        }
    }
}
