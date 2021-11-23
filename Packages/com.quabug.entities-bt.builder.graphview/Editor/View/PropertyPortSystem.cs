using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public class PropertyPortSystem
    {
        private readonly VisualElement _uiRoot;
        private readonly IConnectableVariantContainer _container;
        private readonly IDictionary<string /* property path */, ConnectableVariantView> _propertyViews = new Dictionary<string, ConnectableVariantView>();

        public PropertyPortSystem(VisualElement uiRoot, IConnectableVariantContainer container)
        {
            _uiRoot = uiRoot;
            _container = container;
            Refresh();
        }

        [CanBeNull] public ConnectableVariantView Find([NotNull] Port port)
        {
            return _propertyViews.Values.FirstOrDefault(view => view.Ports.Contains(port));
        }

        public IEnumerable<ConnectableVariantView> Views => _propertyViews.Values;

        public void Refresh()
        {
            var removedViews = new HashSet<string>(_propertyViews.Keys);
            foreach (var variant in _container.ConnectableVariants)
            {
                if (removedViews.Contains(variant.Id))
                {
                    removedViews.Remove(variant.Id);
                }
                else
                {
                    var view = new ConnectableVariantView(variant);
                    _propertyViews.Add(variant.Id, view);
                    _uiRoot.Add(view);
                }
            }

            foreach (var removed in removedViews)
            {
                var view = _propertyViews[removed];
                _propertyViews.Remove(removed);
                _uiRoot.Remove(view);
            }
        }
    }
}