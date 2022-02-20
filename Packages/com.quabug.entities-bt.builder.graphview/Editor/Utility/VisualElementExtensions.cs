using System.Collections.Generic;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public static class VisualElementExtensions
    {
        public static IEnumerable<VisualElement> Ancestors(this VisualElement view)
        {
            for (var parent = view.hierarchy.parent; parent != null; parent = parent.hierarchy.parent) yield return parent;
        }
    }
}