using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public class PropertyPortSystem
    {
        private readonly VisualElement _container;
        private readonly IDictionary<string /* property path */, ConnectableVariantView> _propertyViews = new Dictionary<string, ConnectableVariantView>();

        public PropertyPortSystem(VisualElement container)
        {
            _container = container;
        }

        [CanBeNull] public ConnectableVariantView Find([NotNull] Port port)
        {
            return _propertyViews.Values.FirstOrDefault(view => view.Ports.Contains(port));
        }

        public void Refresh(IConnectableVariantContainer container)
        {
            var removedViews = new HashSet<string>(_propertyViews.Keys);
            foreach (var variant in container.ConnectableVariants)
            {
                if (removedViews.Contains(variant.Id))
                {
                    removedViews.Remove(variant.Id);
                }
                else
                {
                    var view = new ConnectableVariantView(variant);
                    _propertyViews.Add(variant.Id, view);
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