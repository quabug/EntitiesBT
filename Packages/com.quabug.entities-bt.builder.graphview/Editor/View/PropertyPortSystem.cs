using System.Collections.Generic;
using Nuwa.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public class PropertyPortSystem
    {
        private readonly VisualElement _container;
        private readonly IDictionary<string /* property path */, NodePropertyView> _propertyViews = new Dictionary<string, NodePropertyView>();

        public PropertyPortSystem(VisualElement container)
        {
            _container = container;
        }

        public void Refresh(SerializedObject node)
        {
            var removedViews = new HashSet<string>(_propertyViews.Keys);
            foreach (var property in node.FindVariantProperties())
            {
                var type = property.GetManagedFullType();
                if (!typeof(GraphNodeVariant.Any).IsAssignableFrom(type)) continue;

                if (removedViews.Contains(property.propertyPath))
                {
                    removedViews.Remove(property.propertyPath);
                }
                else
                {
                    var view = new NodePropertyView(type);
                    _propertyViews.Add(property.propertyPath, view);
                    _container.Add(view);
                }
            }

            foreach (var removed in removedViews)
            {
                var view = _propertyViews[removed];
                _propertyViews.Remove(removed);
                _container.Remove(view);
            }
        }
    }
}